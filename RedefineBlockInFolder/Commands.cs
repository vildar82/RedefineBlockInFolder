using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(RedefineBlockInFolder.Commands))]

namespace RedefineBlockInFolder
{
   public class Commands :IExtensionApplication
   {
      [CommandMethod("RedefineBlockInFolder", CommandFlags.Modal)]
      public void RedefineBlockCommand()
      {
         Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
         if (doc == null) return;
         Editor ed = doc.Editor;
         Database db = doc.Database;

         using (var ld = doc.LockDocument())
         {
            try
            {
               ObjectIdCollection idsSource;
               string blName;

               // Запрос блока для переопределения               
               GetBlock(ed, db, out idsSource, out blName);

               // Запрос папки для переопределения (рекурсивно?)
               List<FileInfo> filesDwg = GetDir(doc, ed);

               // Перебор всех файлов dwg папке, открытие базы, поиск блока и переопределение.
               RedefBlockInFiles(ed, db, idsSource, blName, filesDwg);
            }
            catch (System.Exception ex)
            {
               ed.WriteMessage("\n" + ex.Message);
            }
         }
      }

      private void RedefBlockInFiles(Editor ed, Database db, ObjectIdCollection idsSource, string blName, List<FileInfo> filesDwg)
      {
         int countFilesRedefined = 0;
         int countFilesWithoutBlock = 0;
         int countFilesError = 0;

         // Прогресс бар
         ProgressMeter pBar = new ProgressMeter();
         pBar.Start("Переопределение блоков в файлах... нажмите Esc для отмены");         
         pBar.SetLimit(filesDwg.Count);

         // Фильтр отслеживаемых сообщений системы
         MyMessageFilter filter = new MyMessageFilter();
         System.Windows.Forms.Application.AddMessageFilter(filter);         

         foreach (var file in filesDwg)
         {
            // Отслеживание нажатия Esc
            System.Windows.Forms.Application.DoEvents();
            if (filter.bCanceled == true)
            {
               pBar.Stop();
               throw new System.Exception("Операция прервана.");
            }

            // Переопределение блока в файле
            try
            {              
               RedefineBlockInFile(ed, db, idsSource, blName, file, ref countFilesRedefined, ref countFilesWithoutBlock);               
            }
            catch (System.Exception ex)
            {
               ed.WriteMessage("\nОшибка при переопределении блока в файле " + file.FullName);
               ed.WriteMessage("\nТекст ошибки " + ex.Message);
            }            
            pBar.MeterProgress();
         }

         System.Windows.Forms.Application.RemoveMessageFilter(filter);
         pBar.Stop();

         ed.WriteMessage("\nПереопределен блок в " + countFilesRedefined + " файле.");
         if (countFilesWithoutBlock != 0)
            ed.WriteMessage("\nНет блока в " + countFilesWithoutBlock + " файле.");
         if (countFilesError != 0)
            ed.WriteMessage("\nОшибка при переопределении блока в " + countFilesError + " файле.");
         ed.WriteMessage("\nГотово");
      }

      private List<FileInfo> GetDir(Document doc, Editor ed)
      {
         var dir = GetFiles(ed);
         List<FileInfo> filesDwg = dir.GetFiles("*.dwg", IncludeSubdirs(ed, dir)).ToList();
         // Если выбрана папка текущего чертежа               
         if (Path.GetFullPath(dir.FullName).Equals(Path.GetDirectoryName(doc.Name), StringComparison.OrdinalIgnoreCase))
         {
            // удалить файл чертежа из списка файлов
            foreach (var file in filesDwg)
            {
               if (Path.GetFileName(doc.Name).Equals(Path.GetFileName(file.Name)))
               {
                  filesDwg.Remove(file);
                  break;
               }
            }
         }
         ed.WriteMessage("\nОбщее количество файлов для переопределения: " + filesDwg.Count);
         return filesDwg;
      }

      private void GetBlock(Editor ed, Database db, out ObjectIdCollection idsSource, out string blName)
      {
         ObjectId idBlRefSource = GetBlRefToRedefine(ed);
         if (!idBlRefSource.IsValid)
         {
            throw new System.Exception("Блок инвалид!");
         }

         idsSource = new ObjectIdCollection();
         idsSource.Add(idBlRefSource);
         blName = "";
         using (var t = db.TransactionManager.StartTransaction())
         {
            var blRefSource = t.GetObject(idBlRefSource, OpenMode.ForRead) as BlockReference;
            if (blRefSource.IsDynamicBlock)
               throw new System.Exception("Динамические блоки не поддерживаются пока (");
            if (blRefSource.AttributeCollection.Count > 0)
               throw new System.Exception("Блоки с атрибутами не поддерживаются пока (");
            blName = blRefSource.Name;
            t.Commit();
         }
         ed.WriteMessage("\nБлок для переопределения - " + blName);
      }

