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
        public async Task<IActionResult> UploadJournal([FromBody] JournalUploadDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var journal = new Journal
            {
                Content = request.Content,
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
}
