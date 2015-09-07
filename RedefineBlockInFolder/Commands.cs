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
               // Запрос блока для переопределения
               ObjectId idBlRefSource = GetBlRefToRedefine(ed);
               if (!idBlRefSource.IsValid)
               {
                  throw new System.Exception("Блок инвалид!");
               }
               var idsSource = new ObjectIdCollection();
               idsSource.Add(idBlRefSource);
               string blName = "";
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

               // Запрос папки для переопределения (рекурсивно?)
               List<FileInfo> filesDwg = GetFiles(ed);
               ed.WriteMessage("\nОбщее количество файлов для переопределения: " + filesDwg.Count);

               // Перебор всех файлов dwg папке, открытие базы, поиск блока и переопределение.
               int countFilesRedefined = 0;
               int countFilesWithoutBlock = 0;
               int countFilesError = 0;
               foreach (var file in filesDwg)
               {
                  try
                  {
                     RedefineBlockInFile(ed, db, idsSource, blName, file, ref countFilesRedefined, ref countFilesWithoutBlock);
                  }
                  catch (System.Exception ex)
                  {
                     ed.WriteMessage("\nОшибка при переопределении блока в файле " + file.FullName);
                     ed.WriteMessage("\nТекст ошибки " + ex.Message);
                  }
               }
               ed.WriteMessage("\nПереопределен блок в " + countFilesRedefined + " файле.");
               if (countFilesWithoutBlock != 0)
                  ed.WriteMessage("\nНет блока в " + countFilesWithoutBlock + " файле.");
               if (countFilesError != 0)
                  ed.WriteMessage("\nОшибка при переопределении блока в " + countFilesError + " файле.");
               ed.WriteMessage("\nГотово");
            }
            catch (System.Exception ex)
            {
               ed.WriteMessage("\n" + ex.Message);
            }
         }
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

      private List<FileInfo> GetFiles(Editor ed)
      {
         // Запрос папки
         //FolderBrowserDialog folderDlg = new FolderBrowserDialog();
         //folderDlg.Description = "Выбор папки для переопределения блока";
         //folderDlg.ShowNewFolderButton = false;
         //if (folderDlg.ShowDialog() == DialogResult.OK)
         Autodesk.AutoCAD.Windows.OpenFileDialog ofdAcad = new Autodesk.AutoCAD.Windows.OpenFileDialog("Выбор папки", "", "", "Выбор папки", Autodesk.AutoCAD.Windows.OpenFileDialog.OpenFileDialogFlags.AllowFoldersOnly);
         if (ofdAcad.ShowDialog() == DialogResult.OK)
         {
            DirectoryInfo dir = new DirectoryInfo(ofdAcad.Filename);
            if (!dir.Exists)
            {
               throw new System.Exception("Папки не существует " + dir.FullName);
            }
            // Вопрос - включая подпапки?
            SearchOption recursive = SearchOption.AllDirectories;
            if (dir.GetDirectories().Length > 0)
            {
               var opt = new PromptKeywordOptions("Включая подпапки");
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

            return dir.GetFiles("*.dwg", recursive).ToList();
         }
         throw new System.Exception("Не выбрана папка.");
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