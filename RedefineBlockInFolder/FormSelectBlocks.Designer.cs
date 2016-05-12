namespace RedefineBlockInFolder
{
   partial class FormSelectBlocks
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
            this.components = new System.ComponentModel.Container();
            this.listBoxblocks = new System.Windows.Forms.ListBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.labelSelblocks = new System.Windows.Forms.Label();
            this.textBoxRename = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.buttonRename = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxblocks
            // 
            this.listBoxblocks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxblocks.FormattingEnabled = true;
            this.listBoxblocks.Location = new System.Drawing.Point(12, 12);
            this.listBoxblocks.Name = "listBoxblocks";
            this.listBoxblocks.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxblocks.Size = new System.Drawing.Size(376, 316);
            this.listBoxblocks.TabIndex = 0;
            this.listBoxblocks.SelectedIndexChanged += new System.EventHandler(this.listBoxblocks_SelectedIndexChanged);
            this.listBoxblocks.SelectedValueChanged += new System.EventHandler(this.listBoxblocks_SelectedValueChanged);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(315, 376);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(234, 376);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // labelSelblocks
            // 
            this.labelSelblocks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelSelblocks.AutoSize = true;
            this.labelSelblocks.Location = new System.Drawing.Point(12, 389);
            this.labelSelblocks.Name = "labelSelblocks";
            this.labelSelblocks.Size = new System.Drawing.Size(103, 13);
            this.labelSelblocks.TabIndex = 2;
            this.labelSelblocks.Text = "Выбрано блоков: 0";
            // 
            // textBoxRename
            // 
            this.textBoxRename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRename.Location = new System.Drawing.Point(15, 347);
            this.textBoxRename.Name = "textBoxRename";
            this.textBoxRename.Size = new System.Drawing.Size(178, 20);
            this.textBoxRename.TabIndex = 3;
            this.toolTip1.SetToolTip(this.textBoxRename, "Переименование блока в этом файле и во всех файлах в выбранной папке. Переименова" +
        "нные блоки не обязательно выбирать для переопределения.");
            // 
            // buttonRename
            // 
            this.buttonRename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRename.Location = new System.Drawing.Point(199, 347);
            this.buttonRename.Name = "buttonRename";
            this.buttonRename.Size = new System.Drawing.Size(98, 20);
            this.buttonRename.TabIndex = 4;
            this.buttonRename.Text = "Переименовать";
            this.toolTip1.SetToolTip(this.buttonRename, "Переименование блока в этом файле и во всех файлах в выбранной папке. Переименова" +
        "нные блоки не обязательно выбирать для переопределения.");
            this.buttonRename.UseVisualStyleBackColor = true;
            this.buttonRename.Click += new System.EventHandler(this.buttonRename_Click);
            // 
            // FormSelectBlocks
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(400, 412);
            this.Controls.Add(this.buttonRename);
            this.Controls.Add(this.textBoxRename);
            this.Controls.Add(this.labelSelblocks);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.listBoxblocks);
            this.Name = "FormSelectBlocks";
            this.Text = "Выбор блоков для переопределения";
            this.ResumeLayout(false);
            this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.ListBox listBoxblocks;
      private System.Windows.Forms.Button buttonCancel;
      private System.Windows.Forms.Button buttonOk;
      private System.Windows.Forms.Label labelSelblocks;
        private System.Windows.Forms.TextBox textBoxRename;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button buttonRename;
    }
}