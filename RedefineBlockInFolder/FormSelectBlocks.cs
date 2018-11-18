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
using AcadLib;

namespace RedefineBlockInFolder
{
    using AcadLib.Strings;

    public partial class FormSelectBlocks : Form
    {
        public List<RedefineBlock> RenameBlocks { get; set; } = new List<RedefineBlock>();
        public List<RedefineBlock> SelectedBlocks
        {
            get
            {
                return listBoxblocks.SelectedItems.Cast<RedefineBlock>().ToList();
            }
        }

        private List<RedefineBlock> blocks;
        private BindingSource bindingDataSource;

        public FormSelectBlocks(List<RedefineBlock> blocks)
        {
            InitializeComponent();
            this.blocks = blocks;
            bindingDataSource = new BindingSource(blocks, null);
            listBoxblocks.DataSource = bindingDataSource;
            listBoxblocks.DisplayMember = "Name";            
        }

        private void UpdateDataBinding()
        {
            
        }

        private void listBoxblocks_SelectedValueChanged(object sender, EventArgs e)
        {
            labelSelblocks.Text = $"Выбрано блоков: {listBoxblocks.SelectedItems.Count}";
        }

        private void listBoxblocks_SelectedIndexChanged(object sender, EventArgs e)
        {
            var text = string.Empty;
            var bl = listBoxblocks.SelectedItem as RedefineBlock;
            if(bl != null)
            {
                text = bl.Name;
            }
            textBoxRename.Text = text;
        }

        private void buttonRename_Click(object sender, EventArgs e)
        {
            var bl = listBoxblocks.SelectedItem as RedefineBlock;
            if(bl == null)
            {
                MessageBox.Show("Не выбран блок!");
            }
            else
            {
                if (textBoxRename.Text.IsValidDbSymbolName())
                {                    
                    bl.Name = textBoxRename.Text;                                        
                    RenameBlocks.Add(bl);
                    bindingDataSource.ResetCurrentItem();
                }
                else
                {
                    MessageBox.Show("Недопустимое имя для блока!");
                }
            }
        }

        private void buttonChangeBasePoint_Click(object sender, EventArgs e)
        {
            var bl = listBoxblocks.SelectedItem as RedefineBlock;
            if (bl == null)
            {
                MessageBox.Show("Не выбран блок!");
            }
            else
            {
                try
                {
                    bl.ChangeBasePoint();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }                
            }
        }
    }
}
