using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoachClone.Api.Data;
using CoachClone.Api.Models;
using System.Security.Claims;

namespace CoachClone.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JournalController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JournalController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadJournal([FromForm] JournalUploadFileDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var file = request.File;

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest("File too large (max 5 MB).");

            if (file.ContentType != "application/pdf" && file.ContentType != "text/plain")
                return BadRequest("Only PDF or text files are allowed.");

            string content;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                content = await reader.ReadToEndAsync();
            }

            var journal = new Journal
            {
                Content = content,
                UserId = int.Parse(userId)
            };

            _context.Journals.Add(journal);
            await _context.SaveChangesAsync();

            return Ok("Journal uploaded!");
        }


    }

    public class JournalUploadDto
    {
        public string Content { get; set; }
    }

    public class JournalUploadFileDto
    {
        public IFormFile File { get; set; }
    }
}
