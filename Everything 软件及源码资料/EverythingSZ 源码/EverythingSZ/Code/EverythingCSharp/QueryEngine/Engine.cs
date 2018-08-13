using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UsnOperation;

namespace QueryEngine
{
    public class Engine
    {
        /// <summary>
        /// When its values is 1407374883553285(0x5000000000005L), it means this file/folder is under drive root
        /// </summary>
        protected const UInt64 ROOT_FILE_REFERENCE_NUMBER = 0x5000000000005L;

        protected static readonly IEnumerable<string> excludeFolders =
            new string[]
            {
                "$RECYCLE.BIN",
                "System Volume Information",
                "$AttrDef",
                "$BadClus",
                "$BitMap",
                "$Boot",
                "$LogFile",
                "$Mft",
                "$MftMirr",
                "$Secure",
                "$TxfLog",
                "$UpCase",
                "$Volume",
                "$Extend"
            }.Select(e => e.ToUpper());

        public static IEnumerable<DriveInfo> GetAllFixedNtfsDrives()
        {
            //return DriveInfo.GetDrives()
            //    .Where(d => d.DriveType == DriveType.Fixed && d.DriveFormat.ToUpper() == "NTFS");
            var dirs = DriveInfo.GetDrives();
            //return dirs.Where(d => d.DriveType == DriveType.Removable && d.DriveFormat.ToUpper() == "NTFS");
            //return dirs.Where(d => d.IsReady&& d.Name.ToUpper().Contains(driver.ToUpper()) && d.DriveFormat.ToUpper() == "NTFS");
            return dirs.Where(d => d.IsReady && d.DriveFormat.ToUpper() == "NTFS");
        }

        public static List<FileAndDirectoryEntry> GetFilesAndDirectories(IEnumerable<DriveInfo> fixedNtfsDrives)
        {
            List<FileAndDirectoryEntry> result = new List<FileAndDirectoryEntry>();
            foreach (var drive in fixedNtfsDrives)
            {
                result.AddRange(GetFilesAndDirectories(drive));
            }
            return result;
        }

        public static List<FileAndDirectoryEntry> GetAllFilesAndDirectories(   )
        {
            var fixedNtfsDrive = GetAllFixedNtfsDrives();
            return GetFilesAndDirectories(fixedNtfsDrive);
        }

        public static List<FileAndDirectoryEntry> GetFilesAndDirectories(string driverName)
        {
            var fixedNtfsDrive = GetAllFixedNtfsDrives().FirstOrDefault(x => string.Compare(x.Name, driverName, StringComparison.OrdinalIgnoreCase) == 0);

            return GetFilesAndDirectories(fixedNtfsDrive);

            //Console.WriteLine(result.Count);
            //Console.WriteLine("==========");
            //for (int i = 0; i < result.Count; i++)
            //{
            //    Console.WriteLine((i) + "、" + result[i].FileName);
            //}
            //for (int i = 0; i < 10; i++)
            //{
            //    Console.WriteLine((i) + "、" + result[i].FileName);
            //}
            //Console.WriteLine("==========");
            //for (int i = 10; i > 0; i--)
            //{
            //    Console.WriteLine((result.Count - i) + "、" + result[result.Count - i].FileName);
            //}
            //return result;
        }

        public static List<FileAndDirectoryEntry> GetFilesAndDirectories(DriveInfo drive)
        {
            var usnOperator = new UsnOperator(drive);

            var usnEntries = new List<UsnEntry>();
            var folders = new List<UsnEntry>();
            using (var itr = usnOperator.GetEntriesEnumerator())
            {
                while (itr.MoveNext())
                {
                    var e = itr.Current;
                    if (excludeFolders.Contains(e.FileName.ToUpper()))
                    {
                        continue;
                    }
                    usnEntries.Add(e);
                    if (e.IsFolder)
                    {
                        folders.Add(e);
                    }
                }
            }

            //var usnEntries = usnOperator.GetEntriesEnumerator().Where(e => !excludeFolders.Contains(e.FileName.ToUpper()));
            //var folders = usnEntries.Where(e => e.IsFolder).ToArray();
            List<FrnFilePath> paths = GetFolderPath(folders, drive);

            return (usnEntries.Join(
                paths,
                usn => usn.ParentFileReferenceNumber,
                path => path.FileReferenceNumber,
                (usn, path) => new FileAndDirectoryEntry(usn, path.Path))).ToList();
        }

