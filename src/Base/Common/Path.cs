using System;
using System.Globalization;
using System.Text;

namespace Fusee.Base.Common
{
    /// <summary>
    /// Methods to perform various operations on strings resembling file paths.
    /// </summary>
    public static class Path
    {
        private static readonly char[] _invalidPathChars;

        /// <summary>
        /// The alternative directory separator character ('/' on windows).
        /// </summary>
        public static readonly char AltDirectorySeparatorChar;
        
        /// <summary>
        /// The directory separator character ('\' on windows, '/' on most other platforms).
        /// </summary>
        public static readonly char DirectorySeparatorChar;
        
        /// <summary>
        /// The path separator character (';' on most platforms)
        /// </summary>
        public static readonly char PathSeparator;
        
        /// <summary>
        /// The directory separator as a string.
        /// </summary>
        internal static readonly string DirectorySeparatorStr;
        
        /// <summary>
        /// The volume separator character (':' on windows).
        /// </summary>
        public static readonly char VolumeSeparatorChar;

        internal static readonly char[] PathSeparatorChars;
        private static readonly bool dirEqualsVolume;

        /// <summary>
        /// Changes the file extension (".txt") of a path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="extension">The extension.</param>
        /// <returns>The path with the exchanged file extension.</returns>
        /// <exception cref="System.ArgumentException">Illegal characters in path.</exception>
        public static string ChangeExtension(string path, string extension)
        {
            if (path == null)
                return null;

            if (path.IndexOfAny(_invalidPathChars) != -1)
                throw new ArgumentException("Illegal characters in path.");

            var iExt = findExtension(path);

            if (extension == null)
                return iExt < 0 ? path : path.Substring(0, iExt);
            if (extension.Length == 0)
                return iExt < 0 ? path + '.' : path.Substring(0, iExt + 1);

            if (path.Length != 0)
            {
                if (extension.Length > 0 && extension[0] != '.')
                    extension = "." + extension;
            }
            else
                extension = string.Empty;

            if (iExt < 0)
            {
                return path + extension;
            }
            if (iExt > 0)
            {
                var temp = path.Substring(0, iExt);
                return temp + extension;
            }

            return extension;
        }

        /// <summary>
        /// Combines two paths. Considers potential trailing directory separators.
        /// </summary>
        /// <param name="path1">Path #1. Leftmost part of the result.</param>
        /// <param name="path2">Path #2. Rightmost part of the result.</param>
        /// <returns>The combined path.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// path1
        /// or
        /// path2
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Illegal characters in path.
        /// or
        /// Illegal characters in path.
        /// </exception>
        public static string Combine(string path1, string path2)
        {
            if (path1 == null)
                throw new ArgumentNullException("path1");

            if (path2 == null)
                throw new ArgumentNullException("path2");

            if (path1.Length == 0)
                return path2;

            if (path2.Length == 0)
                return path1;

            if (path1.IndexOfAny(_invalidPathChars) != -1)
                throw new ArgumentException("Illegal characters in path.");

            if (path2.IndexOfAny(_invalidPathChars) != -1)
                throw new ArgumentException("Illegal characters in path.");

            //TODO???: UNC names
            if (IsPathRooted(path2))
                return path2;

            var p1end = path1[path1.Length - 1];
            if (p1end != DirectorySeparatorChar && p1end != AltDirectorySeparatorChar && p1end != VolumeSeparatorChar)
                return path1 + DirectorySeparatorStr + path2;

            return path1 + path2;
        }


