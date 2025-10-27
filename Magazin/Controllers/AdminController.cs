using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography;
using System.Text;
using Magazin.Models;
using Magazin.Services;
using Microsoft.AspNetCore.Authorization;


namespace Magazin.Controllers
{
    [Authorize(Roles = "Админ")]
    public class AdminController : Controller
    {
        private readonly UserApiService _userApiService;
        private readonly RoleApiService _roleApiService;
        private readonly GenreApiService _genreApiService;
        private readonly AuthorApiService _authorApiService;
        private readonly BookApiService _bookApiService;
        private readonly AdaptationApiService _adaptationApiService;
        private readonly BookVoicesApiService _bookVoicesApiService;



        public AdminController(
            UserApiService userApiService,
            RoleApiService roleApiService,
            GenreApiService genreApiService,
            AuthorApiService authorApiService,
            BookApiService bookApiService,
            AdaptationApiService adaptationApiService,
            BookVoicesApiService bookVoicesApiService)
        {
            _userApiService = userApiService;
            _roleApiService = roleApiService;
            _genreApiService = genreApiService;
            _authorApiService = authorApiService;
            _bookApiService = bookApiService;
            _adaptationApiService = adaptationApiService;
            _bookVoicesApiService = bookVoicesApiService;
        }

        public IActionResult Main() => View();

        //  ЖАНРЫ 
        public async Task<IActionResult> Genres()
        {
            var genres = await _genreApiService.GetGenresAsync();
            return View(genres);
        }

        public IActionResult CreateGenre() => View();

        [HttpPost]
        public async Task<IActionResult> CreateGenre(Genre genre)
        {
            if (ModelState.IsValid)
            {
                await _genreApiService.CreateGenreAsync(genre);
                return RedirectToAction(nameof(Genres));
            }
            return View(genre);
        }

        public async Task<IActionResult> EditGenre(int id)
        {
            var genre = await _genreApiService.GetGenreByIdAsync(id);
            if (genre == null) return NotFound();
            return View(genre);
        }

        [HttpPost]
        public async Task<IActionResult> EditGenre(Genre genre)
        {
            if (ModelState.IsValid)
            {
                await _genreApiService.UpdateGenreAsync(genre.IdGenre, genre);
                return RedirectToAction(nameof(Genres));
            }
            return View(genre);
        }

        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _genreApiService.GetGenreByIdAsync(id);
            if (genre == null) return NotFound();
            return View(genre);
        }

        [HttpPost, ActionName("DeleteGenre")]
        public async Task<IActionResult> DeleteGenreConfirmed(int id)
        {
            await _genreApiService.DeleteGenreAsync(id);
            return RedirectToAction(nameof(Genres));
        }

        //  АВТОРЫ 
        public async Task<IActionResult> Authors()
        {
            var authors = await _authorApiService.GetAuthorsAsync();
            return View(authors);
        }

        public IActionResult CreateAuthor() => View();

        [HttpPost]
        public async Task<IActionResult> CreateAuthor(Author author)
        {
            if (ModelState.IsValid)
            {
                await _authorApiService.CreateAuthorAsync(author);
                return RedirectToAction(nameof(Authors));
            }
            return View(author);
        }

        public async Task<IActionResult> EditAuthor(int id)
        {
            var author = await _authorApiService.GetAuthorByIdAsync(id);
            if (author == null) return NotFound();
            return View(author);
        }

