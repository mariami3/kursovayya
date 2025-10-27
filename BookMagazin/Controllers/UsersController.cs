using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookMagazin.Models;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace BookMagazin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly BookMagazinContext _context;

        public UsersController(BookMagazinContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users
                .Include(u => u.Role)
                .ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            return user;
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User updatedUser)
        {
            if (id != updatedUser.IdUser)
                return BadRequest();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.LoginUser = updatedUser.LoginUser;
            user.Email = updatedUser.Email;
            user.RoleId = updatedUser.RoleId;

            if (!string.IsNullOrWhiteSpace(updatedUser.PasswordUser))
            {
                user.PasswordUser = updatedUser.PasswordUser;
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(u => u.IdUser == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }


        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.LoginUser) || string.IsNullOrWhiteSpace(user.PasswordUser))
                return BadRequest("Логин и пароль обязательны");

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.PasswordUser));
            user.PasswordUser = Convert.ToBase64String(hashBytes);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.IdUser }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Аутентификация
        [HttpPost("authenticate")]
        public async Task<ActionResult<User>> Authenticate([FromBody] LoginModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
                return BadRequest("Логин и пароль обязательны");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.LoginUser == model.Username);

            if (user == null)
                return Unauthorized();

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
            var hashedInput = Convert.ToBase64String(hashBytes);

            if (user.PasswordUser != hashedInput)
                return Unauthorized();

            return Ok(user);
        }

        // Регистрация
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] User user)
        {
            if (string.IsNullOrWhiteSpace(user.LoginUser) || string.IsNullOrWhiteSpace(user.PasswordUser))
                return BadRequest("Логин и пароль обязательны");

            if (user.PasswordUser.Length < 6 || !user.PasswordUser.Any(ch => !char.IsLetterOrDigit(ch)))
                return BadRequest("Пароль должен быть не менее 6 символов и содержать хотя бы один специальный символ.");

            if (await _context.Users.AnyAsync(u => u.LoginUser == user.LoginUser))
                return Conflict("Пользователь с таким логином уже существует");

            user.RoleId = 3; // по умолчанию — обычный пользователь

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.PasswordUser));
            user.PasswordUser = Convert.ToBase64String(hashBytes);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        // Восстановление пароля
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.LoginUser == login);
            if (user == null)
                return NotFound("Пользователь не найден");

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            user.ResetToken = token;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _context.SaveChangesAsync();

            var resetLink = $"http://localhost:5000/Account/ResetPassword?token={Uri.EscapeDataString(token)}";

            await SendEmailAsync(user.Email, "Восстановление пароля", $"Перейдите по ссылке для сброса пароля: {resetLink}");

            return Ok("Ссылка на восстановление отправлена");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Token) || string.IsNullOrWhiteSpace(model.NewPassword))
                return BadRequest("Токен и новый пароль обязательны.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == model.Token && u.ResetTokenExpiry > DateTime.UtcNow);
            if (user == null)
                return BadRequest("Недействительный или просроченный токен.");

            if (model.NewPassword.Length < 6 || !model.NewPassword.Any(ch => !char.IsLetterOrDigit(ch)))
                return BadRequest("Пароль должен быть не менее 6 символов и содержать хотя бы один специальный символ.");

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(model.NewPassword));
            user.PasswordUser = Convert.ToBase64String(hashBytes);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            await _context.SaveChangesAsync();
            return Ok("Пароль успешно обновлён.");
        }

        private async Task SendEmailAsync(string to, string subject, string body)
        {
            var client = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("your_email@gmail.com", "your_app_password"),
                EnableSsl = true,
            };

            var mail = new MailMessage("your_email@gmail.com", to, subject, body);
            mail.IsBodyHtml = true;
            await client.SendMailAsync(mail);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.IdUser == id);
        }

        public class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class ResetPasswordModel
        {
            public string Token { get; set; }
            public string NewPassword { get; set; }
        }
    }
}
