using Microsoft.AspNetCore.Mvc; // Importerar stöd för API controllers
using Microsoft.EntityFrameworkCore; // För databasfrågor via EF
using CoachClone.Api.Data; // Din databascontext
using CoachClone.Api.Models; // Dina modeller
using System.Security.Cryptography; // För att kunna hash:a lösenord
using System.Text; // För att konvertera strängar till byte-array
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt; // För JWT-tokenhantering
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration; // För att läsa konfigurationer som JWT-nyckel
using Microsoft.AspNetCore.Authorization;
namespace CoachClone.Api.Controllers


{
    [ApiController] // Markerar att detta är en API-controller
    [Route("api/[controller]")] // Bygger URL baserat på controller-namnet, t.ex. /api/auth
    public class AuthController : ControllerBase // Ärver från ControllerBase (API utan views)
    {
        private readonly AppDbContext _context; // Skapar en privat variabel för att nå databasen
        private readonly IConfiguration _configuration;
        public AuthController(AppDbContext context, IConfiguration configuration) // Konstruktor som tar emot databasen via Dependency Injection
        {
            _context = context;
            _configuration = configuration; // Sparar context till vår variabel
        }

        [HttpPost("register")] // Skapar en POST-endpoint på /api/auth/register
        public async Task<IActionResult> Register(UserRegisterDto request) // Metod som tar emot DTO med Email, Password och Role
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email)) // Kollar om email redan finns i databasen
                return BadRequest("Email already exists."); // Returnerar fel om emailen finns

            string hashedPw = HashPassword(request.Password); // Hashar lösenordet

            var user = new User // Skapar nytt User-objekt
            {
                Email = request.Email, // Sätter email
                PasswordHash = hashedPw, // Sparar hashat lösenord
                Role = request.Role // Sätter rollen
            };

            _context.Users.Add(user); // Lägger till användaren i databasen
            await _context.SaveChangesAsync(); // Sparar ändringar

            return Ok("User created!"); // Returnerar 200 OK
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return BadRequest("User not found.");

            string hashedPw = HashPassword(request.Password);

            if (user.PasswordHash != hashedPw)
                return BadRequest("Incorrect password.");

            string token = CreateToken(user);

            return Ok(new { token });
        }

        private string CreateToken(User user)
        {
            var secret = _configuration["Jwt:Key"];

            if (string.IsNullOrEmpty(secret))
            {
                throw new Exception("⚠️ DEBUG: Jwt:Key är NULL eller tom! Kolla miljövariabel eller appsettings!");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: new List<Claim>
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
                },
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }





        private string HashPassword(string password) // Privat metod som hash:ar ett lösenord
        {
            using var sha256 = SHA256.Create(); // Skapar SHA256-instans
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password)); // Gör om sträng till byte-array och hash:ar
            return Convert.ToBase64String(bytes); // Returnerar hash som Base64-sträng
        }

        [HttpGet("protected")]
        [Authorize]
        public ActionResult<string> Protected()
        {
            return "Du är auktoriserad! Du har skickat en giltig token!";
        }

    }

    public class UserRegisterDto // Klass som beskriver datan klienten skickar in
    {
        public string Email { get; set; } // Email
        public string Password { get; set; } // Lösenord
        public string Role { get; set; } // Roll (ex. Coach eller Client)
    }


    public class UserLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }


}
