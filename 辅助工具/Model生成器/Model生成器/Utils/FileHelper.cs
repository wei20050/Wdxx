using System.IO;

namespace Model生成器.Utils
{
    public class FileHelper
    {
        #region 写文件
        /// <summary>
        /// 写文件
        /// </summary>
        public static void WriteFile(string path, string str, string tableName)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var filePath = path + "\\" + tableName + ".cs";

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(str);
                    sw.Flush();
                }
            }
        }
        #endregion

    }
}
