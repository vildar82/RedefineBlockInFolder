namespace RedefineBlockInFolder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using AcadLib.Strings;

    public partial class FormSelectBlocks : Form
    {
        private readonly BindingSource _bindingDataSource;

        public FormSelectBlocks(List<RedefineBlock> blocks)
        {
            InitializeComponent();
            _bindingDataSource = new BindingSource(blocks, null);
            listBoxblocks.DataSource = _bindingDataSource;
            listBoxblocks.DisplayMember = "Name";
        }

        public List<RedefineBlock> RenameBlocks { get; set; } = new List<RedefineBlock>();
        public List<RedefineBlock> SelectedBlocks => listBoxblocks.SelectedItems.Cast<RedefineBlock>().ToList();

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
                    _bindingDataSource.ResetCurrentItem();
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
