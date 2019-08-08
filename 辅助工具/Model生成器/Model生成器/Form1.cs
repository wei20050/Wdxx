using System;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Model生成器.DAL;
using Model生成器.Utils;

namespace Model生成器
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //生成
        private void BtnCreate_Click(object sender, EventArgs e)
        {
            new Thread(delegate()
            {
                try
                {
                    var cm = ConfigurationManager.ConnectionStrings["DbContext"];
                    var dal = DalFactory.CreateDal(cm.ProviderName);
                    var tableList = dal.GetAllTables();
                    const string strNamespace = "Entity";

                    #region 操作控件
                    InvokeDelegate invokeDelegate = delegate
                    {
                        btnCreate.Enabled = false;
                        progressBar1.Visible = true;
                        progressBar1.Minimum = 0;
                        progressBar1.Maximum = tableList.Count;
                        progressBar1.Value = 0;
                    };
                    InvokeUtil.Invoke(this, invokeDelegate);
                    #endregion

                    var i = 0;
                    foreach (var table in tableList)
                    {
                        var sb = new StringBuilder();
                        //var sbExt = new StringBuilder();
                        var columnList = dal.GetAllColumns(table["table_name"]);

                        #region 原始Model
                        //sb.Append("using System;\r\n");
                        //sb.Append("using System.Collections.Generic;\r\n");
                        //sb.Append("using System.ComponentModel.DataAnnotations;");
                        //sb.Append("\r\n");
                        sb.Append("namespace " + strNamespace + "\r\n");
                        sb.Append("{\r\n");
                        sb.Append("    /// <summary>\r\n");
                        sb.Append("    /// " + table["comments"] + "\r\n");
                        sb.Append("    /// </summary>\r\n");
                        //sb.Append("    [Serializable]\r\n");
                        //sb.Append("    public partial class " + table["table_name"] + "\r\n");
                        sb.Append("    public class " + table["table_name"] + "\r\n");
                        sb.Append("    {\r\n");
                        foreach (var column in columnList)
                        {
                            var dataType = dal.ConvertDataType(column);

                            sb.Append("        /// <summary>\r\n");
                            sb.Append("        /// " + column["comments"] + "\r\n");
                            sb.Append("        /// </summary>\r\n");

                            if (column["constraint_type"] == "P")
                            {
                                sb.Append("        [System.Data.Objects.DataClasses.EdmScalarProperty(EntityKeyProperty = true, IsNullable = false)]\r\n");
                            }

                            //sb.Append("        [IsDBField]\r\n");
                            sb.Append("        public " + dataType + " " + column["columns_name"] + " { get; set; }\r\n");
                        }
                        sb.Append("    }\r\n");
                        sb.Append("}\r\n");
                        FileHelper.WriteFile(AppDomain.CurrentDomain.BaseDirectory + strNamespace, sb.ToString(), table["table_name"]);
                        #endregion

                        #region 扩展Model
                        //sbExt.Append("using System;\r\n");
                        //sbExt.Append("using System.Collections.Generic;\r\n");
                        //sbExt.Append("using System.Linq;\r\n");
                        //sbExt.Append("\r\n");
                        //sbExt.Append("namespace " + strNamespace + "\r\n");
                        //sbExt.Append("{\r\n");
                        //sbExt.Append("    /// <summary>\r\n");
                        //sbExt.Append("    /// " + table["comments"] + "\r\n");
                        //sbExt.Append("    /// </summary>\r\n");
                        //sbExt.Append("    public partial class " + table["table_name"] + "\r\n");
                        //sbExt.Append("    {\r\n");
                        //sbExt.Append("\r\n");
                        //sbExt.Append("    }\r\n");
                        //sbExt.Append("}\r\n");
                        //FileHelper.WriteFile(Application.StartupPath + "\\extmodels", sbExt.ToString(), table["table_name"]);
                        #endregion

                        #region 操作控件
                        invokeDelegate = delegate
                        {
                            progressBar1.Value = ++i;
                        };
                        InvokeUtil.Invoke(this, invokeDelegate);
                        #endregion
                    }

                    #region 操作控件
                    invokeDelegate = delegate
                    {
                        btnCreate.Enabled = true;
                        progressBar1.Visible = false;
                        progressBar1.Value = 0;
                    };
                    InvokeUtil.Invoke(this, invokeDelegate);
                    #endregion

                    MessageBox.Show(@"生成完成");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }).Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text += ConfigurationManager.AppSettings["DBType"];
        }
    }
}
