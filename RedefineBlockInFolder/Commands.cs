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
      [CommandMethod("PIK", "RedefineBlocksInFolder", CommandFlags.Modal)]
      public void RedefineBlockCommand()
      {
         Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
         if (doc == null) return;
         Editor ed = doc.Editor;
         Database db = doc.Database;

         try
         {
            ObjectIdCollection idsSource = GetBlocks();

            // Запрос папки для переопределения (рекурсивно?)
            List<FileInfo> filesDwg = GetDir(doc, ed);

            // Перебор всех файлов dwg папке, открытие базы, поиск блока и переопределение.
            RedefBlockInFiles(ed, db, idsSource, filesDwg);
         }
         catch (System.Exception ex)
         {
            ed.WriteMessage("\n" + ex.Message);
         }
      }

      private void RedefBlockInFiles(Editor ed, Database db, ObjectIdCollection idsSource, List<FileInfo> filesDwg)
      {
         int countFilesRedefined = 0;
         int countFilesWithoutBlock = 0;
         int countFilesError = 0;

         // Прогресс бар
         ProgressMeter pBar = new ProgressMeter();
         pBar.Start("Переопределение блоков в файлах... нажмите Esc для отмены");         
         pBar.SetLimit(filesDwg.Count);         

         foreach (var file in filesDwg)
         {
            // Переопределение блока в файле
            try
            {              
               RedefineBlockInFile(ed, db, idsSource, file, ref countFilesRedefined, ref countFilesWithoutBlock);               
            }
            catch (System.Exception ex)
            {
               ed.WriteMessage("\nОшибка при переопределении блока в файле " + file.FullName);
               ed.WriteMessage("\nТекст ошибки " + ex.Message);
            }            
            pBar.MeterProgress();
         }
         
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

      private ObjectIdCollection GetBlocks()
      {
         // Список блоков в чертеже
         ObjectIdCollection resVal = null;
         Dictionary<ObjectId,string> idsBlocks = new Dictionary<ObjectId,string>();
         Database db = HostApplicationServices.WorkingDatabase;
         using (var bt = db.BlockTableId.Open( OpenMode.ForRead) as BlockTable)
         {
            foreach (var idBtr in bt)
            {
               using (var btr = idBtr.Open( OpenMode.ForRead) as BlockTableRecord)
               {
                  if (!btr.IsLayout && !btr.IsAnonymous && !btr.IsDependent)
                  {
                     idsBlocks.Add(btr.Id, btr.Name);
                  }
               }
            }
         }

         // Выбор блоков для переопределения
         FormSelectBlocks formSelBlocks = new FormSelectBlocks(idsBlocks);
         if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(formSelBlocks) == DialogResult.OK)
         {
            resVal = new ObjectIdCollection(formSelBlocks.SelectedBlocks.ToArray());            
         }
         else
         {
            throw new System.Exception("Отменено пользователем.");
         }
         return resVal;
      }      

      private DirectoryInfo GetFiles(Editor ed)
      {
         // Запрос папки
         //   Autodesk.AutoCAD.Windows.OpenFileDialog ofdAcad = new Autodesk.AutoCAD.Windows.OpenFileDialog("Выбор папки", Path.GetDirectoryName (ed.Document.Name) , "", "Выбор папки", Autodesk.AutoCAD.Windows.OpenFileDialog.OpenFileDialogFlags.AllowFoldersOnly);         
         //if (ofdAcad.ShowDialog() == DialogResult.OK)
         FolderBrowserDialog folderDlg = new FolderBrowserDialog();
         folderDlg.Description = "Выбор папки для переопределения блоков";
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
                       FileInfo file, ref int countFilesRedefined, ref int countFilesWithoutBlock)
      {
         using (Database dbExt = new Database(false, true))
         {
            dbExt.ReadDwgFile(file.FullName, FileShare.ReadWrite, false, "");
            dbExt.CloseInput(true);

            using (var map = new IdMapping())
            {
               // Копирование блока с переопредедлением.
               dbExt.WblockCloneObjects(idsSource, dbExt.BlockTableId, map, DuplicateRecordCloning.Replace, false);
               dbExt.SaveAs(file.FullName, DwgVersion.Current);
               ed.WriteMessage("\n" + file.FullName + " - ок.");
               countFilesRedefined++;
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