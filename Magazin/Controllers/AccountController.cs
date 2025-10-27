using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Net.Mail;
using System.Net.Sockets;
using System.Net;
using System.Net.Http.Json;
using Magazin.Models;

namespace Magazin.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BookMagazinApi");
        }

        // GET: Autorization (отображение формы входа)
        [HttpGet]
        public IActionResult Autorization()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home"); // если уже авторизован → на главную
            }

            return View("Autorization");
        }


        // POST: Autorization (обработка логина)
        [HttpPost]
        public async Task<IActionResult> Autorization(string loginUser, string passwordUser)
        {
            if (string.IsNullOrEmpty(loginUser) || string.IsNullOrEmpty(passwordUser))
            {
                ModelState.AddModelError("", "Введите логин и пароль.");
                return View("Autorization");
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/users/authenticate", new { Username = loginUser, Password = passwordUser });
                var text = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", $"Ошибка: {response.StatusCode} — {text}");
                    return View("Autorization");
                }


                var user = await response.Content.ReadFromJsonAsync<User>();
                if (user == null)
                {
                    ModelState.AddModelError("", "Ошибка загрузки данных пользователя.");
                    return View("Autorization");
                }

                // Получаем роль
                var roleResponse = await _httpClient.GetAsync($"/api/roles/{user.RoleId}");
                string roleName = "Пользователь";
                if (roleResponse.IsSuccessStatusCode)
                {
                    var role = await roleResponse.Content.ReadFromJsonAsync<Role>();
                    if (role != null)
                        roleName = role.NameRole;
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.LoginUser),
                    new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
                    new Claim(ClaimTypes.Role, roleName),
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ошибка входа: " + ex.Message);
                return View("Autorization");
            }
        }

        // GET: Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        public async Task<IActionResult> Register(User user, string confirmPassword)
        {
            if (user.PasswordUser != confirmPassword)
            {
                ModelState.AddModelError("", "Пароли не совпадают.");
            }

            if (string.IsNullOrWhiteSpace(user.PasswordUser) || user.PasswordUser.Length < 6 || !user.PasswordUser.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                ModelState.AddModelError("", "Пароль должен быть не менее 6 символов и содержать хотя бы один спецсимвол.");
            }

            if (string.IsNullOrWhiteSpace(user.Email) || !IsEmailValid(user.Email))
            {
                ModelState.AddModelError("", "Некорректный email.");
            }

            if (!ModelState.IsValid)
                return View(user);

            var response = await _httpClient.PostAsJsonAsync("/api/users/register", user);
            var responseText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Autorization");

            ModelState.AddModelError("", $"Ошибка регистрации: {response.StatusCode} — {responseText}");
            return View(user);
        }

        // Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Autorization");
        }

        // Проверка email
        private bool IsEmailValid(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                string domain = addr.Host;
                var entry = Dns.GetHostEntry(domain);

                return entry.AddressList.Any(ip =>
                    ip.AddressFamily == AddressFamily.InterNetwork ||
                    ip.AddressFamily == AddressFamily.InterNetworkV6);
            }
            catch
            {
                return false;
            }
        }

        // GET: ForgotPassword (форма для ввода email)
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: ForgotPassword (отправка email в API)
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("", "Введите email.");
                return View();
            }

            var response = await _httpClient.PostAsJsonAsync("/api/users/forgot-password", email);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Ссылка на восстановление отправлена на вашу почту.";
                return RedirectToAction("Autorization");
            }

            ModelState.AddModelError("", "Ошибка: " + await response.Content.ReadAsStringAsync());
            return View();
        }

        // GET: ResetPassword (форма для сброса пароля)
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Недействительный токен.");
            }

            ViewBag.Token = token;
            return View();
        }

        // POST: ResetPassword (отправка нового пароля в API)
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string token, string passwordUser, string confirmPassword)
        {
            if (passwordUser != confirmPassword)
            {
                ModelState.AddModelError("", "Пароли не совпадают.");
                ViewBag.Token = token;
                return View();
            }

            var model = new { Token = token, NewPassword = passwordUser };
            var response = await _httpClient.PostAsJsonAsync("/api/users/reset-password", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Пароль успешно изменён. Войдите в систему.";
                return RedirectToAction("Autorization");
            }

            ModelState.AddModelError("", "Ошибка: " + await response.Content.ReadAsStringAsync());
            ViewBag.Token = token;
            return View();
        }

    }
}
