using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KDM.Filters;

namespace KDM.Controllers
{
    //[KDMActionFilter]
    public class NoticeController : Controller
    {
        KDMDB db = new KDMDB();
        // GET: Notice
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase File, string FileNote)
        {
            if (File != null)
            {
                var lastFile = db.tbl_file_details.ToList().LastOrDefault();
                int lastId = 0;
                if (lastFile != null)
                {
                    lastId = lastFile.ID + 1;
                }
                else
                {
                    lastId = 1;
                }

                tbl_file_details details = new tbl_file_details();
                details.ID = lastId;
                details.FileNote = FileNote;
                details.FileName = lastId + "_" + File.FileName;
                details.UploadDate = DateTime.Now;
                details.UploadBy = User.Identity.Name;

                db.tbl_file_details.Add(details);
                db.SaveChanges();

                string path = Server.MapPath("~/Content/NoticeFile/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                File.SaveAs(path + Path.GetFileName(lastId + "_" + File.FileName));
                ViewBag.Message = "File uploaded successfully.";
            }
            return View();
        }

        public ActionResult FileList()
        {
            var fileList = db.tbl_file_details.OrderByDescending(x => x.UploadDate).ToList();
            return View(fileList);
        }

        public ActionResult ViewFile(string fileName)
        {
            string filePath = "~/Content/NoticeFile/" + fileName;
            string extension = Path.GetExtension(filePath);
            if (extension == ".pdf")
            {
                return File(filePath, "application/pdf");
            }
            else if (extension == ".JPEG")
            {
                return File(filePath, "image/jpeg");
            }
            else if (extension == ".JPG")
            {
                return File(filePath, "image/jpeg");
            }
            else
            {
                return File(filePath, "text/html");
            }
        }

        public ActionResult DownloadFile(string fileName)
        {
            string filePath = "~/Content/NoticeFile/" + fileName;
            string fullName = Server.MapPath(filePath);
            byte[] fileBytes = GetFile(fullName);
            return File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public ActionResult DeleteFile(string fileName)
        {
            var getFile = db.tbl_file_details.FirstOrDefault(x => x.FileName == fileName);
            if (getFile != null)
            {
                string filePath = Request.MapPath("~/Content/NoticeFile/" + fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                db.tbl_file_details.Remove(getFile);
                db.SaveChanges();
            }

            return RedirectToAction("FileList");
        }


        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }

    }
}