        internal static string CleanPath(string s)
        {
            var l = s.Length;
            var sub = 0;
            var start = 0;

            // Host prefix?
            var s0 = s[0];
            if (l > 2 && s0 == '\\' && s[1] == '\\')
            {
                start = 2;
            }

            // We are only left with root
            if (l == 1 && (s0 == DirectorySeparatorChar || s0 == AltDirectorySeparatorChar))
                return s;

            // Cleanup
            for (var i = start; i < l; i++)
            {
                var c = s[i];

                if (c != DirectorySeparatorChar && c != AltDirectorySeparatorChar)
                    continue;
                if (i + 1 == l)
                    sub++;
                else
                {
                    c = s[i + 1];
                    if (c == DirectorySeparatorChar || c == AltDirectorySeparatorChar)
                        sub++;
                }
            }

            if (sub == 0)
                return s;

            var copy = new char[l - sub];
            if (start != 0)
            {
                copy[0] = '\\';
                copy[1] = '\\';
            }
            for (int i = start, j = start; i < l && j < copy.Length; i++)
            {
                var c = s[i];

                if (c != DirectorySeparatorChar && c != AltDirectorySeparatorChar)
                {
                    copy[j++] = c;
                    continue;
                }

                // For non-trailing cases.
                if (j + 1 != copy.Length)
                {
                    copy[j++] = DirectorySeparatorChar;
                    for (; i < l - 1; i++)
                    {
                        c = s[i + 1];
                        if (c != DirectorySeparatorChar && c != AltDirectorySeparatorChar)
                            break;
                    }
                }
            }
            return new string(copy);
        }

        /// <summary>
        /// Extracts the directory name from a given path (which is potentially a path to a file).
        /// </summary>
        /// <param name="path">The path to extrect from.</param>
        /// <returns>The directory name.</returns>
        /// <exception cref="System.ArgumentException">
        /// Invalid path
        /// or
        /// Argument string consists of whitespace characters only.
        /// or
        /// Path contains invalid characters
        /// </exception>
        public static string GetDirectoryName(string path)
        {
            // For empty string MS docs say both
            // return null AND throw exception.  Seems .NET throws.
            if (path == string.Empty)
                throw new ArgumentException("Invalid path");

            if (path == null || GetPathRoot(path) == path)
                return null;

            if (path.Trim().Length == 0)
                throw new ArgumentException("Argument string consists of whitespace characters only.");

            if (path.IndexOfAny(_invalidPathChars) > -1)
                throw new ArgumentException("Path contains invalid characters");

            var nLast = path.LastIndexOfAny(PathSeparatorChars);
            if (nLast == 0)
                nLast++;

            if (nLast > 0)
            {
                var ret = path.Substring(0, nLast);
                var l = ret.Length;

                if (l >= 2 && DirectorySeparatorChar == '\\' && ret[l - 1] == VolumeSeparatorChar)
                    return ret + DirectorySeparatorChar;
                if (l == 1 && DirectorySeparatorChar == '\\' && path.Length >= 2 && path[nLast] == VolumeSeparatorChar)
                    return ret + VolumeSeparatorChar;
                //
                // Important: do not use CanonicalizePath here, use
                // the custom CleanPath here, as this should not
                // return absolute paths
                //
                return CleanPath(ret);
            }

            return string.Empty;
        }

        /// <summary>
        /// Retrieves the file extension (e.g. ".txt") from a given path.
        /// </summary>
        /// <param name="path">The path to get the extension from.</param>
        /// <returns>The extension.</returns>
        /// <exception cref="System.ArgumentException">Illegal characters in path.</exception>
        public static string GetExtension(string path)
        {
            if (path == null)
                return null;

            if (path.IndexOfAny(_invalidPathChars) != -1)
                throw new ArgumentException("Illegal characters in path.");

            var iExt = findExtension(path);

            if (iExt > -1)
            {
                if (iExt < path.Length - 1)
                    return path.Substring(iExt);
            }
            return string.Empty;
        }

        /// <summary>
        /// Retrieves a file name from the given path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The file name</returns>
        /// <exception cref="System.ArgumentException">Illegal characters in path.</exception>
        public static string GetFileName(string path)
        {
            if (path == null || path.Length == 0)
                return path;

            if (path.IndexOfAny(_invalidPathChars) != -1)
                throw new ArgumentException("Illegal characters in path.");

            var nLast = path.LastIndexOfAny(PathSeparatorChars);
            if (nLast >= 0)
                return path.Substring(nLast + 1);

            return path;
        }

