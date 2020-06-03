namespace Test.Client.Form
{
    partial class FormMain
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
            this.btnCs = new System.Windows.Forms.Button();
            this.btnQk = new System.Windows.Forms.Button();
            this.btnGetTest = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.txtXxk = new System.Windows.Forms.TextBox();
            this.btnGetTime = new System.Windows.Forms.Button();
            this.btnPutUser = new System.Windows.Forms.Button();
            this.btnPostUser2 = new System.Windows.Forms.Button();
            this.btnPostUser = new System.Windows.Forms.Button();
            this.btnPutUser2 = new System.Windows.Forms.Button();
            this.btnGetUser1 = new System.Windows.Forms.Button();
            this.GetUser = new System.Windows.Forms.Button();
            this.btnDeleteUser = new System.Windows.Forms.Button();
            this.dgvUsers = new System.Windows.Forms.DataGridView();
            this.txtId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCs
            // 
            this.btnCs.BackColor = System.Drawing.Color.Green;
            this.btnCs.ForeColor = System.Drawing.Color.White;
            this.btnCs.Location = new System.Drawing.Point(427, 14);
            this.btnCs.Name = "btnCs";
            this.btnCs.Size = new System.Drawing.Size(75, 27);
            this.btnCs.TabIndex = 1;
            this.btnCs.Text = "测试连接";
            this.btnCs.UseVisualStyleBackColor = false;
            this.btnCs.Click += new System.EventHandler(this.btnCs_Click);
            // 
            // btnQk
            // 
            this.btnQk.Location = new System.Drawing.Point(508, 14);
            this.btnQk.Name = "btnQk";
            this.btnQk.Size = new System.Drawing.Size(280, 63);
            this.btnQk.TabIndex = 2;
            this.btnQk.Text = "清空消息框";
            this.btnQk.UseVisualStyleBackColor = true;
            this.btnQk.Click += new System.EventHandler(this.btnQk_Click);
            // 
            // btnGetTest
            // 
            this.btnGetTest.Location = new System.Drawing.Point(19, 45);
            this.btnGetTest.Name = "btnGetTest";
            this.btnGetTest.Size = new System.Drawing.Size(154, 32);
            this.btnGetTest.TabIndex = 3;
            this.btnGetTest.Text = "无参返回字符串(Test)";
            this.btnGetTest.UseVisualStyleBackColor = true;
            this.btnGetTest.Click += new System.EventHandler(this.btnGetTest_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "服务地址:";
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(82, 18);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(339, 21);
            this.txtUrl.TabIndex = 5;
            // 
            // txtXxk
            // 
            this.txtXxk.Location = new System.Drawing.Point(508, 83);
            this.txtXxk.Multiline = true;
            this.txtXxk.Name = "txtXxk";
            this.txtXxk.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtXxk.Size = new System.Drawing.Size(280, 405);
            this.txtXxk.TabIndex = 6;
            // 
            // btnGetTime
            // 
            this.btnGetTime.Location = new System.Drawing.Point(183, 45);
            this.btnGetTime.Name = "btnGetTime";
            this.btnGetTime.Size = new System.Drawing.Size(154, 32);
            this.btnGetTime.TabIndex = 8;
            this.btnGetTime.Text = "无参返回时间(Time)";
            this.btnGetTime.UseVisualStyleBackColor = true;
            this.btnGetTime.Click += new System.EventHandler(this.btnGetTime_Click);
            // 
            // btnPutUser
            // 
            this.btnPutUser.Location = new System.Drawing.Point(347, 83);
            this.btnPutUser.Name = "btnPutUser";
            this.btnPutUser.Size = new System.Drawing.Size(154, 32);
            this.btnPutUser.TabIndex = 11;
            this.btnPutUser.Text = "修改id:1 name:张修改";
            this.btnPutUser.UseVisualStyleBackColor = true;
            this.btnPutUser.Click += new System.EventHandler(this.btnPutUser_Click);
            // 
            // btnPostUser2
            // 
            this.btnPostUser2.Location = new System.Drawing.Point(183, 83);
            this.btnPostUser2.Name = "btnPostUser2";
            this.btnPostUser2.Size = new System.Drawing.Size(154, 32);
            this.btnPostUser2.TabIndex = 10;
            this.btnPostUser2.Text = "新增id:随机 name:李四";
            this.btnPostUser2.UseVisualStyleBackColor = true;
            this.btnPostUser2.Click += new System.EventHandler(this.btnPostUser2_Click);
            // 
            // btnPostUser
            // 
            this.btnPostUser.Location = new System.Drawing.Point(19, 83);
            this.btnPostUser.Name = "btnPostUser";
            this.btnPostUser.Size = new System.Drawing.Size(154, 32);
            this.btnPostUser.TabIndex = 9;
            this.btnPostUser.Text = "新增id:1 name:张三";
            this.btnPostUser.UseVisualStyleBackColor = true;
            this.btnPostUser.Click += new System.EventHandler(this.btnPostUser_Click);
            // 
            // btnPutUser2
            // 
            this.btnPutUser2.Location = new System.Drawing.Point(347, 121);
            this.btnPutUser2.Name = "btnPutUser2";
            this.btnPutUser2.Size = new System.Drawing.Size(154, 32);
            this.btnPutUser2.TabIndex = 14;
            this.btnPutUser2.Text = "根据id修改name:李修改";
            this.btnPutUser2.UseVisualStyleBackColor = true;
            this.btnPutUser2.Click += new System.EventHandler(this.btnPutUser2_Click);
            // 
            // btnGetUser1
            // 
            this.btnGetUser1.Location = new System.Drawing.Point(101, 121);
            this.btnGetUser1.Name = "btnGetUser1";
            this.btnGetUser1.Size = new System.Drawing.Size(72, 32);
            this.btnGetUser1.TabIndex = 12;
            this.btnGetUser1.Text = "查询id:1";
            this.btnGetUser1.UseVisualStyleBackColor = true;
            this.btnGetUser1.Click += new System.EventHandler(this.btnGetUser1_Click);
            // 
            // GetUser
            // 
            this.GetUser.Location = new System.Drawing.Point(347, 45);
            this.GetUser.Name = "GetUser";
            this.GetUser.Size = new System.Drawing.Size(154, 32);
            this.GetUser.TabIndex = 17;
            this.GetUser.Text = "查询所有人";
            this.GetUser.UseVisualStyleBackColor = true;
            this.GetUser.Click += new System.EventHandler(this.GetUser_Click);
            // 
            // btnDeleteUser
            // 
            this.btnDeleteUser.Location = new System.Drawing.Point(19, 121);
            this.btnDeleteUser.Name = "btnDeleteUser";
            this.btnDeleteUser.Size = new System.Drawing.Size(72, 32);
            this.btnDeleteUser.TabIndex = 15;
            this.btnDeleteUser.Text = "删除id:1";
            this.btnDeleteUser.UseVisualStyleBackColor = true;
            this.btnDeleteUser.Click += new System.EventHandler(this.btnDeleteUser_Click);
            // 
            // dgvUsers
            // 
            this.dgvUsers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUsers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsers.Location = new System.Drawing.Point(20, 159);
            this.dgvUsers.Name = "dgvUsers";
            this.dgvUsers.RowTemplate.Height = 23;
            this.dgvUsers.Size = new System.Drawing.Size(482, 329);
            this.dgvUsers.TabIndex = 18;
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(212, 128);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(125, 21);
            this.txtId.TabIndex = 20;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(183, 132);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 19;
            this.label2.Text = "ID:";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.txtId);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dgvUsers);
            this.Controls.Add(this.GetUser);
            this.Controls.Add(this.btnDeleteUser);
            this.Controls.Add(this.btnPutUser2);
            this.Controls.Add(this.btnGetUser1);
            this.Controls.Add(this.btnPutUser);
            this.Controls.Add(this.btnPostUser2);
            this.Controls.Add(this.btnPostUser);
            this.Controls.Add(this.btnGetTime);
            this.Controls.Add(this.txtXxk);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnGetTest);
            this.Controls.Add(this.btnQk);
            this.Controls.Add(this.btnCs);
            this.Name = "FormMain";
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnCs;
        private System.Windows.Forms.Button btnQk;
        private System.Windows.Forms.Button btnGetTest;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.TextBox txtXxk;
        private System.Windows.Forms.Button btnGetTime;
        private System.Windows.Forms.Button btnPutUser;
        private System.Windows.Forms.Button btnPostUser2;
        private System.Windows.Forms.Button btnPostUser;
        private System.Windows.Forms.Button btnPutUser2;
        private System.Windows.Forms.Button btnGetUser1;
        private System.Windows.Forms.Button GetUser;
        private System.Windows.Forms.Button btnDeleteUser;
        private System.Windows.Forms.DataGridView dgvUsers;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label label2;
    }
}

