namespace CardReading.Core
{
    partial class CardReadSetting
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
            this.DgvDllInfo = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BtnSave = new System.Windows.Forms.Button();
            this.LabMsg = new System.Windows.Forms.Label();
            this.LabPort = new System.Windows.Forms.Label();
            this.CmbPort = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.DgvDllInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // DgvDllInfo
            // 
            this.DgvDllInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvDllInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column8,
            this.Column3,
            this.Column9});
            this.DgvDllInfo.Location = new System.Drawing.Point(12, 12);
            this.DgvDllInfo.Name = "DgvDllInfo";
            this.DgvDllInfo.RowHeadersVisible = false;
            this.DgvDllInfo.RowTemplate.Height = 23;
            this.DgvDllInfo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DgvDllInfo.Size = new System.Drawing.Size(760, 266);
            this.DgvDllInfo.TabIndex = 0;
            this.DgvDllInfo.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvDllInfo_CellClick);
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "Name";
            this.Column1.HeaderText = "读卡器名称";
            this.Column1.Name = "Column1";
            this.Column1.Width = 255;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "Description";
            this.Column2.HeaderText = "读卡器说明";
            this.Column2.Name = "Column2";
            this.Column2.Width = 500;
            // 
            // Column8
            // 
            this.Column8.DataPropertyName = "Type";
            this.Column8.HeaderText = "Type";
            this.Column8.Name = "Column8";
            this.Column8.Visible = false;
            // 
            // Column3
            // 
            this.Column3.DataPropertyName = "IsComPortRequired";
            this.Column3.HeaderText = "是否需要com端口";
            this.Column3.Name = "Column3";
            this.Column3.Visible = false;
            // 
            // Column9
            // 
            this.Column9.DataPropertyName = "TypeId";
            this.Column9.HeaderText = "TypeId";
            this.Column9.Name = "Column9";
            this.Column9.Visible = false;
            // 
            // BtnSave
            // 
            this.BtnSave.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnSave.Location = new System.Drawing.Point(689, 287);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(83, 44);
            this.BtnSave.TabIndex = 1;
            this.BtnSave.Text = "保存";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // LabMsg
            // 
            this.LabMsg.AutoSize = true;
            this.LabMsg.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LabMsg.Location = new System.Drawing.Point(7, 295);
            this.LabMsg.Name = "LabMsg";
            this.LabMsg.Size = new System.Drawing.Size(93, 28);
            this.LabMsg.TabIndex = 2;
            this.LabMsg.Text = "已选择:  ";
            // 
            // LabPort
            // 
            this.LabPort.AutoSize = true;
            this.LabPort.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LabPort.Location = new System.Drawing.Point(484, 295);
            this.LabPort.Name = "LabPort";
            this.LabPort.Size = new System.Drawing.Size(72, 28);
            this.LabPort.TabIndex = 3;
            this.LabPort.Text = "端口:  ";
            // 
            // CmbPort
            // 
            this.CmbPort.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CmbPort.FormattingEnabled = true;
            this.CmbPort.Location = new System.Drawing.Point(562, 293);
            this.CmbPort.Name = "CmbPort";
            this.CmbPort.Size = new System.Drawing.Size(121, 35);
            this.CmbPort.TabIndex = 4;
            // 
            // CardReadSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 340);
            this.Controls.Add(this.CmbPort);
            this.Controls.Add(this.LabPort);
            this.Controls.Add(this.LabMsg);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.DgvDllInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CardReadSetting";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "读卡器设置";
            this.Load += new System.EventHandler(this.CardReadSetting_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DgvDllInfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView DgvDllInfo;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Label LabMsg;
        private System.Windows.Forms.Label LabPort;
        private System.Windows.Forms.ComboBox CmbPort;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
    }
}