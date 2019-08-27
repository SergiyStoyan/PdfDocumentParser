//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        27 February 2007
//Copyright: (C) 2007, Sergey Stoyan
//********************************************************************************************
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace Cliver
{
    /// <summary>
    /// Miscellaneous useful methods for file path/name construction
    /// </summary>
    public static class PathRoutines
    {
        public static bool ArePathsEqual(string path1, string path2)
        {
            var p1 = GetNormalizedPath(path1, true);
            var p2 = GetNormalizedPath(path2, true);
            return p1 == p2;
        }

        public static bool IsDirWithinDir(string dir1, string dir2)
        {
            var p1 = GetNormalizedPath(dir1, true);
            var p2 = GetNormalizedPath(dir2, true);
            string[] p1s = p1.Split(Path.DirectorySeparatorChar);
            string[] p2s = p2.Split(Path.DirectorySeparatorChar);
            if (p1s.Length < p2s.Length)
                return false;
            for (int i = 0; i < p2s.Length; i++)
                if (p1s[i] != p2s[i])
                    return false;
            return true;
        }

        public static string GetNormalizedPath(string path, bool lowerCaseIfIsCaseInsensitive)
        {
            string p = Path.GetFullPath(new Uri(path).LocalPath).TrimEnd(Path.DirectorySeparatorChar);
            if (lowerCaseIfIsCaseInsensitive && !FileSystemRoutines.IsCaseSensitive)
                return p.ToLowerInvariant();
            return p;
        }

        public static string GetAbsolutePath(string path)
        {
            if (Path.IsPathRooted(path))
                return path;
            return Log.AppDir + Path.DirectorySeparatorChar + path;
        }

        public static string GetLegalizedPath(string path, bool webDecode = false, string illegalCharReplacement = "")
        {
            if (webDecode)
            {
                path = HttpUtility.HtmlDecode(path);
                path = HttpUtility.UrlDecode(path);
            }
            return Regex.Replace(path, invalidPathChars, illegalCharReplacement);
        }
        static string invalidPathChars = "[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]";

        public static string GetLegalizedFileName(string file, bool webDecode = false, string illegalCharReplacement = "")
        {
            if (webDecode)
            {
                file = HttpUtility.HtmlDecode(file);
                file = HttpUtility.UrlDecode(file);
            }
            return Regex.Replace(file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1), invalidFileNameChars, illegalCharReplacement);
        }
        static string invalidFileNameChars = "[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]";
        
        public static string GetLegalizedFile(string file, bool webDecode = false, string illegalCharReplacement = "")
        {
            if (webDecode)
            {
                file = HttpUtility.HtmlDecode(file);
                file = HttpUtility.UrlDecode(file);
            }
            int p = file.LastIndexOf(Path.DirectorySeparatorChar) + 1;
            return Regex.Replace(file.Substring(0, p), invalidPathChars, illegalCharReplacement) + Regex.Replace(file.Substring(p), invalidFileNameChars, illegalCharReplacement);
        }

        /// <summary>
        /// Works for any length path unlike Path.GetFileName()
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetFileName(string file)
        {
            return Regex.Replace(file, @".*"+ Regex.Escape(Path.DirectorySeparatorChar.ToString()), "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        public static string GetFileNameWithoutExtention(string file)
        {
            string n = GetFileName(file);
            return Regex.Replace(n, @"\.[^\.]*$", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        /// <summary>
        /// Works for any length path unlike Path.GetFileName()
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static string GetDirName(string dir)
        {
            return Regex.Replace(dir.TrimEnd(Path.DirectorySeparatorChar), @".*" + Regex.Escape(Path.DirectorySeparatorChar.ToString()), "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        public static string InsertSuffixBeforeFileExtension(string file, string suffix)
        {
            Match m = Regex.Match(file, @"(.*)(\.[^" + Regex.Escape(Path.DirectorySeparatorChar.ToString()) + "]*)$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success)
                return m.Groups[1].Value + suffix + m.Groups[2].Value;
            return file + suffix;
        }

        /// <summary>
        /// Works for any length path unlike Path.GetFileName()
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetFileExtension(string file)
        {
            return Regex.Replace(file, @".*\.", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        /// <summary>
        /// Works for any length path unlike Path.GetDir()
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>        
        public static string GetFileDir(string file, bool removeTrailingSeparator = true)
        {
            string fd = Regex.Replace(file, @"[^" + Regex.Escape(Path.DirectorySeparatorChar.ToString()) + @"]*$", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (removeTrailingSeparator)
                fd = fd.TrimEnd(Path.DirectorySeparatorChar);
            return fd;
        }

        public static string ReplaceFileExtention(string file, string extention)
        {
            return Regex.Replace(file, @"\.[^\.]+$", "." + extention, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        public static string GetPathMirroredInDir(string path, string rootDir, string mirrorDir)
        {
            string p = GetNormalizedPath(path, false);
            string rd = GetNormalizedPath(rootDir, false);
            string md = GetNormalizedPath(mirrorDir, false);
            return Regex.Replace(p, @"^\s*" + Regex.Escape(rd), md);
        }

        public static string GetRelativePath(string path, string baseDir)
        {
            string p = GetNormalizedPath(path, false);
            string bd = GetNormalizedPath(baseDir, false);
            return Regex.Replace(p, @"^\s*" + Regex.Escape(bd), "");
        }
    }
}