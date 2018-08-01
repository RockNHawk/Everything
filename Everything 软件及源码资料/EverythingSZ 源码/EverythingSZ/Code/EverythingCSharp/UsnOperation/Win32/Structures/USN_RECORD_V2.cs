using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace PInvoke.Win32.Structures
{
    /// <summary>
    /// Contains the USN Record Length(32bits), USN(64bits), File Reference Number(64bits), 
    /// Parent File Reference Number(64bits), Reason Code(32bits), File Attributes(32bits),
    /// File Name Length(32bits), the File Name Offset(32bits) and the File Name.
    /// </summary>
    public struct USN_RECORD_V2
    {
        private const int FR_OFFSET = 8;
        private const int PFR_OFFSET = 16;
        private const int USN_OFFSET = 24;
        private const int REASON_OFFSET = 40;
        private const int FA_OFFSET = 52;
        private const int FNL_OFFSET = 56;
        private const int FN_OFFSET = 58;

        public readonly UInt32 RecordLength;
        public readonly UInt64 FileReferenceNumber;
        public readonly UInt64 ParentFileReferenceNumber;
        public readonly Int64 Usn;
        public readonly UInt32 Reason;
        public readonly UInt32 FileAttributes;
        public readonly Int32 FileNameLength;
        public readonly Int32 FileNameOffset;
        public readonly string FileName;

        /// <summary>
        /// USN Record Constructor
        /// </summary>
        /// <param name="usnRecordPtr">Buffer of bytes representing the USN Record</param>
        public USN_RECORD_V2(IntPtr usnRecordPtr)
        {
            this.RecordLength = (UInt32)Marshal.ReadInt32(usnRecordPtr);
            this.FileReferenceNumber = (UInt64)Marshal.ReadInt64(usnRecordPtr, FR_OFFSET);
            this.ParentFileReferenceNumber = (UInt64)Marshal.ReadInt64(usnRecordPtr, PFR_OFFSET);
            this.Usn = Marshal.ReadInt64(usnRecordPtr, USN_OFFSET);
            this.Reason = (UInt32)Marshal.ReadInt32(usnRecordPtr, REASON_OFFSET);
            this.FileAttributes = (UInt32)Marshal.ReadInt32(usnRecordPtr, FA_OFFSET);
            this.FileNameLength = Marshal.ReadInt16(usnRecordPtr, FNL_OFFSET);
            this.FileNameOffset = Marshal.ReadInt16(usnRecordPtr, FN_OFFSET);
            this.FileName = Marshal.PtrToStringUni(new IntPtr(usnRecordPtr.ToInt32() + this.FileNameOffset), this.FileNameLength / sizeof(char));
        }
    }
}
