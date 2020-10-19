using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace KDM.Helpers
{
    public class FileHelper
    {
        public static string GetFileExt(string contentType)
        {
            string fileExt = "";
            switch (contentType)
            {
                case "image/gif":
                    fileExt = "gif";
                    break;
                case "image/jpeg":
                    fileExt = "jpg";
                    break;
                case "image/png":
                    fileExt = "png";
                    break;
                case "image/bmp":
                    fileExt = "bmp";
                    break;
            }

            return fileExt;
        }

        public static string SaveFile(HttpPostedFileBase file)
        {
            string fileName = "";
            string fileType = "";
            string fileRelativePath = "";
            string filePath = "";

            if (file != null)
            {
                fileName = Guid.NewGuid().ToString();
                fileType = GetFileExt(file.ContentType);
                fileName = fileName + "." + fileType;
                fileRelativePath = Path.Combine("~/Content/ImageData", fileName);
                filePath = HttpContext.Current.Server.MapPath(fileRelativePath);
                file.SaveAs(filePath);
            }
            else
            {
                fileName = "imagenotavailable";
                fileType = "png";
                fileName = fileName + "." + fileType;
                fileRelativePath = Path.Combine("~/Content/Images", fileName);
            }

            return fileRelativePath;
        }

        public static bool DeleteFile(string id)
        {
            bool ret = false;
            using (KDMDB db = new KDMDB())
            {
                Int64 fileId = Convert.ToInt64(id);
                var file = (from rw in db.tbl_file
                            where rw.file_id == fileId
                            select rw).FirstOrDefault();

                if (file != null)
                {
                    db.tbl_file.Remove(file);
                    db.SaveChanges();

                    if (File.Exists(file.file_url))
                        File.Delete(file.file_url);
                }
            }

            return ret;
        }
    }
}