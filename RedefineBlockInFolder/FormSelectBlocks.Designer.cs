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
         this.listBoxblocks = new System.Windows.Forms.ListBox();
         this.buttonCancel = new System.Windows.Forms.Button();
         this.buttonOk = new System.Windows.Forms.Button();
         this.labelSelblocks = new System.Windows.Forms.Label();
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
         this.listBoxblocks.Size = new System.Drawing.Size(445, 355);
         this.listBoxblocks.TabIndex = 0;
         this.listBoxblocks.SelectedValueChanged += new System.EventHandler(this.listBoxblocks_SelectedValueChanged);
         // 
         // buttonCancel
         // 
         this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.buttonCancel.Location = new System.Drawing.Point(382, 376);
         this.buttonCancel.Name = "buttonCancel";
         this.buttonCancel.Size = new System.Drawing.Size(75, 23);
         this.buttonCancel.TabIndex = 1;
         this.buttonCancel.Text = "Отмена";
         this.buttonCancel.UseVisualStyleBackColor = true;
         // 
         // buttonOk
         // 
         this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.buttonOk.Location = new System.Drawing.Point(301, 376);
         this.buttonOk.Name = "buttonOk";
         this.buttonOk.Size = new System.Drawing.Size(75, 23);
         this.buttonOk.TabIndex = 1;
         this.buttonOk.Text = "OK";
         this.buttonOk.UseVisualStyleBackColor = true;
         // 
         // labelSelblocks
         // 
         this.labelSelblocks.AutoSize = true;
         this.labelSelblocks.Location = new System.Drawing.Point(12, 389);
         this.labelSelblocks.Name = "labelSelblocks";
         this.labelSelblocks.Size = new System.Drawing.Size(103, 13);
         this.labelSelblocks.TabIndex = 2;
         this.labelSelblocks.Text = "Выбрано блоков: 0";
         // 
         // FormSelectBlocks
         // 
         this.AcceptButton = this.buttonOk;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.buttonCancel;
         this.ClientSize = new System.Drawing.Size(469, 411);
         this.Controls.Add(this.labelSelblocks);
         this.Controls.Add(this.buttonOk);
         this.Controls.Add(this.buttonCancel);
         this.Controls.Add(this.listBoxblocks);
         this.Name = "FormSelectBlocks";
         this.Text = "FormSelectBlocks";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.ListBox listBoxblocks;
      private System.Windows.Forms.Button buttonCancel;
      private System.Windows.Forms.Button buttonOk;
      private System.Windows.Forms.Label labelSelblocks;
   }
}