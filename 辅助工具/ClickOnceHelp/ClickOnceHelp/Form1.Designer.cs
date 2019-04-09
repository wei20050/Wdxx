namespace ClickOnceHelp
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.TxtUrl = new System.Windows.Forms.TextBox();
            this.BtnUrlSave = new System.Windows.Forms.Button();
            this.LabUrl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TxtUrl
            // 
            this.TxtUrl.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtUrl.Location = new System.Drawing.Point(16, 32);
            this.TxtUrl.Name = "TxtUrl";
            this.TxtUrl.Size = new System.Drawing.Size(576, 31);
            this.TxtUrl.TabIndex = 2;
            this.TxtUrl.Text = "http://localhost";
            // 
            // BtnUrlSave
            // 
            this.BtnUrlSave.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnUrlSave.Location = new System.Drawing.Point(16, 69);
            this.BtnUrlSave.Name = "BtnUrlSave";
            this.BtnUrlSave.Size = new System.Drawing.Size(146, 51);
            this.BtnUrlSave.TabIndex = 3;
            this.BtnUrlSave.Text = "写入安装地址";
            this.BtnUrlSave.UseVisualStyleBackColor = true;
            this.BtnUrlSave.Click += new System.EventHandler(this.BtnUrlSave_Click);
            // 
            // LabUrl
            // 
            this.LabUrl.AutoSize = true;
            this.LabUrl.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LabUrl.Location = new System.Drawing.Point(12, 10);
            this.LabUrl.Name = "LabUrl";
            this.LabUrl.Size = new System.Drawing.Size(190, 19);
            this.LabUrl.TabIndex = 4;
            this.LabUrl.Text = "客户端安装网站地址:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label1.Location = new System.Drawing.Point(221, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(332, 57);
            this.label1.TabIndex = 5;
            this.label1.Text = "本工具用于把客户端安装地址写入到\r\n Setup.exe 运行程序中 \r\n写入的地址就是客户端安装网站的地址";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(602, 135);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LabUrl);
            this.Controls.Add(this.BtnUrlSave);
            this.Controls.Add(this.TxtUrl);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox TxtUrl;
        private System.Windows.Forms.Button BtnUrlSave;
        private System.Windows.Forms.Label LabUrl;
        private System.Windows.Forms.Label label1;
    }
}

