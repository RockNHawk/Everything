using PInvoke.Win32.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UsnOperation
{
    class UsnEntryFactory
    {
        private const int FR_OFFSET = 8;
        private const int PFR_OFFSET = 16;
        private const int USN_OFFSET = 24;
        private const int REASON_OFFSET = 40;
        private const int FA_OFFSET = 52;
        private const int FNL_OFFSET = 56;
        private const int FN_OFFSET = 58;

        /// <summary>
        /// USN Record Constructor
        /// </summary>
        /// <param name="usnRecordPtr">Buffer of bytes representing the USN Record</param>
        public static UsnEntry Create(IntPtr usnRecordPtr)
        {
            var fileNameOffset = Marshal.ReadInt16(usnRecordPtr, FN_OFFSET);
            var fileNameLength = Marshal.ReadInt16(usnRecordPtr, FNL_OFFSET);
            var fileAttributes = (UInt32)Marshal.ReadInt32(usnRecordPtr, FA_OFFSET);

            var entry = new UsnEntry()
            {
                RecordLength = (UInt32)Marshal.ReadInt32(usnRecordPtr),
                FileReferenceNumber = (UInt64)Marshal.ReadInt64(usnRecordPtr, FR_OFFSET),
                ParentFileReferenceNumber = (UInt64)Marshal.ReadInt64(usnRecordPtr, PFR_OFFSET),
                Usn = Marshal.ReadInt64(usnRecordPtr, USN_OFFSET),
                Reason = (UInt32)Marshal.ReadInt32(usnRecordPtr, REASON_OFFSET),
                FileAttributes = fileAttributes,
                FileNameLength = fileNameLength,
                FileNameOffset = fileNameOffset,
                FileName = Marshal.PtrToStringUni(new IntPtr(usnRecordPtr.ToInt32() + fileNameOffset), fileNameLength / sizeof(char)),
                IsFolder = (fileAttributes & Win32ApiConstant.FILE_ATTRIBUTE_DIRECTORY) != 0,
            };
            return entry;
        }

    }
}
