using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web;
using ToyStoreOnlineWeb.Data;
using ToyStoreOnlineWeb.Models;

namespace ToyStoreOnlineWeb.Service
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file);
    }

    public class FileStorageService : IFileStorageService
    {
        private readonly string _fileStoragePath;

        public FileStorageService(string fileStoragePath)
        {
            _fileStoragePath = fileStoragePath;
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var uniqueFileName = GetUniqueFileName(file.FileName);
                var filePath = Path.Combine(_fileStoragePath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return uniqueFileName;
            }

            return null;
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                   + "_"
                   + Guid.NewGuid().ToString().Substring(0, 4)
                   + Path.GetExtension(fileName);
        }
    }

}
