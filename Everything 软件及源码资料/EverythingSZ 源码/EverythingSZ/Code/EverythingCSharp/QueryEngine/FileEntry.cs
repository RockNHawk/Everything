// -----------------------------------------------------------------------
// <copyright file="UsnEntry.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace UsnOperation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using PInvoke.Win32.Constants;
    using PInvoke.Win32.Structures;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class FileEntry
    {
        public string FileName { get; set; }

        public   bool IsFolder { get; set; }

        public FileEntry Parent { get; set; }

        public string Path { get; set; }

        public FileEntry()
        {

        }
    }
}
