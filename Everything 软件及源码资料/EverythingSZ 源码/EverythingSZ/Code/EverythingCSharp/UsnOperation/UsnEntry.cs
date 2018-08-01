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
    public struct UsnEntry
    {
        public readonly UInt32 RecordLength;
        public readonly UInt64 FileReferenceNumber;

        /// <summary>
        /// Gets the parent file reference number.
        /// When its values is 1407374883553285(0x5000000000005L), it means this file/folder is under drive root
        /// </summary>
        /// <value>
        /// The parent file reference number.
        /// </value>
        public readonly UInt64 ParentFileReferenceNumber;
        public readonly Int64 Usn;
        public readonly UInt32 Reason;
        public readonly UInt32 FileAttributes;
        public readonly Int32 FileNameLength;
        public readonly Int32 FileNameOffset;
        public readonly string FileName;

        public readonly bool IsFolder;

        //public bool IsFolder
        //{
        //    get
        //    {
        //        return (this.FileAttributes & Win32ApiConstant.FILE_ATTRIBUTE_DIRECTORY) != 0;
        //    }
        //}

        public UsnEntry(USN_RECORD_V2 usnRecord)
        {
            this.RecordLength = usnRecord.RecordLength;
            this.FileReferenceNumber = usnRecord.FileReferenceNumber;
            this.ParentFileReferenceNumber = usnRecord.ParentFileReferenceNumber;
            this.Usn = usnRecord.Usn;
            this.Reason = usnRecord.Reason;
            this.FileAttributes = usnRecord.FileAttributes;
            this.FileNameLength = usnRecord.FileNameLength;
            this.FileNameOffset = usnRecord.FileNameOffset;
            this.FileName = usnRecord.FileName;
            this.IsFolder = (this.FileAttributes & Win32ApiConstant.FILE_ATTRIBUTE_DIRECTORY) != 0;
        }
    }
}
