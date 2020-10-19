using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp;

using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Web.Mvc;
using System.Web.Routing;

using Spire.Pdf;
using Spire.Pdf.HtmlConverter;
using System.Drawing;
using System.Threading;
using Spire.Pdf.Graphics;

namespace KDM.Helpers
{
    public class ReportHelper
    {
        public static byte[] GetPDF1(string pHTML)
        {
            byte[] bPDF = null;

            MemoryStream ms = new MemoryStream();
            TextReader txtReader = new StringReader(pHTML);

            // 1: create object of a itextsharp document class
            Document doc = new Document(PageSize.A4, 25, 25, 25, 25);

            // 2: we create a itextsharp pdfwriter that listens to the document and directs a XML-stream to a file
            PdfWriter oPdfWriter = PdfWriter.GetInstance(doc, ms);

            // 3: we create a worker parse the document
            HTMLWorker htmlWorker = new HTMLWorker(doc);

            // 4: we open document and start the worker on the document
            doc.Open();
            htmlWorker.StartDocument();

            // 5: parse the html into the document
            htmlWorker.Parse(txtReader);

            // 6: close the document and the worker
            htmlWorker.EndDocument();
            htmlWorker.Close();
            doc.Close();

            bPDF = ms.ToArray();

            return bPDF;
        }

        public static byte[] GetPDF(string html)
        {
            StringReader sr = new StringReader(html.ToString());
            byte[] data = null;
            
            Document pdfDoc = new Document(PageSize.A4, 0f, 0f, 0f, 0f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                pdfDoc.Open();

                htmlparser.Parse(sr);
                pdfDoc.Close();

                data = memoryStream.ToArray();
                memoryStream.Close();
            }

            return data;
        }

        public static Byte[] PdfSharpHtmlToPDF(String html)
        {
            string cssFile = HttpContext.Current.Server.MapPath("~/Content/Report.css");
            var cssData = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.ParseStyleSheet(cssFile, true);

            Byte[] res = null;
            using (MemoryStream ms = new MemoryStream())
            {
                TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerateConfig config = new TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerateConfig();
                config.PageOrientation = PdfSharp.PageOrientation.Portrait;
                config.PageSize = PdfSharp.PageSize.A4;
                config.MarginBottom = 20;
                var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html, config, cssData);
                pdf.Save(ms);
                res = ms.ToArray();
            }
            return res;
        }

        public static byte[] SpirePDFHtmlToPDF(string html)
        {
            byte[] ret = null;

            //using (MemoryStream ms = new MemoryStream())
            //{
            //    Spire.Pdf.HtmlConverter.Qt.HtmlConverter.Convert(
            //    html,
            //    //memory stream
            //    ms,
            //    //enable javascript
            //    true,
            //    //load timeout
            //    10 * 1000,
            //    //page size
            //    new SizeF(612, 792),
            //    //page margins
            //    new Spire.Pdf.Graphics.PdfMargins(0),
            //    //load from content type
            //    LoadHtmlType.SourceCode
            //    );
            //}

            using (MemoryStream ms = new MemoryStream())
            {
                //Create a pdf document.
                Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();

                PdfPageSettings setting = new PdfPageSettings();

                //setting.Size = new SizeF(1000, 1000);
                setting.Size = Spire.Pdf.PdfPageSize.A4;
                setting.Orientation = Spire.Pdf.PdfPageOrientation.Portrait;
                setting.Margins = new Spire.Pdf.Graphics.PdfMargins(20);

                doc.Template.Top = GetHeader(doc, setting.Margins);
                //apply blank templates to other parts of page template
                doc.Template.Bottom = new PdfPageTemplateElement(doc.PageSettings.Size.Width, setting.Margins.Bottom);
                doc.Template.Left = new PdfPageTemplateElement(setting.Margins.Left, doc.PageSettings.Size.Height);
                doc.Template.Right = new PdfPageTemplateElement(setting.Margins.Right, doc.PageSettings.Size.Height);

                PdfHtmlLayoutFormat htmlLayoutFormat = new PdfHtmlLayoutFormat();
                htmlLayoutFormat.IsWaiting = true;

                Thread thread = new Thread(() =>
                { doc.LoadFromHTML(html, true, setting, htmlLayoutFormat); });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
                
                //Save pdf file.
                doc.SaveToStream(ms);
                doc.Close();

                ret = ms.ToArray();
                
            }
                
            return ret;

        }


        private static PdfPageTemplateElement GetHeader(Spire.Pdf.PdfDocument doc, PdfMargins margins)
        {
            SizeF pageSize = doc.PageSettings.Size;

            PdfPageTemplateElement header = new PdfPageTemplateElement(pageSize.Width, margins.Top);
            header.Foreground = false;

            float x = margins.Left;
            float y = margins.Top;

            string headerFile = HttpContext.Current.Server.MapPath("~/Content/Images/report_header.png");
            Spire.Pdf.Graphics.PdfImage headerImage = Spire.Pdf.Graphics.PdfImage.FromFile(headerFile);
            float width = headerImage.Width;
            float height = headerImage.Height;

            header.Graphics.DrawImage(headerImage, x, margins.Top - height - 2, width, height);

            return header;
        }


        public static void ResponsePDF(ControllerContext context, string viewName, object model=null)
        {
            string htmlContent = RenderViewToString(context, viewName, model);
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/pdf";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + "PDFfile.pdf");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.BinaryWrite(GetPDF(htmlContent));
            HttpContext.Current.Response.End();
        }

        public static FileContentResult ResponseAsPDF(ControllerContext context, string viewName, object model = null)
        {
            string htmlContent = RenderViewToString(context, viewName, model);
            //Create a PDF Document
            //var PDF =GetPDF(htmlContent);
            //var PDF = PdfSharpHtmlToPDF(htmlContent);
            var PDF = SpirePDFHtmlToPDF(htmlContent);
            //return a  pdf document from a view
            var contentLength = PDF.Length;
               HttpContext.Current.Response.AppendHeader("Content-Length", contentLength.ToString());
            var id = Guid.NewGuid().ToString();
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "inline; filename=" + id + ".pdf");
                return new FileContentResult(PDF, "application/pdf;");
        }

        public static string RenderViewToString(ControllerContext context, string viewName, object model = null)
        {
            context.Controller.ViewData.Model = model;

            try
            {
                using (StringWriter sw = new StringWriter())
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                    ViewContext viewContext = new ViewContext(context, viewResult.View, context.Controller.ViewData, context.Controller.TempData, sw);
                    viewResult.View.Render(viewContext, sw);

                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static T CreateController<T>(RouteData routeData = null)
                where T : Controller, new()
        {
            T controller = new T();

            // Create an MVC Controller Context
            HttpContextBase wrapper = null;
            if (HttpContext.Current != null)
                wrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
            //else
            //    wrapper = CreateHttpContextBase(writer);


            if (routeData == null)
                routeData = new RouteData();

            if (!routeData.Values.ContainsKey("controller") && !routeData.Values.ContainsKey("Controller"))
                routeData.Values.Add("controller", controller.GetType().Name
                                                            .ToLower()
                                                            .Replace("controller", ""));

            controller.ControllerContext = new ControllerContext(wrapper, routeData, controller);
            return controller;
        }
    }
}