using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WMS.Share.DTOs;
using WMS.Share.Models.Location;

namespace WMS.Backend.Helpers
{
    public class WineryHelper
    {
        public static string? FilePath { get; set; }
        public static async Task<Winery> RegisterFilesAsync(Winery winery, IFileStorage fileStorage) 
        {
            if (FilePath is not null) 
            {
                if (!Directory.Exists(FilePath)) 
                {
                    Directory.CreateDirectory(FilePath);
                }
                if (winery.File1 is not null)
                {
                    winery.File1 = await fileStorage.SaveLocalFilesAsync(winery.File1, GetExtension(winery.FileName1!), FilePath!);
                }
                if (winery.File2 is not null)
                {
                    winery.File2 = await fileStorage.SaveLocalFilesAsync(winery.File2, GetExtension(winery.FileName2!), FilePath!);
                }
                if (winery.File3 is not null)
                {
                    winery.File3 = await fileStorage.SaveLocalFilesAsync(winery.File3, GetExtension(winery.FileName3!), FilePath!);
                }
                if (winery.File4 is not null)
                {
                    winery.File4 = await fileStorage.SaveLocalFilesAsync(winery.File4, GetExtension(winery.FileName4!), FilePath!);
                }
                if (winery.File5 is not null)
                {
                    winery.File5 = await fileStorage.SaveLocalFilesAsync(winery.File5, GetExtension(winery.FileName5!), FilePath!);
                }
            }

            return winery;
        }

        private static string GetExtension(string file) 
        {
            var extensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            string fileExtension = Path.GetExtension(file);
            return extensions.Contains(fileExtension)?fileExtension:string.Empty;
        }
    }
}