        /// <summary>
        /// Retrieves the file name without extension from a given path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The file name without extension.</returns>
        public static string GetFileNameWithoutExtension(string path)
        {
            return ChangeExtension(GetFileName(path), null);
        }


        internal static bool IsDirectorySeparator(char c)
        {
            return c == DirectorySeparatorChar || c == AltDirectorySeparatorChar;
        }

        /// <summary>
        /// Retrieves the path root from a given path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The path root.</returns>
        /// <exception cref="System.ArgumentException">The specified path is not of a legal form.</exception>
        /// <exception cref="System.NotImplementedException"> return Directory.GetCurrentDirectory().Substring(0, 2); </exception>
        public static string GetPathRoot(string path)
        {
            if (path == null)
                return null;

            if (path.Trim().Length == 0)
                throw new ArgumentException("The specified path is not of a legal form.");

            if (!IsPathRooted(path))
                return string.Empty;

            if (DirectorySeparatorChar == '/')
            {
                // UNIX
                return IsDirectorySeparator(path[0]) ? DirectorySeparatorStr : string.Empty;
            }
            // Windows
            var len = 2;

            if (path.Length == 1 && IsDirectorySeparator(path[0]))
                return DirectorySeparatorStr;
            if (path.Length < 2)
                return string.Empty;

            if (IsDirectorySeparator(path[0]) && IsDirectorySeparator(path[1]))
            {
                // UNC: \\server or \\server\share
                // Get server
                while (len < path.Length && !IsDirectorySeparator(path[len])) len++;

                // Get share
                if (len < path.Length)
                {
                    len++;
                    while (len < path.Length && !IsDirectorySeparator(path[len])) len++;
                }

                return DirectorySeparatorStr +
                       DirectorySeparatorStr +
                       path.Substring(2, len - 2).Replace(AltDirectorySeparatorChar, DirectorySeparatorChar);
            }
            if (IsDirectorySeparator(path[0]))
            {
                // path starts with '\' or '/'
                return DirectorySeparatorStr;
            }
            if (path[1] == VolumeSeparatorChar)
            {
                // C:\folder
                if (path.Length >= 3 && IsDirectorySeparator(path[2])) len++;
            }
            else
                throw new NotImplementedException(" return Directory.GetCurrentDirectory().Substring(0, 2); ");
            return path.Substring(0, len);
        }

        /// <summary>
        /// Determines whether the specified path has an extension.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>true if the path has an extenstion, otherwise false.</returns>
        /// <exception cref="System.ArgumentException">Illegal characters in path.</exception>
        public static bool HasExtension(string path)
        {
            if (path == null || path.Trim().Length == 0)
                return false;

            if (path.IndexOfAny(_invalidPathChars) != -1)
                throw new ArgumentException("Illegal characters in path.");

            var pos = findExtension(path);
            return 0 <= pos && pos < path.Length - 1;
        }

        /// <summary>
        /// Determines whether the specified path has a root.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>true if the path has a root, otherwise false.</returns>
        /// <exception cref="System.ArgumentException">Illegal characters in path.</exception>
        public static bool IsPathRooted(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            if (path.IndexOfAny(_invalidPathChars) != -1)
                throw new ArgumentException("Illegal characters in path.");

            var c = path[0];
            return c == DirectorySeparatorChar ||
                   c == AltDirectorySeparatorChar ||
                   (!dirEqualsVolume && path.Length > 1 && path[1] == VolumeSeparatorChar);
        }

        /// <summary>
        /// Gets the invalid file name chars.
        /// </summary>
        /// <returns>An array containing characters that must NOT be used in file names</returns>
        public static char[] GetInvalidFileNameChars()
        {
            // if (Environment.IsRunningOnWindows)
            {
                return new char[41]
                {
                    '\x00', '\x01', '\x02', '\x03', '\x04', '\x05', '\x06', '\x07',
                    '\x08', '\x09', '\x0A', '\x0B', '\x0C', '\x0D', '\x0E', '\x0F', '\x10', '\x11', '\x12',
                    '\x13', '\x14', '\x15', '\x16', '\x17', '\x18', '\x19', '\x1A', '\x1B', '\x1C', '\x1D',
                    '\x1E', '\x1F', '\x22', '\x3C', '\x3E', '\x7C', ':', '*', '?', '\\', '/'
                };
            }
            // return new char[2] {'\x00', '/'};
        }

