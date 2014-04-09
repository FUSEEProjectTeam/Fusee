using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FuExport
{
    partial class ManifestFile
    {
        private readonly int _fileCount;

        private readonly string _projName;
        private readonly string[] _fileNames;
        private readonly long[] _fileSize;
        private readonly string[] _fileTypes;
        private readonly string[] _fileFormats;

        public ManifestFile(string projName, ICollection<string> filePaths, int specFiles)
        {
            _projName = projName;
            _fileCount = filePaths.Count;

            var fileNamesList = new List<string>();
            var fileSizeList = new List<long>();
            var fileTypesList = new List<string>();
            var fileFormatsList = new List<string>();

            for (var ct = 0; ct <= filePaths.Count - 1; ct++)
                {
                    string filePath = filePaths.ElementAt(ct);

                    var fInfo = new FileInfo(filePath);
                    string fType = FileTypes.GetFileType(filePath);

                    // size, type
                    fileSizeList.Add(fInfo.Length);
                    fileTypesList.Add(fType);

                    // special files
                    if (ct < specFiles)
                    {
                        fileNamesList.Add(Path.GetFileName(filePath));
                        fileFormatsList.Add(" ");
                        continue;
                    }

                    // Sound files in more than one format
                    var doubleExt = false;

                    if ((fType == "Sound") && (ct < filePaths.Count - 1))
                    {
                        string fileName1 = Path.GetFileNameWithoutExtension(filePath);
                        string fileName2 = Path.GetFileNameWithoutExtension(filePaths.ElementAt(ct + 1));

                        if (fileName1 == fileName2)
                        {
                            string ext1 = Path.GetExtension(filePath);
                            string ext2 = Path.GetExtension(filePaths.ElementAt(ct + 1));

                            fileFormatsList.Add(" \"formats\": [\"" + ext1 + "\", \"" + ext2 + "\"],	");
                            fileNamesList.Add("Assets/" + fileName1);

                            ct++;
                            doubleExt = true;
                        }
                    }

                    if (!doubleExt)
                    {
                        fileNamesList.Add("Assets/" + Path.GetFileName(filePath));
                        fileFormatsList.Add(" ");
                    }
                }

            // Convert to Array
            _fileCount = fileNamesList.Count;

            _fileNames = new string[_fileCount];
            _fileSize = new long[_fileCount];
            _fileTypes = new string[_fileCount];
            _fileFormats = new string[_fileCount];

            _fileNames = fileNamesList.ToArray();
            _fileSize = fileSizeList.ToArray();
            _fileTypes = fileTypesList.ToArray();
            _fileFormats = fileFormatsList.ToArray();
        }
    }
}