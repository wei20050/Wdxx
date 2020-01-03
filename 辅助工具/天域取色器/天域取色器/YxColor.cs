using System.Drawing;

namespace 天域取色器
{
    public class YxColor
    {

        /// <summary>
        /// 取鼠标周围颜色
        /// </summary>
        /// <returns>颜色字符串</returns>
        public static string[,] GetColor(int x,int y,Bitmap bitmap)
        {
            try
            {
                var cArr = new string[15,15];
                var zx = x - 7;
                var zy = y - 7;
                for (var i = 0; i < 15; i++)
                {
                    for (var j = 0; j < 15; j++)
                    {
                        var color = bitmap.GetPixel(zx+i, zy+j);
                        cArr[i, j] = color.Name;
                    }
                }
                return cArr;
            }
            catch
            {
                return null;
            }
        }
    }
}
