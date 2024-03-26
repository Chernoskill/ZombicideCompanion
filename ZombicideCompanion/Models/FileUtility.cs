using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Storage;
using System.IO;

namespace ZombicideCompanion.Models
{
    public static class FileUtility
    {
        public static async Task SaveFile(string str, string filePath)
        {
            Console.WriteLine($"[FileUtility/SaveFile] string: \"{str}\", filePath: \"{filePath}\".");
            using FileStream outputStream = File.OpenWrite(filePath);
            using StreamWriter streamWriter = new StreamWriter(outputStream);

            await streamWriter.WriteAsync(str);

            //Console.WriteLine($"[FileUtility/SaveFile] Write string: {str}");
        }

        public static async Task<string> LoadFile(string filePath)
        {
            Console.WriteLine($"[FileUtility/LoadFile] filePath: \"{filePath}\".");
            try
            {
                using Stream fileStream = File.OpenRead(filePath);
                using StreamReader reader = new StreamReader(fileStream);
                var c = await reader.ReadToEndAsync();
                //Console.WriteLine($"[FileUtility/LoadFile] Loaded File: {c}");
                return c;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return string.Empty;
        }

        public static async Task<Folder?> PickFolder(CancellationToken cancellationToken)
        {
            try
            {
                var result = await FolderPicker.Default.PickAsync("initial-path", cancellationToken);
                result.EnsureSuccess();
                return result.Folder;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MainPage/PickFolder] Folder not picked. {ex.Message}");
                return null;
            }
        }

        public static async Task<string?> PickFile()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync();
                return result!.FullPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MainPage/PickFile] File not picked. {ex.Message}");
                return null;
            }
        }
    }
}
