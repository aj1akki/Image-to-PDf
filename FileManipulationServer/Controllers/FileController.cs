using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf;
using System.Net.Http.Headers;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Syncfusion.Drawing;
using System.IO;

namespace FileManipulationServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [EnableCors("OpenCORSPolicy")]
    public class FileController : ControllerBase
    {
        private readonly ILogger<FileController> _logger;
        
        private readonly IHostingEnvironment hostingEnv;

      
        public FileController(ILogger<FileController> logger, IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            hostingEnv = hostingEnvironment;
        }

        [HttpPost("Save")]
        [EnableCors("OpenCORSPolicy")]
        // Upload method for normal upload
        public void UploadImages(IList<IFormFile> chunkFile, IList<IFormFile> UploadFiles)
        {
            long size = 0;
            // for normal upload
            try
            {

                foreach (var file in UploadFiles)
                {
                    var filename = ContentDispositionHeaderValue
                                    .Parse(file.ContentDisposition)
                                    .FileName
                                    .Trim('"');
                    filename = hostingEnv.WebRootPath + $@"\{filename}";
                    size += file.Length;
                    if (!System.IO.File.Exists(filename))
                    {
                        using (FileStream fs = System.IO.File.Create(filename))
                        {
                            file.CopyTo(fs);
                            fs.Flush();
                        }
                    }
                    else
                    {
                        System.IO.File.Delete(filename);
                        using (FileStream fs = System.IO.File.Create(filename))
                        {
                            file.CopyTo(fs);
                            fs.Flush();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.StatusCode = 204;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File failed to upload";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
            }
        }

        [HttpPost("Remove")]
        [EnableCors("OpenCORSPolicy")]
        // to delete uploaded chunk-file from server
        public void RemoveImages(IList<IFormFile> UploadFiles)
        {
            try
            {
                var filename = hostingEnv.WebRootPath + $@"\{UploadFiles[0].FileName}";
                if (System.IO.File.Exists(filename))
                {
                    System.IO.File.Delete(filename);
                }
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.StatusCode = 200;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File removed successfully";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
            }
        }

        [HttpPost("CleanUp")]
        [EnableCors("OpenCORSPolicy")]
        // to delete uploaded chunk-file from server
        public void CleanUp([FromBody]string[] UploadFiles)
        {
            try
            {
                foreach (var file in UploadFiles)
                {
                    var filename = hostingEnv.WebRootPath + $@"\{file}";
                    if (System.IO.File.Exists(filename))
                    {
                        System.IO.File.Delete(filename);
                    }
                }
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.StatusCode = 200;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File removed successfully";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
            }
        }

        [HttpPost("Download")]
        [EnableCors("OpenCORSPolicy")]
        [Consumes("application/json")]
        // to delete uploaded chunk-file from server
        public IActionResult DownloadPdf([FromBody] string[] fileNames)
        {
            try
            {
                //Create a new PDF document.
                PdfDocument doc = new PdfDocument();
                for (int i = 0; i < fileNames.Length; i++)
                {

                    var path = hostingEnv.WebRootPath + $@"\{fileNames[i]}";
                    if (System.IO.File.Exists(path))
                    {

                        using (FileStream fs = System.IO.File.OpenRead(path))
                        {
                            //Add a page to the document.
                            PdfPage page = doc.Pages.Add();

                            //Create PDF graphics for the page
                            PdfGraphics graphics = page.Graphics;

                            PdfBitmap image = new PdfBitmap(fs);

                            //Scale Image by maintaining aspect ratio 
                            float PageWidth = page.Graphics.ClientSize.Width;
                            float PageHeight = page.Graphics.ClientSize.Height;
                            float myWidth = image.Width;
                            float myHeight = image.Height;

                            float shrinkFactor;

                            if (myWidth > PageWidth)
                            {
                                shrinkFactor = myWidth / PageWidth;
                                myWidth = PageWidth;
                                myHeight = myHeight / shrinkFactor;
                            }

                            if (myHeight > PageHeight)
                            {
                                shrinkFactor = myHeight / PageHeight;
                                myHeight = PageHeight;
                                myWidth = myWidth / shrinkFactor;
                            }

                            float XPosition = (PageWidth - myWidth) / 2;
                            float YPosition = (PageHeight - myHeight) / 2;

                            graphics.DrawImage(image, XPosition, YPosition, myWidth, myHeight);

                            //Draw the image
                            //graphics.DrawImage(image, 0, 0, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height);

                        }
                    }
                }
                //Save the PDF document to stream
                MemoryStream stream = new MemoryStream();
                doc.Save(stream);
                //If the position is not set to '0' then the PDF will be empty.
                stream.Position = 0;
                //Close the document.
                doc.Close(true);
                //Defining the ContentType for pdf file.

                string contentType = "application/pdf";
                //Define the file name.
                string fileName = Guid.NewGuid() + ".pdf";
                //Creates a FileContentResult object by using the file contents, content type, and file name.                         
                return File(stream, contentType, fileName);

            }

            catch (Exception e)
            {
                return BadRequest();
            }
        }
    }


}
