//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace Cliver
{
    public class FileSystemRoutines
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="PATHs">dirs listes like environmentVariable PATH</param>
        /// <param name="PATHEXTs">extensions listed like environmentVariable PATHEXT</param>
        /// <returns></returns>
        public static string FindFullCommandLinePath(string fileName, string PATHs, string PATHEXTs)
        {
            var paths = new[] { Environment.CurrentDirectory }.Concat(PATHs.Split(';'));
            var extensions = new[] { string.Empty }.Concat(PATHEXTs.Split(';').Where(e => e.StartsWith(".")));
            var combinations = paths.SelectMany(x => extensions, (path, extension) => Path.Combine(path, fileName + extension));
            return combinations.FirstOrDefault(File.Exists);
        }

        public static bool IsCaseSensitive
        {
            get
            {
                if (isCaseSensitive == null)
                {
                    var tmp = Path.GetTempPath();
                    isCaseSensitive = !Directory.Exists(tmp.ToUpper()) || !Directory.Exists(tmp.ToLower());
                }
                return (bool)isCaseSensitive;
            }
        }
        static bool? isCaseSensitive = null;

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
            DirectoryInfo di = new DirectoryInfo(directory);
            if (!di.Exists)
                di.Create();
            else if (unique)
            {
                int i = 0;
                do
                {
                    di = new DirectoryInfo(directory + "_" + (++i));
                }
                while (di.Exists);
                di.Create();
            }
            return di.FullName;
        }

        public static void CopyDirectory(string directory1, string directory2, bool overwrite = false)
        {
            if (!Directory.Exists(directory2))
                Directory.CreateDirectory(directory2);
            foreach (string file in Directory.GetFiles(directory1))
                File.Copy(file, directory2 + Path.DirectorySeparatorChar + PathRoutines.GetFileName(file), overwrite);
            foreach (string d in Directory.GetDirectories(directory1))
                CopyDirectory(d, directory2 + Path.DirectorySeparatorChar + PathRoutines.GetDirName(d), overwrite);
        }

        public static void ClearDirectory(string directory, bool recursive = true, bool throwException = true)
        {
            try
            {
                if (!Directory.Exists(directory))
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
            catch (Exception e)
            {
                if (throwException)
                    throw;
            }
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

        public static bool DeleteDirectorySteadfastly(string directory, bool recursive = true)
        {
            bool error = false;
            foreach (string file in Directory.GetFiles(directory))
            {
                try
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                catch
                {
                    error = true;
                }
            }
            if (recursive)
                foreach (string d in Directory.GetDirectories(directory))
                    DeleteDirectorySteadfastly(d, recursive);
            try
            {
                Directory.Delete(directory, false);
            }
            catch
            {
                error = true;
            }
            return !error;
        }

        /// <summary>
        /// (!)It throws an exception when the destination file exists and !overwrite.
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <param name="overwrite"></param>
        public static void CopyFile(string file1, string file2, bool overwrite = false)
        {
            CreateDirectory(PathRoutines.GetFileDir(file2), false);
            File.Copy(file1, file2, overwrite);//(!)it throws an exception when the destination file exists and !overwrite
        }

        public static void MoveFile(string file1, string file2, bool overwrite = true)
        {
            CreateDirectory(PathRoutines.GetFileDir(file2), false);
            if (File.Exists(file2))
            {
                if (!overwrite)
                    throw new System.Exception("File " + file2 + " already exists.");
                File.Delete(file2);
            }
            File.Move(file1, file2);
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
        //    }
        //    else
        //    {
        //        string f2 = PathRoutines.GetPathMirroredInDir(f, path1, path2);
        //        File.Copy(f, f2, overwrite);
        //    }
        //}

        public static bool IsFileLocked(string file)
        {
            try
            {
                using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None))
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
