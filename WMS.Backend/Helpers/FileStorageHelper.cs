using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WMS.Share.DTOs;
using WMS.Share.Models.Location;

namespace WMS.Backend.Helpers
{
    public class FileStorageHelper
    {
        public static string? FilePath { get; set; }
        public static async Task<dynamic> RegisterFilesAsync(dynamic obj,IFileStorage fileStorage)
        {   
            if (FilePath is not null) 
            {
                if (!Directory.Exists(FilePath)) 
                {
                    Directory.CreateDirectory(FilePath);
                }
                if (obj.Attachment1 is not null)
                {
                    obj.Attachment1 = await fileStorage.SaveLocalFilesAsync(obj.Attachment1, GetExtension(obj.Attachment1Name!), FilePath!);
                }
                if (obj.Attachment2 is not null)
                {
                    obj.Attachment2 = await fileStorage.SaveLocalFilesAsync(obj.Attachment2, GetExtension(obj.Attachment2Name!), FilePath!);
                }
                if (obj.Attachment3 is not null)
                {
                    obj.Attachment3 = await fileStorage.SaveLocalFilesAsync(obj.Attachment3, GetExtension(obj.Attachment3Name!), FilePath!);
                }
                if (obj.Attachment4 is not null)
                {
                    obj.Attachment4 = await fileStorage.SaveLocalFilesAsync(obj.Attachment4, GetExtension(obj.Attachment4Name!), FilePath!);
                }
                if (obj.Attachment5 is not null)
                {
                    obj.Attachment5 = await fileStorage.SaveLocalFilesAsync(obj.Attachment5, GetExtension(obj.Attachment5Name!), FilePath!);
                }
            }

            return obj;
        }

        private static string GetExtension(string file) 
        {
            var extensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            string fileExtension = Path.GetExtension(file);
            return extensions.Contains(fileExtension)?fileExtension:string.Empty;
        }
    }
}