      private ObjectId GetBlRefToRedefine(Editor ed)
      {
         var opt = new PromptEntityOptions("\nВыбор блока");
         opt.SetRejectMessage("\nТолько блоки");
         opt.AddAllowedClass(typeof(BlockReference), true);
         var res = ed.GetEntity(opt);
         if (res.Status == PromptStatus.OK)
         {
            return res.ObjectId;
         }
         throw new System.Exception("Не выбран блок.");
      }

      private DirectoryInfo GetFiles(Editor ed)
      {
         // Запрос папки
         //   Autodesk.AutoCAD.Windows.OpenFileDialog ofdAcad = new Autodesk.AutoCAD.Windows.OpenFileDialog("Выбор папки", Path.GetDirectoryName (ed.Document.Name) , "", "Выбор папки", Autodesk.AutoCAD.Windows.OpenFileDialog.OpenFileDialogFlags.AllowFoldersOnly);         
         //if (ofdAcad.ShowDialog() == DialogResult.OK)
         FolderBrowserDialog folderDlg = new FolderBrowserDialog();
         folderDlg.Description = "Выбор папки для переопределения блока";
         folderDlg.ShowNewFolderButton = false;
         //folderDlg.RootFolder = Environment.SpecialFolder.MyComputer;
         folderDlg.SelectedPath = Path.GetDirectoryName(ed.Document.Name);
         if (folderDlg.ShowDialog() == DialogResult.OK)
         {
            DirectoryInfo dir = new DirectoryInfo(folderDlg.SelectedPath);
            if (!dir.Exists)
            {
               throw new System.Exception("Папки не существует " + dir.FullName);
            }            

            return dir;
         }
         throw new System.Exception("Не выбрана папка.");
      }

      private static SearchOption IncludeSubdirs(Editor ed, DirectoryInfo dir)
      {
         // Вопрос - включая подпапки?
         SearchOption recursive = SearchOption.AllDirectories;
         if (dir.GetDirectories().Length > 0)
         {
            var opt = new PromptKeywordOptions("\nВключая подпапки");
            opt.Keywords.Add("Да");
            opt.Keywords.Add("Нет");
            opt.Keywords.Default = "Да";
            var res = ed.GetKeywords(opt);
            if (res.Status == PromptStatus.OK)
            {
               if (res.StringResult == "Нет")
               {
                  recursive = SearchOption.TopDirectoryOnly;
               }
            }
         }
         ed.WriteMessage("\nПапка для переопределения блока " + dir.FullName);
         if (recursive == SearchOption.AllDirectories)
            ed.WriteMessage("\nВключая подпапки");
         else
            ed.WriteMessage("\nТолько в этой папке, без подпапок.");
         return recursive;
      }

      private void RedefineBlockInFile(Editor ed, Database db, ObjectIdCollection idsSource,
                                                string blName, FileInfo file, ref int countFilesRedefined, ref int countFilesWithoutBlock)
      {
         using (Database dbExt = new Database(false, true))
         {
            dbExt.ReadDwgFile(file.FullName, FileShare.ReadWrite, false, "");
            dbExt.CloseInput(true);

            using (var t = dbExt.TransactionManager.StartTransaction())
            {
               var btExt = t.GetObject(dbExt.BlockTableId, OpenMode.ForRead) as BlockTable;
               if (btExt.Has(blName))
               {
                  var map = new IdMapping();
                  // Копирование блока с переопредедлением.
                  db.WblockCloneObjects(idsSource, btExt.ObjectId, map, DuplicateRecordCloning.Replace, false);
                  dbExt.SaveAs(file.FullName, DwgVersion.Current);
                  ed.WriteMessage("\n" + file.FullName + " - ок.");
                  countFilesRedefined++;
               }
               else
               {
                  ed.WriteMessage("\n" + file.FullName + " - нет блока.");
                  countFilesWithoutBlock++;
               }
               t.Commit();
            }
         }
      }

      public void Initialize()
      {
         Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
         if (doc == null) return;
         Editor ed = doc.Editor;
         ed.WriteMessage("\nЗагружена программа для переопределения выбранного блока в папке.");
         ed.WriteMessage("\nКоманда - RedefineBlockInFolder.");
      }

      public void Terminate()
      {
         
      }
   }
}