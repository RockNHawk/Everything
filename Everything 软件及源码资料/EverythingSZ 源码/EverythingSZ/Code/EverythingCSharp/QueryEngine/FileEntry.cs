namespace UsnOperation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class FileEntry
    {
        public string FileName { get; set; }

        public bool IsFolder { get; set; }

        public FileEntry Parent { get; set; }

        //public string Path { get; set; }

        /// <summary>
        /// 此实体常驻内存，
        /// 在内存中存儲大量 Path 路徑很耗内存，每次重新計算 Path 可節省内存（1百万文件约 250M 内存）
        /// 
        /// 另外 Path 应延迟加载，不要启动时就计算，因为几十万个 Path 的计算很耗时间（1百万文件约 2s）
        /// </summary>
        public string Path
        {
            get
            {
                return GetFileFullPath(this);
            }
        }

        private static string GetFileFullPath(FileEntry f)
        {
            string path = f.FileName;
            var parent = f.Parent;
            while (parent != null)
            {
                path = parent.FileName + "\\" + path;
                parent = parent.Parent;
            }
            return path;
        }


        public FileEntry()
        {

        }
    }
}
