using Common.Utilities;
using System;
using System.IO;
using System.Text;

namespace WebFramework
{
    public static class ErrorHandlerStartApp
    {
        public static void WriteInFile(Exception ex)
        {
            string errorFolder = GetErrorFolder();
            WriteInFile(ex, errorFolder);
        }

        private static void WriteInFile(Exception ex, string errorFolder)
        {
            var fileError = Path.Combine(errorFolder, $"errorStart{DateTime.Now.ToString().Replace("/","_").Replace(":","_")}.json");
            using (FileStream fs = File.Create(fileError))
            {
                Byte[] title = new UTF8Encoding(true).GetBytes(ex.ToJson());
                fs.Write(title, 0, title.Length);
            }
        }

        private static string GetErrorFolder()
        {
            var errorFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Errors");
            if (!Directory.Exists(errorFolder))
                Directory.CreateDirectory(errorFolder);
            return errorFolder;
        }
    }
}
