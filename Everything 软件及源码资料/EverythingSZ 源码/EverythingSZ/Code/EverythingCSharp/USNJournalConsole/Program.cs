using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using QueryEngine;
using System.Diagnostics;
using System.Threading.Tasks;

namespace USNJournalConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();


            watch.Start();
            watch.Stop();

            double time = 0d;

            for (int i = 0; i < 10; i++)
            {
                watch.Restart();

                /*
    new code
    1184025 files, 11.4229784 seconds

    old code
    1184037 files, 14.6285 seconds
                 */
                //var allFiles = Task.Run(() => Engine.GetAllFilesAndDirectories()).Result;
                var allFiles = Engine.GetAllFilesAndDirectories();
                watch.Stop();

                var nowt = watch.Elapsed.TotalSeconds;
                time += nowt;
                Console.WriteLine("{0} files, {1} seconds", allFiles.Count(), nowt);
            }

            Console.WriteLine("Avg {0} seconds", time / 10);

            //var drives = DriveInfo.GetDrives();
            //foreach (var item in drives)
            //{

            //watch.Restart();
            //    //new System.IO.DirectoryInfo(item).Attributes.;


            //Console.WriteLine("drive {0} files, {1} seconds", allFiles.Count(), watch.Elapsed.TotalSeconds);
            //watch.Stop();
            //}



            //watch.Restart();
            //var aaa = GetV(@"d:\");
            //watch.Stop();
            //File.WriteAllText(@"1.txt", string.Join(Environment.NewLine, aaa));
            //File.WriteAllText(@"2.txt", string.Join(Environment.NewLine, allFiles.Select(e=>e.FullFileName)));
            //Console.WriteLine("{0} files, {1} seconds", aaa.Count(), watch.Elapsed.TotalSeconds);

            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }

        static IEnumerable<Entity> GetV(string path)
        {
            List<Entity> result = new List<Entity>();
            if (!string.IsNullOrEmpty(path) && !path.Contains("System Volume Information") && !path.Contains("RECYCLE"))
            {
                var dirs = Directory.GetDirectories(path);
                foreach (var item in dirs)
                {
                    var child = GetV(item);
                    if (child != null)
                        result.AddRange(child);
                }
                var files = Directory.GetFiles(path);
                result.AddRange(files.Select(e => new Entity(e, false)));
                result.AddRange(dirs.Select(e => new Entity(e, true)));
                return result;
            }
            return null;
        }

        public class Entity
        {
            public override string ToString()
            {
                return Path;// + "," + (IsDir ? "1" : "0");
            }
            public Entity()
            {

            }
            public Entity(string p, bool isd)
            {
                IsDir = isd;
                Path = p;
            }

            public string Path { get; set; }

            public bool IsDir { get; set; }
        }

    }
}
