using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Backend
{
    public class FileReader
    {
        public static List<string> ReadCsvFile(string relativePathToFile)
        {
            // var path = $"Assets/Resource/{relativePathToFile}";
            var reader = new StreamReader(relativePathToFile);
            var content = reader.ReadToEnd().Split(',').ToList();
            return content;
        }
    }
}