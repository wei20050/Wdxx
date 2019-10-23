using System;
using System.IO;
using System.Threading;
// ReSharper disable UnusedMember.Global

namespace NetFrameWork.Core
{

    /// <summary>
    /// 文件处理核心
    /// </summary>
    public class CoreFile
    {

        /// <summary>
        /// 检测文件是否可以读取
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static bool IsFileReady(string filePath)
        {
            try
            {
                using (var inputStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return inputStream.Length > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 文件监控对象
        /// </summary>
        private FileSystemWatcher Watcher { get; set; }

        /// <summary>
        /// 获取到文件路径数据事件触发
        /// </summary>
        protected virtual void OnFilePath(string filePath)
        {
            FilePathReceived?.Invoke(filePath);
        }

        /// <summary>
        /// 新增文件事件触发
        /// </summary>
        /// <param name="source"></param>
        /// <param name="fse"></param>
        private void OnCreated(object source, FileSystemEventArgs fse)
        {
            try
            {
                var count = 0;
                while (!IsFileReady(fse.FullPath))
                {
                    Thread.Sleep(10);
                    if (count++ > 100)
                    {
                        return;
                    }
                }
                OnFilePath(fse.FullPath);
            }
            catch (Exception e)
            {
                throw new Exception("CoreFile.OnCreated Err", e);
            }
        }

        /// <summary>
        /// 封装的委托
        /// </summary>
        /// <param name="filePath"></param>
        public delegate void DelegateFilePathReceived(string filePath);

        /// <summary>
        /// 新的的数据返回事件
        /// </summary>
        public event DelegateFilePathReceived FilePathReceived;

        /// <summary>
        /// 开启文件监控
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="filter">文件后缀</param>
        public void StartWatcher(string path, string filter)
        {
            Watcher = new FileSystemWatcher
            {
                Path = path,
                NotifyFilter = NotifyFilters.FileName,
                Filter = "*." + filter
            };
            // 添加事件处理器
            Watcher.Created += OnCreated;
            // 开始监控。
            Watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 停止文件监控
        /// </summary>
        public void StopWatcher()
        {
            Watcher.Dispose();
        }

    }
}
