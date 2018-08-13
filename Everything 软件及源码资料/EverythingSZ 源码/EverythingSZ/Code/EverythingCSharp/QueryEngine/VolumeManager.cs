using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QueryEngine
{
    public class VolumeManager
    {
        public VolumeManager Instance { get; set; } = new VolumeManager();

        public void Init() {
            var drives = DriveInfo.GetDrives();

            var map = new Dictionary<string, VolumeFileManager>();

            for (int i = 0; i < drives.Length; i++)
            {
                var d = drives[i];

                var fileMgr = new VolumeFileManager();
                fileMgr.Init(d);

                map[d.Name] = fileMgr;

            }

        }

    }

}
