using System;
using System.Collections.Generic;
using NetFrameWork.Database;
using System.Windows;

namespace Test.Db
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
            //var ret = _db.Update( new user { id = 1,name = ""});
            //MessageBox.Show(ret.ToString());
            //var ret = _db.FindListBySql<data>("select * from user join userage on user.aid = userage.id ");
            //var s = 12345;
            var userage = new userage
            {
                id = Guid.NewGuid().ToString(),
                age = 20
            };
            var user = new user
            {
                id= Guid.NewGuid().ToString(),
                name = "张三",
                sex = "男",
                aid = userage.id
            };
            //var i1 = _db.Insert(userage);
            //var i2 = _db.Insert(user);
            //var b1 = _db.Delete<userage>(p=>p.id== "1d7dbc46-a013-4a80-b568-d59bb8762892");
            //var b2 = _db.Delete<user>(p => p.id == "13d66624-d96c-4f46-8d98-59fba59933ca");
            var s = _db.SelectAll<user>(p=>p.id.EndsWith("9"));
        }

    }

    public class data
    {
        public user user { get; set; }
        public userage userage { get; set; }
    }
    public class user
    {
        public string id { get; set; }
        public string name { get; set; }
        public string aid { get; set; }
        public string sex { get; set; }
    }

    public class userage
    {
        public string id { get; set; }
        public int age { get; set; }
    }
}
