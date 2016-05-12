using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace RedefineBlockInFolder
{
    public class RedefineBlock
    {
        public ObjectId IdBtr { get; set; }
        public string Name { get; set; }   
        public string OldName { get; set; }     

        public RedefineBlock(BlockTableRecord btr)
        {
            IdBtr = btr.Id;
            Name = btr.Name;
            OldName = Name;
        }
    }
}
