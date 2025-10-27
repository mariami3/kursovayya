using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using BookMagazin.Models;

namespace BookMagazin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsApiController : ControllerBase
    {
        private readonly string _connectionString;

        public StatisticsApiController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("{period}")]
        public async Task<ActionResult<StatisticsData>> GetStatistics(string period = "month")
        {
            var stats = new StatisticsData { Period = period };

            string dateFilter = period switch
            {
                "day" => "DATEADD(DAY, -1, GETDATE())",
                "week" => "DATEADD(DAY, -7, GETDATE())",
                "month" => "DATEADD(MONTH, -1, GETDATE())",
                "year" => "DATEADD(YEAR, -1, GETDATE())",
                _ => "DATEADD(MONTH, -1, GETDATE())"
            };

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // 🔹 Топ книг
                var cmdBooks = new SqlCommand(@"
            SELECT TOP 5 b.Title, COUNT(o.ID_Order) AS Total
            FROM Orders o
            JOIN OrderItem oi ON o.ID_Order = oi.Order_ID
            JOIN Book b ON oi.Book_ID = b.ID_Book
            WHERE o.Date >= " + dateFilter + @"
            GROUP BY b.Title
            ORDER BY Total DESC", connection);

                using (var reader = await cmdBooks.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        stats.TopBooks.Add(reader.GetString(0));
                        stats.TopBooksCount.Add(reader.GetInt32(1));
                    }
                }

                // 🔹 Топ жанров
                var cmdGenres = new SqlCommand(@"
            SELECT TOP 5 g.NameGenre, COUNT(o.ID_Order) AS Total
            FROM Orders o
            JOIN OrderItem oi ON o.ID_Order = oi.Order_ID
            JOIN Book b ON oi.Book_ID = b.ID_Book
            JOIN Genre g ON b.Genre_ID = g.ID_Genre
            WHERE o.Date >= " + dateFilter + @"
            GROUP BY g.NameGenre
            ORDER BY Total DESC", connection);

                using (var reader = await cmdGenres.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        stats.TopGenres.Add(reader.GetString(0));
                        stats.TopGenresCount.Add(reader.GetInt32(1));
                    }
                }

                // 🔹 Топ авторов
                var cmdAuthors = new SqlCommand(@"
            SELECT TOP 5 a.NameAuthor, COUNT(o.ID_Order) AS Total
            FROM Orders o
            JOIN OrderItem oi ON o.ID_Order = oi.Order_ID
            JOIN Book b ON oi.Book_ID = b.ID_Book
            JOIN Author a ON b.Author_ID = a.ID_Author
            WHERE o.Date >= " + dateFilter + @"
            GROUP BY a.NameAuthor
            ORDER BY Total DESC", connection);

                using (var reader = await cmdAuthors.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        stats.TopAuthors.Add(reader.GetString(0));
                        stats.TopAuthorsCount.Add(reader.GetInt32(1));
                    }
                }

                // 🔹 Количество заказов за неделю и месяц
                var cmdOrdersWeek = new SqlCommand(
                    "SELECT COUNT(*) FROM Orders WHERE Date >= DATEADD(DAY, -7, GETDATE())", connection);
                stats.OrdersLastWeek = (int)await cmdOrdersWeek.ExecuteScalarAsync();

                var cmdOrdersMonth = new SqlCommand(
                    "SELECT COUNT(*) FROM Orders WHERE Date >= DATEADD(MONTH, -1, GETDATE())", connection);
                stats.OrdersLastMonth = (int)await cmdOrdersMonth.ExecuteScalarAsync();

               
            }

            return Ok(stats);
        }

    }
}