        /// <summary>
        /// Gets the invalid path chars.
        /// </summary>
        /// <returns>An array containing characters that must NOT be used in path names</returns>
        public static char[] GetInvalidPathChars()
        {
            // return a new array as we do not want anyone to be able to change the values
            // if (Environment.IsRunningOnWindows)
            {
                return new char[36]
                {
                    '\x22', '\x3C', '\x3E', '\x7C', '\x00', '\x01', '\x02', '\x03', '\x04', '\x05', '\x06', '\x07',
                    '\x08', '\x09', '\x0A', '\x0B', '\x0C', '\x0D', '\x0E', '\x0F', '\x10', '\x11', '\x12',
                    '\x13', '\x14', '\x15', '\x16', '\x17', '\x18', '\x19', '\x1A', '\x1B', '\x1C', '\x1D',
                    '\x1E', '\x1F'
                };
            }
            // return new char[1] {'\x00'};
        }

        // private class methods

        private static int findExtension(string path)
        {
            // method should return the index of the path extension
            // start or -1 if no valid extension
            if (path != null)
            {
                var iLastDot = path.LastIndexOf('.');
                var iLastSep = path.LastIndexOfAny(PathSeparatorChars);

                if (iLastDot > iLastSep)
                    return iLastDot;
            }
            return -1;
        }

        static Path()
        {
            VolumeSeparatorChar = ':';
            DirectorySeparatorChar = '\\';
            AltDirectorySeparatorChar = '/';

            PathSeparator = ';';
            _invalidPathChars = GetInvalidPathChars();

            DirectorySeparatorStr = DirectorySeparatorChar.ToString();
            PathSeparatorChars = new[]
            {
                DirectorySeparatorChar,
                AltDirectorySeparatorChar,
                VolumeSeparatorChar
            };

            dirEqualsVolume = DirectorySeparatorChar == VolumeSeparatorChar;
        }

        // returns the server and share part of a UNC. Assumes "path" is a UNC.
        private static string GetServerAndShare(string path)
        {
            var len = 2;
            while (len < path.Length && !IsDirectorySeparator(path[len])) len++;

            if (len < path.Length)
            {
                len++;
                while (len < path.Length && !IsDirectorySeparator(path[len])) len++;
            }

            return path.Substring(2, len - 2).Replace(AltDirectorySeparatorChar, DirectorySeparatorChar);
        }

        private static bool SameRoot(string root, string path)
        {
            if ((root.Length < 2) || (path.Length < 2))
                return false;

            if (IsDirectorySeparator(root[0]) && IsDirectorySeparator(root[1]))
            {
                if (!(IsDirectorySeparator(path[0]) && IsDirectorySeparator(path[1])))
                    return false;

                var rootShare = GetServerAndShare(root);
                var pathShare = GetServerAndShare(path);

                return string.Compare(rootShare, pathShare, CultureInfo.InvariantCulture, CompareOptions.None) == 0;
            }

            // same volume/drive
            if (!root[0].Equals(path[0]))
                return false;
            // presence of the separator
            if (path[1] != VolumeSeparatorChar)
                return false;
            if ((root.Length > 2) && (path.Length > 2))
            {
                // but don't directory compare the directory separator
                return IsDirectorySeparator(root[2]) && IsDirectorySeparator(path[2]);
            }
            return true;
        }


        // required for FileIOPermission (and most proibably reusable elsewhere too)
        // both path MUST be "full paths"
        internal static bool IsPathSubsetOf(string subset, string path)
        {
            if (subset.Length > path.Length)
                return false;

            // check that everything up to the last separator match
            var slast = subset.LastIndexOfAny(PathSeparatorChars);
            if (string.Compare(subset, 0, path, 0, slast) != 0)
                return false;

            slast++;
            // then check if the last segment is identical
            var plast = path.IndexOfAny(PathSeparatorChars, slast);
            if (plast >= slast)
            {
                return string.Compare(subset, slast, path, slast, path.Length - plast) == 0;
            }
            if (subset.Length != path.Length)
                return false;

            return string.Compare(subset, slast, path, slast, subset.Length - slast) == 0;
        }

