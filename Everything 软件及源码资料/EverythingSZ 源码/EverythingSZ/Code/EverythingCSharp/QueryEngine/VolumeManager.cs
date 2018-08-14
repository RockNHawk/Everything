using Rhythm.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEngine
{
    public class VolumeManager
    {
        public static VolumeManager Instance { get; set; } = new VolumeManager();

        internal Dictionary<string, VolumeFileManager> mgrs;

        public async Task<Dictionary<string, VolumeFileManager>> Init()
        {
            var drives = DriveInfo.GetDrives()
                .Where(d => d.IsReady && d.DriveFormat.ToUpper() == "NTFS");//.ToList();


            //var map = new Dictionary<string, VolumeFileManager>();
            //for (int i = 0; i < drives.Length; i++)
            //{
            //    var d = drives[i];

            //    var fileMgr = new VolumeFileManager();
            //    fileMgr.Init(d);

            //    map[d.Name] = fileMgr;
            //}

            var mgrs = (await TaskUtility.PartitionerExecuteAsync(drives, (drive, i) =>
            {
                return Task.Run(() =>
                {
                    var fileMgr = new VolumeFileManager();
                    fileMgr.Init(drive);
                    return fileMgr;
                });
                //var fileMgr = new VolumeFileManager();
                //fileMgr.Init(drive);
                //return Task.FromResult(fileMgr);
            }))
            .ToDictionary(x => x.Result.VolumeName, x => x.Result);

            this.mgrs = mgrs;
            System.GC.Collect();

            return mgrs;

        }

    }

}
