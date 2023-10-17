using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.StaticFiles;
using System.Runtime.CompilerServices;

namespace CityInfo.API.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : Controller
    {
        private readonly FileExtensionContentTypeProvider _fileProvider;
        public FilesController(FileExtensionContentTypeProvider fileProvider)
        {
            _fileProvider = fileProvider ?? throw new System.ArgumentNullException(nameof(fileProvider));
        }
        [HttpGet("{fileId}")]
        public ActionResult GetFile(string fileId)
        {
            var pathToFile = "TestFile.pdf";

            if (!System.IO.File.Exists(pathToFile))
            {
                return NotFound();
            }

            //This will determine the files content type and if the content type cannot be
            //determined the a default of "application/octet-stream" is given
            if (!_fileProvider.TryGetContentType(pathToFile, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = System.IO.File.ReadAllBytes(pathToFile);
            return File(bytes, contentType, Path.GetFileName(pathToFile));
        }


    }
}
