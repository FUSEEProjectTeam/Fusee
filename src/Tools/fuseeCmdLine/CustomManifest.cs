using System.Collections.Generic;

namespace Fusee.Tools.fuseeCmdLine
{
    partial class ManifestFile
    {
        private readonly int _fileCount;

        private readonly string _projName;
        private readonly IList<string> _fileNames;
        private readonly IList<long> _fileSize;
        private readonly IList<string> _fileTypes;
        private readonly IList<string> _fileFormats;

        public ManifestFile(string projName, IList<string> fileNames, IList<long> fileSize, IList<string> fileTypes, IList<string> fileFormats)
        {
            _projName = projName;
            _fileNames = fileNames;
            _fileSize = fileSize;
            _fileTypes = fileTypes;
            _fileFormats = fileFormats;

            _fileCount = _fileNames.Count;
        }

        /*
        public ManifestFile(string projName, List<string> filePaths, List<string> dstRelPaths, int specFiles)
        {
            _projName = projName;
            _fileCount = filePaths.Count;

            var fileNamesList = new List<string>();
            var fileSizeList = new List<long>();
            var fileTypesList = new List<string>();
            var fileFormatsList = new List<string>();

            GenerateAssetManifestEntryItems(filePaths, dstRelPaths, specFiles, fileNamesList, fileSizeList, fileTypesList, fileFormatsList);

            // Convert to Array
            _fileCount = fileNamesList.Count;

            _fileNames = fileNamesList.ToArray();
            _fileSize = fileSizeList.ToArray();
            _fileTypes = fileTypesList.ToArray();
            _fileFormats = fileFormatsList.ToArray();
        }
        */
    }
}