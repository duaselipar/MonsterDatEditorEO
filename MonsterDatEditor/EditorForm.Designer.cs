using System.Drawing.Printing;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace MonsterDatEditor
{
    partial class EditorForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label lblStatus;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorForm));
            dataGridView1 = new DataGridView();
            btnLoad = new Button();
            lblStatus = new Label();
            txtSearch = new TextBox();
            btnSearch = new Button();
            btnSave = new Button();
            btnExport = new Button();
            btnImport = new Button();
            cmbSearch = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(8, 53);
            dataGridView1.Margin = new Padding(2);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(969, 339);
            dataGridView1.TabIndex = 0;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(8, 11);
            btnLoad.Margin = new Padding(2);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(116, 35);
            btnLoad.TabIndex = 1;
            btnLoad.Text = "Load DAT";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // lblStatus
            // 
            lblStatus.Location = new Point(8, 394);
            lblStatus.Margin = new Padding(2, 0, 2, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(451, 24);
            lblStatus.TabIndex = 6;
            lblStatus.Text = "Ready";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(266, 18);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(253, 23);
            txtSearch.TabIndex = 7;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(525, 18);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(75, 23);
            btnSearch.TabIndex = 8;
            btnSearch.Text = "Search";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(861, 11);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(116, 35);
            btnSave.TabIndex = 9;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnExport
            // 
            btnExport.Location = new Point(739, 11);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(116, 35);
            btnExport.TabIndex = 10;
            btnExport.Text = "Export";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += btnExport_Click;
            // 
            // btnImport
            // 
            btnImport.Location = new Point(617, 9);
            btnImport.Name = "btnImport";
            btnImport.Size = new Size(116, 37);
            btnImport.TabIndex = 11;
            btnImport.Text = "Import";
            btnImport.UseVisualStyleBackColor = true;
            btnImport.Click += btnImport_Click;
            // 
            // cmbSearch
            // 
            cmbSearch.FormattingEnabled = true;
            cmbSearch.Location = new Point(139, 18);
            cmbSearch.Name = "cmbSearch";
            cmbSearch.Size = new Size(121, 23);
            cmbSearch.TabIndex = 12;
            // 
            // EditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(989, 419);
            Controls.Add(cmbSearch);
            Controls.Add(btnImport);
            Controls.Add(btnExport);
            Controls.Add(btnSave);
            Controls.Add(btnSearch);
            Controls.Add(txtSearch);
            Controls.Add(lblStatus);
            Controls.Add(btnLoad);
            Controls.Add(dataGridView1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            MaximizeBox = false;
            Name = "EditorForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Monster.dat Editor by DuaSelipar";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();

            // ** COLUMN SETUP WAJIB BUAT SELEPAS InitializeComponent() **
            // (letak kat Form1.cs constructor atau method sendiri)
        }

        #endregion

        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnSave;
        private Button btnExport;
        private Button btnImport;
        private ComboBox cmbSearch;
    }
}
