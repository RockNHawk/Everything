using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UsnOperation;

namespace QueryEngine
{
    public class FileTree
    {
        public UsnEntry USN { get; set; }
    }

    public class VolumeFileManager
    {
        //public VolumeFileManager Instance { get; set; } = new VolumeFileManager();

        internal int entryCount;
        internal int fileCount;
        internal int directoryCount;

        internal List<FileEntry> entries;
        internal List<FileEntry> files;
        //internal List<FileEntry> directories;

        public string VolumeName { get; internal set; }

        public void Init(DriveInfo drive)
        {
            this.VolumeName = drive.Name;

            // http://irix.me/2016/04/16/NTFS-USN/

            // 可以根據磁盤名字和大小估算文件總數，比如 如果是 c 盤，那麽就是 windows 系統的文件數量
            int cap = 1000000;

            int fileCount = 0, directoryCount = 0;

            // 第一次先列出为平行字典
            var dic = GetFilesDic(drive, cap, out fileCount, out directoryCount);

            var entries = new List<FileEntry>(dic.Count);

            foreach (var item in dic)
            {
                var usn = item.Value.Item1;
                var file = item.Value.Item2;
                var parentFileReferenceNumber = usn.ParentFileReferenceNumber;
                if (parentFileReferenceNumber == 0U)
                {

                }
                else if (parentFileReferenceNumber == 0x5000000000005U)
                {

                }
                else
                {
                    if (dic.TryGetValue(parentFileReferenceNumber, out var parent))
                    {
                        file.Parent = parent.Item2;
                    }
                    else
                    {
                    }
                    entries.Add(file);
                }
            }

            //var files = new List<FileEntry>(fileCount);
            //var directories = new List<FileEntry>(directoryCount);
            for (int i = 0; i < entries.Count; i++)
            {
                var f = entries[i];

                //f.Path = GetFileFullPath(f);
                //GetFileFullPath(f);
                //if (f.IsFolder)
                //{
                //    directories.Add(f);
                //}
                //else
                //{
                //    files.Add(f);
                //}
            }

            this.entryCount = entries.Count;
            this.fileCount = fileCount;
            this.directoryCount = directoryCount;

            //this.files = files;
            //this.directories = dic;
            this.entries = entries;

            dic.Clear();

        }


        /// <summary>
        /// 列出为平行字典
        /// </summary>
        /// <param name="drive"></param>
        /// <param name="cap"></param>
        /// <returns></returns>
        private static Dictionary<ulong, Tuple<UsnEntry, FileEntry>> GetFilesDic(DriveInfo drive, int cap, out int fileCount, out int directoryCount)
        {
            int fc = 0, dc = 0;
            using (var usnOperator = new UsnOperator(drive))
            {
                usnOperator.Prepare();
                var estimatedEntryCount = usnOperator.EstimatedEntryCount;
                var dic = new Dictionary<ulong, Tuple<UsnEntry, FileEntry>>(estimatedEntryCount);
                using (var itr = usnOperator.GetEntriesEnumerator())
                {
                    while (itr.MoveNext())
                    {
                        var entry = itr.Current;
                        dic[entry.FileReferenceNumber] = new Tuple<UsnEntry, FileEntry>(entry, new FileEntry
                        {
                            FileName = entry.FileName,
                            IsFolder = entry.IsFolder,
                        });
                        if (entry.IsFolder)
                        {
                            dc++;
                        }
                        else
                        {
                            fc++;
                        }
                    }
                    fileCount = fc;
                    directoryCount = dc;
                    return dic;
                }
            }
        }

    }
}
