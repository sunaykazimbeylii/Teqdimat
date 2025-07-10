namespace TravelFinalProject.Utilities
{
    public static class FileValidator
    {
        public static bool ValidateType(this IFormFile file, string type)
        {
            return file.ContentType.Contains(type);
        }
        public static bool ValidateSize(this IFormFile file, FileSize fileSize, int size)
        {
            switch (fileSize)
            {
                case FileSize.KB:
                    return file.Length <= size * 1024;
                case FileSize.MB:
                    return file.Length <= size * 1024 * 1024;
                case FileSize.GB:
                    return file.Length <= size * 1024 * 1024 * 1024;
            }
            return false;
        }

        public static async Task<string> CreateFileAsync(this IFormFile file, params string[] roots)
        {
            string fileName = string.Concat(Guid.NewGuid().ToString(), file.FileName.Substring(file.FileName.LastIndexOf('.')));
            string path = _getPath(roots);
            path = Path.Combine(path, fileName);
            using (FileStream fl = new FileStream(path, FileMode.Create))
            {

                await file.CopyToAsync(fl);
            }


            return fileName;

        }
        public static void DeleteFile(this string fileName, params string[] roots)
        {
            string path = _getPath(roots);
            path = Path.Combine(path, fileName);
            if (File.Exists(path))
            {

                File.Delete(path);
            }

        }
        private static string _getPath(params string[] roots)
        {
            string path = string.Empty;
            for (int i = 0; i < roots.Length; i++)
            {
                path = Path.Combine(path, roots[i]);
            }
            return path;
        }
    }
}