        [HttpPost]
        public async Task<IActionResult> EditAuthor(Author author)
        {
            if (ModelState.IsValid)
            {
                await _authorApiService.UpdateAuthorAsync(author.IdAuthor, author);
                return RedirectToAction(nameof(Authors));
            }
            return View(author);
        }

        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _authorApiService.GetAuthorByIdAsync(id);
            if (author == null) return NotFound();
            return View(author);
        }

        [HttpPost, ActionName("DeleteAuthor")]
        public async Task<IActionResult> DeleteAuthorConfirmed(int id)
        {
            await _authorApiService.DeleteAuthorAsync(id);
            return RedirectToAction(nameof(Authors));
        }

        //  КНИГИ 
        public async Task<IActionResult> Books()
        {
            var books = await _bookApiService.GetBooksAsync();
            return View(books);
        }

        public async Task<IActionResult> CreateBook()
        {
            var genres = await _genreApiService.GetGenresAsync();
            var authors = await _authorApiService.GetAuthorsAsync();
            ViewBag.Genres = new SelectList(genres, "IdGenre", "NameGenre");
            ViewBag.Authors = new SelectList(authors, "IdAuthor", "NameAuthor");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook(Book book)
        {
            if (ModelState.IsValid)
            {
                await _bookApiService.CreateBookAsync(book);
                return RedirectToAction(nameof(Books));
            }
            if (string.IsNullOrWhiteSpace(book.Title) || book.Price <= 0 || book.GenreId == null || book.AuthorId == null)
            {
                ModelState.AddModelError("", "Заполните все обязательные поля: Название, Цена, Жанр, Автор.");
                return View(book);
            }

            var genres = await _genreApiService.GetGenresAsync();
            var authors = await _authorApiService.GetAuthorsAsync();
            ViewBag.Genres = new SelectList(genres, "IdGenre", "NameGenre", book.GenreId);
            ViewBag.Authors = new SelectList(authors, "IdAuthor", "NameAuthor", book.AuthorId);

            return View(book);
        }

        public async Task<IActionResult> EditBook(int id)
        {
            var book = await _bookApiService.GetBookByIdAsync(id);
            if (book == null) return NotFound();

            var genres = await _genreApiService.GetGenresAsync();
            var authors = await _authorApiService.GetAuthorsAsync();
            ViewBag.Genres = new SelectList(genres, "IdGenre", "NameGenre", book.GenreId);
            ViewBag.Authors = new SelectList(authors, "IdAuthor", "NameAuthor", book.AuthorId);

            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> EditBook(Book book)
        {
            if (ModelState.IsValid)
            {
                await _bookApiService.UpdateBookAsync(book.IdBook, book);
                return RedirectToAction(nameof(Books));
            }

            var genres = await _genreApiService.GetGenresAsync();
            var authors = await _authorApiService.GetAuthorsAsync();
            ViewBag.Genres = new SelectList(genres, "IdGenre", "NameGenre", book.GenreId);
            ViewBag.Authors = new SelectList(authors, "IdAuthor", "NameAuthor", book.AuthorId);

            return View(book);
        }

        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _bookApiService.GetBookByIdAsync(id);
            if (book == null) return NotFound();
            return View(book);
        }

        [HttpPost, ActionName("DeleteBook")]
        public async Task<IActionResult> DeleteBookConfirmed(int id)
        {
            await _bookApiService.DeleteBookAsync(id);
            return RedirectToAction(nameof(Books));
        }

        //  ЭКРАНИЗАЦИИ 
        public async Task<IActionResult> Adaptations()
        {
            var adaptations = await _adaptationApiService.GetAdaptationsAsync();
            return View(adaptations);
        }

        public async Task<IActionResult> CreateAdaptation()
        {
            var books = await _bookApiService.GetBooksAsync();
            ViewBag.Books = new SelectList(books, "IdBook", "Title");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdaptation(Adaptation adaptation)
        {
            if (ModelState.IsValid)
            {
                await _adaptationApiService.CreateAdaptationAsync(adaptation);
                return RedirectToAction(nameof(Adaptations));
            }

            var books = await _bookApiService.GetBooksAsync();
            ViewBag.Books = new SelectList(books, "IdBook", "Title", adaptation.BookId);

            return View(adaptation);
        }

        public async Task<IActionResult> EditAdaptation(int id)
        {
            var adaptation = await _adaptationApiService.GetAdaptationByIdAsync(id);
            if (adaptation == null) return NotFound();

            var books = await _bookApiService.GetBooksAsync();
            ViewBag.Books = new SelectList(books, "IdBook", "Title", adaptation.BookId);

            return View(adaptation);
        }

        [HttpPost]
        public async Task<IActionResult> EditAdaptation(Adaptation adaptation)
        {
            if (ModelState.IsValid)
            {
                await _adaptationApiService.UpdateAdaptationAsync(adaptation.IdAdaptation, adaptation);
                return RedirectToAction(nameof(Adaptations));
            }

            var books = await _bookApiService.GetBooksAsync();
            ViewBag.Books = new SelectList(books, "IdBook", "Title", adaptation.BookId);

            return View(adaptation);
        }

        public async Task<IActionResult> DeleteAdaptation(int id)
        {
            var adaptation = await _adaptationApiService.GetAdaptationByIdAsync(id);
            if (adaptation == null) return NotFound();
            return View(adaptation);
        }

        [HttpPost, ActionName("DeleteAdaptation")]
        public async Task<IActionResult> DeleteAdaptationConfirmed(int id)
        {
            await _adaptationApiService.DeleteAdaptationAsync(id);
            return RedirectToAction(nameof(Adaptations));
        }

        //  ПОЛЬЗОВАТЕЛИ 
        public async Task<IActionResult> Users()
        {
            var users = await _userApiService.GetUsersAsync();
            return View(users);
        }

        public async Task<IActionResult> CreateUser()
        {
            var roles = await _roleApiService.GetRolesAsync();
            ViewBag.Roles = new SelectList(roles, "IdRole", "NameRole");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.LoginUser))
                ModelState.AddModelError("LoginUser", "Логин обязателен.");

            if (string.IsNullOrWhiteSpace(user.PasswordUser) || user.PasswordUser.Length < 6)
                ModelState.AddModelError("PasswordUser", "Пароль должен быть не короче 6 символов.");

            if (ModelState.IsValid)
            {
                using var sha256 = SHA256.Create();
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.PasswordUser));
                user.PasswordUser = Convert.ToBase64String(hashBytes);

                await _userApiService.CreateUserAsync(user);
                return RedirectToAction(nameof(Users));
            }

            var roles = await _roleApiService.GetRolesAsync();
            ViewBag.Roles = new SelectList(roles, "IdRole", "NameRole", user.RoleId);

            return View(user);
        }

        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _userApiService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _roleApiService.GetRolesAsync();
            ViewBag.Roles = new SelectList(roles, "IdRole", "NameRole", user.RoleId);

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(User user, string? newPassword)
        {
            ModelState.Remove("PasswordUser"); 

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(newPassword))
                {
                    using var sha256 = SHA256.Create();
                    var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
                    user.PasswordUser = Convert.ToBase64String(hashBytes);
                }
                else
                {
                    var existing = await _userApiService.GetUserByIdAsync(user.IdUser);
                    if (existing == null) return NotFound();
                    user.PasswordUser = existing.PasswordUser;
                }

                await _userApiService.UpdateUserAsync(user.IdUser, user);
                return RedirectToAction(nameof(Users));
            }

            var roles = await _roleApiService.GetRolesAsync();
            ViewBag.Roles = new SelectList(roles, "IdRole", "NameRole", user.RoleId);

            return View(user);
        }



        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userApiService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("DeleteUser")]
        public async Task<IActionResult> DeleteUserConfirmed(int id)
        {
            await _userApiService.DeleteUserAsync(id);
            return RedirectToAction(nameof(Users));
        }

        //  РОЛИ 
        public async Task<IActionResult> Roles()
        {
            var roles = await _roleApiService.GetRolesAsync();
            return View(roles);
        }

        public IActionResult CreateRole() => View();

        [HttpPost]
        public async Task<IActionResult> CreateRole(Role role)
        {
            if (ModelState.IsValid)
            {
                await _roleApiService.CreateRoleAsync(role);
                return RedirectToAction(nameof(Roles));
            }
            return View(role);
        }

        public async Task<IActionResult> EditRole(int id)
        {
            var role = await _roleApiService.GetRoleByIdAsync(id);
            if (role == null) return NotFound();
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(Role role)
        {
            if (ModelState.IsValid)
            {
                await _roleApiService.UpdateRoleAsync(role.IdRole, role);
                return RedirectToAction(nameof(Roles));
            }
            return View(role);
        }

        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _roleApiService.GetRoleByIdAsync(id);
            if (role == null) return NotFound();
            return View(role);
        }

        [HttpPost, ActionName("DeleteRole")]
        public async Task<IActionResult> DeleteRoleConfirmed(int id)
        {
            await _roleApiService.DeleteRoleAsync(id);
            return RedirectToAction(nameof(Roles));
        }

        //  АУДИОАННОТАЦИИ
        // Список озвучек
        public async Task<IActionResult> BookVoices()
        {
            var voices = await _bookVoicesApiService.GetBookVoicesAsync();
            return View(voices);
        }

        // Добавление озвучки
        [HttpGet]
        public async Task<IActionResult> CreateBookVoice()
        {
            var books = await _bookApiService.GetBooksAsync();
            ViewBag.Books = new SelectList(books, "IdBook", "Title");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBookVoice(BookVoice voice, IFormFile? audioFile)
        {
            if (!ModelState.IsValid)
            {
                var books = await _bookApiService.GetBooksAsync();
                ViewBag.Books = new SelectList(books, "IdBook", "Title", voice.BookId);
                return View(voice);
            }

            await _bookVoicesApiService.CreateBookVoiceAsync(voice, audioFile);
            return RedirectToAction(nameof(BookVoices));
        }

        // Редактирование озвучки
        [HttpGet]
        public async Task<IActionResult> EditBookVoice(int id)
        {
            var voice = await _bookVoicesApiService.GetBookVoiceByIdAsync(id);
            if (voice == null) return NotFound();

            var books = await _bookApiService.GetBooksAsync();
            ViewBag.Books = new SelectList(books, "IdBook", "Title", voice.BookId);
            return View(voice);
        }

        [HttpPost]
        public async Task<IActionResult> EditBookVoice(BookVoice voice, IFormFile? audioFile)
        {
            if (!ModelState.IsValid)
            {
                var books = await _bookApiService.GetBooksAsync();
                ViewBag.Books = new SelectList(books, "IdBook", "Title", voice.BookId);
                return View(voice);
            }

            await _bookVoicesApiService.UpdateBookVoiceAsync(voice.IdVoice, voice, audioFile);
            return RedirectToAction(nameof(BookVoices));
        }

        // Удаление
        [HttpPost]
        public async Task<IActionResult> DeleteBookVoice(int id)
        {
            await _bookVoicesApiService.DeleteBookVoiceAsync(id);
            return RedirectToAction(nameof(BookVoices));
        }
    }
}

