using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;

namespace RedefineBlockInFolder
{
   public partial class FormSelectBlocks : Form
   {
      public List<ObjectId> SelectedBlocks
      {
         get
         {            
            return listBoxblocks.SelectedItems.Cast<KeyValuePair<ObjectId, string>>().Select(k=>k.Key).ToList();
         }
      }

      public FormSelectBlocks(Dictionary<ObjectId, string> blocks)
      {
         InitializeComponent();

         listBoxblocks.DataSource = new BindingSource(blocks, null);
         listBoxblocks.DisplayMember = "Value";
         listBoxblocks.ValueMember = "Key";
      }

      private void listBoxblocks_SelectedValueChanged(object sender, EventArgs e)
      {
         labelSelblocks.Text = $"Выбрано блоков: {listBoxblocks.SelectedItems.Count}";
      }
   }
}
