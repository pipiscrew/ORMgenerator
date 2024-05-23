namespace ORMgenerator
{
    partial class frmTables
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
            this.lst = new System.Windows.Forms.CheckedListBox();
            this.btnAll = new System.Windows.Forms.Button();
            this.btnUnAll = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.groupExport = new System.Windows.Forms.GroupBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.optDapper = new System.Windows.Forms.RadioButton();
            this.optNative = new System.Windows.Forms.RadioButton();
            this.chkWinForms = new System.Windows.Forms.CheckBox();
            this.groupExport.SuspendLayout();
            this.SuspendLayout();
            // 
            // lst
            // 
            this.lst.CheckOnClick = true;
            this.lst.Dock = System.Windows.Forms.DockStyle.Left;
            this.lst.FormattingEnabled = true;
            this.lst.Location = new System.Drawing.Point(5, 5);
            this.lst.Name = "lst";
            this.lst.Size = new System.Drawing.Size(324, 396);
            this.lst.TabIndex = 0;
            // 
            // btnAll
            // 
            this.btnAll.Location = new System.Drawing.Point(335, 5);
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(31, 25);
            this.btnAll.TabIndex = 1;
            this.btnAll.Text = "+";
            this.btnAll.UseVisualStyleBackColor = true;
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // btnUnAll
            // 
            this.btnUnAll.Location = new System.Drawing.Point(335, 36);
            this.btnUnAll.Name = "btnUnAll";
            this.btnUnAll.Size = new System.Drawing.Size(31, 25);
            this.btnUnAll.TabIndex = 2;
            this.btnUnAll.Text = "-";
            this.btnUnAll.UseVisualStyleBackColor = true;
            this.btnUnAll.Click += new System.EventHandler(this.btnUnAll_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(349, 365);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(174, 36);
            this.btnNext.TabIndex = 4;
            this.btnNext.Text = ">> generate >>";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // groupExport
            // 
            this.groupExport.Controls.Add(this.radioButton1);
            this.groupExport.Controls.Add(this.optDapper);
            this.groupExport.Controls.Add(this.optNative);
            this.groupExport.Location = new System.Drawing.Point(335, 81);
            this.groupExport.Name = "groupExport";
            this.groupExport.Size = new System.Drawing.Size(188, 206);
            this.groupExport.TabIndex = 5;
            this.groupExport.TabStop = false;
            this.groupExport.Text = "export method :";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Enabled = false;
            this.radioButton1.Location = new System.Drawing.Point(14, 59);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(174, 19);
            this.radioButton1.TabIndex = 2;
            this.radioButton1.Tag = "2";
            this.radioButton1.Text = "DBASEWrapper Parameters";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // optDapper
            // 
            this.optDapper.AutoSize = true;
            this.optDapper.Location = new System.Drawing.Point(14, 84);
            this.optDapper.Name = "optDapper";
            this.optDapper.Size = new System.Drawing.Size(65, 19);
            this.optDapper.TabIndex = 1;
            this.optDapper.Tag = "3";
            this.optDapper.Text = "Dapper";
            this.optDapper.UseVisualStyleBackColor = true;
            // 
            // optNative
            // 
            this.optNative.AutoSize = true;
            this.optNative.Checked = true;
            this.optNative.Location = new System.Drawing.Point(14, 34);
            this.optNative.Name = "optNative";
            this.optNative.Size = new System.Drawing.Size(164, 19);
            this.optNative.TabIndex = 0;
            this.optNative.TabStop = true;
            this.optNative.Tag = "1";
            this.optNative.Text = "DBASEWrapper Reflection";
            this.optNative.UseVisualStyleBackColor = true;
            // 
            // chkWinForms
            // 
            this.chkWinForms.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkWinForms.AutoSize = true;
            this.chkWinForms.Location = new System.Drawing.Point(381, 322);
            this.chkWinForms.Name = "chkWinForms";
            this.chkWinForms.Size = new System.Drawing.Size(142, 25);
            this.chkWinForms.TabIndex = 6;
            this.chkWinForms.Text = "export winforms flavor";
            this.chkWinForms.UseVisualStyleBackColor = true;
            // 
            // frmTables
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 406);
            this.Controls.Add(this.chkWinForms);
            this.Controls.Add(this.groupExport);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnUnAll);
            this.Controls.Add(this.btnAll);
            this.Controls.Add(this.lst);
            this.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmTables";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmTables";
            this.Load += new System.EventHandler(this.frmTables_Load);
            this.groupExport.ResumeLayout(false);
            this.groupExport.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox lst;
        private System.Windows.Forms.Button btnAll;
        private System.Windows.Forms.Button btnUnAll;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.GroupBox groupExport;
        private System.Windows.Forms.RadioButton optNative;
        private System.Windows.Forms.RadioButton optDapper;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.CheckBox chkWinForms;
    }
}