        private static List<FrnFilePath> GetFolderPath(IList<UsnEntry> folders, DriveInfo drive)
        {
            Dictionary<UInt64, FrnFilePath> pathDic = new Dictionary<ulong, FrnFilePath>();

            // 跟路径，即磁盘跟路径（C:,D:）
            pathDic.Add(ROOT_FILE_REFERENCE_NUMBER,
                new FrnFilePath(ROOT_FILE_REFERENCE_NUMBER, null, string.Empty, drive.Name));

            // 这里 FrnFilePath 仅仅是复制一下数据
            foreach (var folder in folders)
            {
                pathDic.Add(folder.FileReferenceNumber,
                    new FrnFilePath(folder.FileReferenceNumber, folder.ParentFileReferenceNumber, folder.FileName));
            }

            Stack<UInt64> treeWalkStack = new Stack<ulong>();

            foreach (var key in pathDic.Keys)
            {
                treeWalkStack.Clear();

                FrnFilePath currentValue = pathDic[key];

                if (string.IsNullOrWhiteSpace(currentValue.Path)
                    && currentValue.ParentFileReferenceNumber.HasValue

                    // 如果此文件夹的父级存在于列表中
                    && pathDic.ContainsKey(currentValue.ParentFileReferenceNumber.Value))
                {
                    FrnFilePath parentValue = pathDic[currentValue.ParentFileReferenceNumber.Value];

                    // 递归将父级文件夹加入到 treeWalkStack 中，方便下一步填充他们的路径
                    while (string.IsNullOrWhiteSpace(parentValue.Path)
                        && parentValue.ParentFileReferenceNumber.HasValue

                        // 如果父级文件夹的父级存在于列表中
                        && pathDic.ContainsKey(parentValue.ParentFileReferenceNumber.Value))
                    {
                        var temp = currentValue;
                        currentValue = parentValue;

                        if (currentValue.ParentFileReferenceNumber.HasValue
                            && pathDic.ContainsKey(currentValue.ParentFileReferenceNumber.Value))
                        {
                            treeWalkStack.Push(temp.FileReferenceNumber);
                            parentValue = pathDic[currentValue.ParentFileReferenceNumber.Value];
                        }
                        else
                        {
                            parentValue = null;
                            break;
                        }
                    }

                    if (parentValue != null)
                    {
                        currentValue.Path = BuildPath(currentValue, parentValue);

                        // 循环 treeWalkStack （不同意普通列表，treeWalkStack 是先进的后出，从最父级到子级）
                        // 这里可以优化，把 路径的获取放到前面的循环去，同时去除 treeWalkStack？
                        while (treeWalkStack.Count() > 0)
                        {
                            UInt64 walkedKey = treeWalkStack.Pop();

                            FrnFilePath walkedNode = pathDic[walkedKey];
                            FrnFilePath parentNode = pathDic[walkedNode.ParentFileReferenceNumber.Value];

                            // 这里的 Path 是一直累加的，先把父级的路径填充，然后把父级的子级填充，这样逐级填充
                            walkedNode.Path = BuildPath(walkedNode, parentNode);
                        }
                    }
                }
            }

            var result = pathDic.Values.Where(p => !string.IsNullOrWhiteSpace(p.Path) && p.Path.StartsWith(drive.Name)).ToList();

            return result;
        }

        private static string BuildPath(FrnFilePath currentNode, FrnFilePath parentNode)
        {
            return string.Concat(new string[] { parentNode.Path, "\\", currentNode.FileName });
        }
    }
}
