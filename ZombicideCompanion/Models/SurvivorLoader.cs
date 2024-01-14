using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombicideCompanion.Models
{
    public static class SurvivorLoader
    {
        public static void WriteTextToFile(string text, string targetFileName)
        {
            // Write the file content to the app data directory  
            string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);
            using FileStream outputStream = System.IO.File.OpenWrite(targetFile);
            using StreamWriter streamWriter = new StreamWriter(outputStream);
            streamWriter.Write(text);
        }

        public static string ReadTextFile(string targetFileName)
        {
            string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);
            using FileStream InputStream = System.IO.File.OpenRead(targetFile);
            using StreamReader reader = new StreamReader(InputStream);
            return reader.ReadToEnd();
        }
    }
}
