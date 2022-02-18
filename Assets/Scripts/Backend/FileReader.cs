using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Backend
{
    public static class FileReader
    {
        /// <summary>
        /// Reads a CSV file from a path relative to the game folder.
        /// </summary>
        /// <param name="relativePathToFile">string for the path relative to the StreamingAssets path in the root folder.
        /// EX: if the file path is 'StreamingAssets/Words/word_01.csv' relativePath will be 'Words/word_01.csv</param>
        /// <returns>A string list of the read values</returns>
        public static List<string> ReadCsvFile(string relativePathToFile)
        {
            var path    = Path.Combine(Application.streamingAssetsPath, relativePathToFile);
            var reader  = new StreamReader(path);
            var content = reader.ReadToEnd().Split(',').ToList();
            return content;
        }
    }
}