using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cliver
{
    public class FileSystemRoutines
    {
        static public List<string> GetFiles(string directory, bool include_subfolders = true)
        {
            List<string> fs = Directory.EnumerateFiles(directory).ToList();
            if (include_subfolders)
                foreach (string d in Directory.EnumerateDirectories(directory))
                    fs.AddRange(GetFiles(d));
            return fs;
        }

        public static string CreateDirectory(string directory, bool unique = false)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            else if (unique)
            {
                int i = 1;
                string p = directory + "_" + i;
                for (; Directory.Exists(p); p = p + "_" + (++i)) ;
                directory = p;
                Directory.CreateDirectory(directory);
            }
            return directory;
        }

        public static void ClearDirectory(string directory, bool recursive = true)
        {
            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                return;
            }

            foreach (string file in Directory.GetFiles(directory))
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
            if (recursive)
                foreach (string d in Directory.GetDirectories(directory))
                    DeleteDirectory(d, recursive);
        }

        public static void DeleteDirectory(string directory, bool recursive = true)
        {
            foreach (string file in Directory.GetFiles(directory))
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
            if (recursive)
                foreach (string d in Directory.GetDirectories(directory))
                    DeleteDirectory(d, recursive);
            Directory.Delete(directory, false);
        }

        public static void CopyFile(string file1, string file2, bool overwrite = false)
        {
            CreateDirectory(PathRoutines.GetDirFromPath(file2), false);
            File.Copy(file1, file2, overwrite);
        }

        //public static void Copy(string path1, string path2, bool overwrite = false)
        //{
        //    if (Directory.Exists(path1))
        //    {
        //        CreateDirectory(path2, false);
        //        foreach (string f in Directory.GetFiles(path1))
        //        {
        //            string f2 = PathRoutines.GetPathMirroredInDir(f, path1, path2);
        //            File.Copy(f, f2, overwrite);
        //        }
        //        foreach (string d in Directory.GetDirectories(path1))
        //        {
        //            string d2 = PathRoutines.GetPathMirroredInDir(d, path1, path2);
        //            Copy(d, d2, overwrite);
        //        }
        //        CreateDirectory(PathRoutines.GetDirFromPath(path2), false);
        //    }
        //    else
        //    {
        //        string f2 = PathRoutines.GetPathMirroredInDir(f, path1, path2);
        //        File.Copy(f, f2, overwrite);
        //    }
        //}
    }
}