        /// <summary>
        /// Combines an arbitrary number of paths. Considers potential trailing directory separators.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <returns>The combined path.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// paths  or  one of the paths contains a null value
        /// </exception>
        /// <exception cref="System.ArgumentException">Illegal characters in path.</exception>
        public static string Combine(params string[] paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            bool need_sep;
            var ret = new StringBuilder();
            var pathsLen = paths.Length;
            int slen;
            need_sep = false;

            foreach (var s in paths)
            {
                if (s == null)
                    throw new ArgumentNullException("paths", "One of the paths contains a null value");
                if (s.Length == 0)
                    continue;
                if (s.IndexOfAny(_invalidPathChars) != -1)
                    throw new ArgumentException("Illegal characters in path.");

                if (need_sep)
                {
                    need_sep = false;
                    ret.Append(DirectorySeparatorStr);
                }

                pathsLen--;
                if (IsPathRooted(s))
                    ret.Length = 0;

                ret.Append(s);
                slen = s.Length;
                if (slen > 0 && pathsLen > 0)
                {
                    var p1end = s[slen - 1];
                    if (p1end != DirectorySeparatorChar && p1end != AltDirectorySeparatorChar &&
                        p1end != VolumeSeparatorChar)
                        need_sep = true;
                }
            }

            return ret.ToString();
        }


        /// <summary>
        /// Combines three paths. Considers potential trailing directory separators.
        /// </summary>
        /// <param name="path1">Path #1. Leftmost part of the result.</param>
        /// <param name="path2">Path #2.</param>
        /// <param name="path3">Path #3. Rightmost part of the result.</param>
        /// <returns>The combined path.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// path1
        /// or
        /// path2
        /// or
        /// path3
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Illegal characters in path1.
        /// or
        /// Illegal characters in path2.
        /// or
        /// Illegal characters in path3.
        /// </exception>

        public static string Combine(string path1, string path2, string path3)
        {
            if (path1 == null)
                throw new ArgumentNullException("path1");

            if (path2 == null)
                throw new ArgumentNullException("path2");

            if (path3 == null)
                throw new ArgumentNullException("path3");

            return Combine(new[] {path1, path2, path3});
        }

        /// <summary>
        /// Combines three paths. Considers potential trailing directory separators.
        /// </summary>
        /// <param name="path1">Path #1. Leftmost part of the result.</param>
        /// <param name="path2">Path #2.</param>
        /// <param name="path3">Path #3.</param>
        /// <param name="path4">Path #4. Rightmost part of the result.</param>
        /// <returns>The combined path.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// path1
        /// or
        /// path2
        /// or
        /// path3
        /// or
        /// path4
        /// </exception>
        public static string Combine(string path1, string path2, string path3, string path4)
        {
            if (path1 == null)
                throw new ArgumentNullException("path1");

            if (path2 == null)
                throw new ArgumentNullException("path2");

            if (path3 == null)
                throw new ArgumentNullException("path3");

            if (path4 == null)
                throw new ArgumentNullException("path4");

            return Combine(new[] {path1, path2, path3, path4});
        }

        internal static void Validate(string path)
        {
            Validate(path, "path");
        }

        internal static void Validate(string path, string parameterName)
        {
            if (path == null)
                throw new ArgumentNullException(parameterName);
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path is empty");
            if (path.IndexOfAny(_invalidPathChars) != -1)
                throw new ArgumentException("Path contains invalid chars");
            // if (Environment.IsRunningOnWindows)
            {
                var idx = path.IndexOf(':');
                if (idx >= 0 && idx != 1)
                    throw new ArgumentException(parameterName);
            }
        }
    }
}
