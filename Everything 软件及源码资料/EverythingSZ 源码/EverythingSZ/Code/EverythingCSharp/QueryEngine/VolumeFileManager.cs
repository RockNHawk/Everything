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


        List<FileEntry> allFiles;

        public void Init(DriveInfo drive)
        {
            // http://irix.me/2016/04/16/NTFS-USN/


            int cap = 10000;

            // 第一次先列出为平行字典
            var dic = GetFilesDic(drive, cap);
            var files = new List<FileEntry>(dic.Count);

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
                    files.Add(file);
                }
            }

            for (int i = 0; i < files.Count; i++)
            {
                var f = files[i];

                string path = f.FileName;
                var parent = f.Parent;
                while (parent != null)
                {
                    path = parent.FileName + "\\" + path;
                    parent = parent.Parent;
                }
                f.Path = path;
            }

            this.allFiles = files;

            Console.WriteLine(dic);



        }

        /// <summary>
        /// 列出为平行字典
        /// </summary>
        /// <param name="drive"></param>
        /// <param name="cap"></param>
        /// <returns></returns>
        private static Dictionary<ulong, Tuple<UsnEntry, FileEntry>> GetFilesDic(DriveInfo drive, int cap)
        {
            using (var usnOperator = new UsnOperator(drive))
            {
                var dic = new Dictionary<ulong, Tuple<UsnEntry, FileEntry>>(cap);
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
                    }
                }

                return dic;
            }
        }

    }
}
