﻿using System;
using System.IO;
using System.Threading;

namespace SqliteChangePwd
{
    public class YxLog
    {
        //多线程锁防止写文件或者创建目录与文件冲突
        private static readonly object Lock = new object();
        //日志文件夹默认根目录logs文件夹
        private static readonly string FilePath = $@"{AppDomain.CurrentDomain.BaseDirectory}logs\";
        //日志分隔文件大小 6M
        private const int FileSize = 6 * 1024 * 1024; 

        /// <summary>
        /// 写错误日志
        /// </summary>
        public static void Error(object log)
        {
            new Thread(() =>
            {
                lock (Lock)
                {
                    WriteFile($@"[Error] {log}", CreateLogPath());
                }
            }).Start();
        }

        /// <summary>
        /// 写操作日志
        /// </summary>
        public static void Info(object log)
        {
            new Thread(() =>
            {
                lock (Lock)
                {
                    WriteFile($@"[Info] {log}", CreateLogPath());
                }
            }).Start();
        }

        /// <summary>
        /// 生成日志文件路径
        /// </summary>
        private static string CreateLogPath()
        {
            var index = 0;
            string logPath;
            var bl = true;
            do
            {
                index++;
                logPath = $@"{FilePath}{DateTime.Now:yyyy-MM-dd}_{index}.log";
                if (File.Exists(logPath))
                {
                    var fileInfo = new FileInfo(logPath);
                    if (fileInfo.Length < FileSize)
                    {
                        bl = false;
                    }
                }
                else
                {
                    bl = false;
                }
            } while (bl);
            return logPath;
        }

        /// <summary>
        /// 写文件
        /// </summary>
        private static void WriteFile(string txt, string logPath)
        {
            lock (Lock)
            {
                if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(FilePath) ?? throw new InvalidOperationException());
                }
            }

            if (!File.Exists(logPath))
            {
                using (var fs = new FileStream(logPath, FileMode.Create))
                {
                    fs.Close();
                }
            }

            using (var fs = new FileStream(logPath, FileMode.Append, FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs))
                {
                    #region 日志内容

                    var value = $@"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {txt}";

                    #endregion

                    sw.WriteLine(value);
                    sw.Flush();
                }
                fs.Close();
            }
        }
    }
}