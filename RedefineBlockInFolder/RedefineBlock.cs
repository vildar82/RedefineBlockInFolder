using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace RedefineBlockInFolder
{
    public class RedefineBlock
    {
        public ObjectId IdBtr { get; set; }
        public string Name { get; set; }   
        public string OldName { get; set; }    
        public bool IsDynamic { get; set; }
        public bool IsChangeBasePoint { get; set; }
        private Matrix3d MatChangeBasePoint { get; set; }

        public RedefineBlock(BlockTableRecord btr)
        {
            IdBtr = btr.Id;
            Name = btr.Name;
            OldName = Name;
            IsDynamic = btr.IsDynamicBlock;
        }

        public void ChangeBasePoint()
        {
            // Вставка блока
            var blRefId = AcadLib.Blocks.BlockInsert.Insert(OldName);
                        
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;

            // Выбор новой базовой точки
            var ptInput = ed.GetPointWCS("\nВыбор новой базовой точки блока:");            

            using (var t = db.TransactionManager.StartTransaction())
            {
                var blRef = blRefId.GetObject(OpenMode.ForRead, false, true) as BlockReference;
                var ptNewBase = ptInput- blRef.Position;
                // Вектор смещения базовой точки вхождения блока
                var ptBaseInBtr = ptNewBase.TransformBy(blRef.BlockTransform);
                var vec = new Vector3d(ptBaseInBtr.ToArray());
                // Вектор смещения объектов в блоке
                var vecBtr = vec.Negate();

                // Перемещение всех объектов блока на заданные вектор
                var matInBtr = Matrix3d.Displacement(vecBtr);                    
                var btr = IdBtr.GetObject(OpenMode.ForRead) as BlockTableRecord;
                foreach (var idEnt in btr)
                {
                    var ent = idEnt.GetObject(OpenMode.ForWrite, false, true) as Entity;
                    ent.TransformBy(matInBtr);
                }

                // Перемещение всех вхождений блока
                MatChangeBasePoint = Matrix3d.Displacement(vec);
                var refs = btr.GetBlockReferenceIds(true, false);
                foreach (ObjectId itemBlRefId in refs)
                {
                    var itemBlRef = itemBlRefId.GetObject( OpenMode.ForWrite, false, true) as BlockReference;
                    itemBlRef.TransformBy(MatChangeBasePoint);
                }

                if (IsDynamic)
                {
                    btr.UpdateAnonymousBlocks();
                    var anonyms = btr.GetAnonymousBlockIds();
                    foreach (ObjectId idAnonymBtr in anonyms)
                    {
                        var anonymBtr = idAnonymBtr.GetObject(OpenMode.ForRead) as BlockTableRecord;
                        var refsAnonym = anonymBtr.GetBlockReferenceIds(true, false);
                        foreach (ObjectId idBlRefAnonym in refsAnonym)
                        {
                            var blRefAnonym = idBlRefAnonym.GetObject( OpenMode.ForWrite, false, true) as BlockReference;
                            blRefAnonym.TransformBy(MatChangeBasePoint);
                        }
                    }
                }
                t.Commit();
            }
            IsChangeBasePoint = true;
        }

        public void ChangeBasePointInRedefineBase (Database dbExt, ObjectId idBtr)
        {
            var btr = idBtr.GetObject( OpenMode.ForRead) as BlockTableRecord;
            var idsBlRefs =btr.GetBlockReferenceIds(true, false);
            foreach (ObjectId idBlRef in idsBlRefs)
            {
                var blRef = idBlRef.GetObject( OpenMode.ForWrite) as BlockReference;
                if (blRef == null) continue;

                blRef.TransformBy(MatChangeBasePoint);
            }
            if (btr.IsDynamicBlock && !btr.IsAnonymous)
            {
                var idsBtrAnonym =btr.GetAnonymousBlockIds();
                foreach (ObjectId idBtrAnonym in idsBtrAnonym)
                {                    
                    ChangeBasePointInRedefineBase(dbExt, idBtrAnonym);
                }
            }
        }
    }
}
