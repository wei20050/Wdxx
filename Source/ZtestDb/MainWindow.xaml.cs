using System.Windows;
using NetFrameWork.Database;

namespace ZtestDb
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private readonly ORM _db = new ORM();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var s = _db.SelectAll<user>();
            var sName = string.Empty;
            foreach (var sn in s)
            {
                sName += $"id:{sn.id}  name:{sn.name},";
            }
            MessageBox.Show(sName);
        }

    }

    public class user
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}
