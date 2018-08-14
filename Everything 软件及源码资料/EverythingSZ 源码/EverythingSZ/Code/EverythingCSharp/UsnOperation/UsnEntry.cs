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
        public UInt32 RecordLength;
        public UInt64 FileReferenceNumber;

        /// <summary>
        /// Gets the parent file reference number.
        /// When its values is 1407374883553285(0x5000000000005L), it means this file/folder is under drive root
        /// </summary>
        /// <value>
        /// The parent file reference number.
        /// </value>
        public UInt64 ParentFileReferenceNumber;
        public Int64 Usn;
        public UInt32 Reason;
        public UInt32 FileAttributes;
        public Int32 FileNameLength;
        public Int32 FileNameOffset;
        public string FileName;

        public bool IsFolder;

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

        //public UsnEntry(
        //    UInt32 RecordLength，
        //    UInt64 FileReferenceNumber，
        //    UInt64 ParentFileReferenceNumber，
        //    Int64 Usn，
        //    UInt32 Reason，
        //    UInt32 FileAttributes，
        //    Int16 FileNameLength，
        //    Int16 FileNameOffset，
        //    string FileName
        //    )
        //{

        //}

    }
}
