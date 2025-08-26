
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.Controllers
{
    [Route("[controller]")]
    public class DownloadController : Controller
    {
        private readonly ILogger<DownloadController> _logger;

        public DownloadController(ILogger<DownloadController> logger)
        {
            _logger = logger;
        }

        [HttpPost("UploadImage")]
        public IActionResult UploadImage(IFormFile files)
        {
            if (files == null || files.Length == 0)
                return BadRequest("No file uploaded.");

            var imageName = $"{Guid.NewGuid()}{Path.GetExtension(files.FileName)}"; // Nombre único

            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp-images")))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp-images"));
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp-images", imageName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                files.CopyTo(stream);
            }

            var publicUrl = $"{Request.Scheme}://{Request.Host}/temp-images/{imageName}";
            return Ok(new { url = publicUrl });
        }

        [HttpGet("temp-files/{fileName}")]
        public IActionResult GetTempFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest("File name is missing.");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp-images", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            System.IO.File.Delete(filePath);

            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return File(fileBytes, contentType, fileName);
        }
    }
}