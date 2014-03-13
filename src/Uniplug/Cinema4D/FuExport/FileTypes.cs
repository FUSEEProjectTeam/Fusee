using System.Collections.Generic;
using System.IO;

namespace FuExport
{
    class FileTypes
    {
        private static readonly Dictionary<string, string> FileTypesDict = new Dictionary<string, string>
        {
            // image files
            {"bmp",  "Image"},
            {"gif",  "Image"},
            {"jpe",  "Image"},
            {"jpeg", "Image"},
            {"jpg",  "Image"},
            {"png",  "Image"},
            {"svg",  "Image"},

            // audio files
            {"ogg", "Sound"},
            {"oga", "Sound"},
            {"mp3", "Sound"},

            // font files
            {"ttf", "Font"},

            // script files
            {"js", "Script"}
        };

        public static string GetFileType(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return extension != null && FileTypesDict.ContainsKey(extension.Remove(0, 1))
                       ? FileTypesDict[extension.Remove(0, 1)]
                       : "File";
        }
    }
}