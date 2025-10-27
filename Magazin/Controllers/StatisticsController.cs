using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Magazin.Models;
using System.Net.Http.Json;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using OfficeOpenXml.Drawing.Chart;


namespace Magazin.Controllers
{
    [Authorize(Roles = "Админ")]
    public class StatisticsController : Controller
    {
        private readonly HttpClient _httpClient;

        public StatisticsController()
        {
            _httpClient = new HttpClient();
        }

        public async Task<IActionResult> Index(string period = "month")
        {
            var apiUrl = $"https://localhost:7176/api/StatisticsApi/{period}";

            try
            {
                var response = await _httpClient.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                    return View("Error", new ErrorViewModel { RequestId = "Ошибка при получении статистики" });

                var stats = await response.Content.ReadFromJsonAsync<StatisticsData>();
                return View(stats);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { RequestId = $"Ошибка: {ex.Message}" });
            }
        }
        public async Task<IActionResult> ExportToExcel(string period = "month")
        {
            ExcelPackage.License.SetNonCommercialPersonal("Мариами Петриашвили");


            var apiUrl = $"https://localhost:7176/api/StatisticsApi/{period}";
            var response = await _httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
                return BadRequest("Не удалось получить статистику");

            var stats = await response.Content.ReadFromJsonAsync<StatisticsData>();

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Статистика");

            ws.Cells["A1"].Value = $"📊 Статистика за период: {period}";
            ws.Cells["A1"].Style.Font.Bold = true;
            ws.Cells["A1"].Style.Font.Size = 14;

            int row = 3;

            void AddSection(string title, List<string> names, List<int> values, string chartTitle, string colorHex)
            {
                ws.Cells[row, 1].Value = title;
                ws.Cells[row, 1].Style.Font.Bold = true;
                ws.Cells[row, 1].Style.Font.Size = 12;
                row++;

                ws.Cells[row, 1].Value = "Название";
                ws.Cells[row, 2].Value = "Количество";
                ws.Cells[row, 1, row, 2].Style.Font.Bold = true;
                ws.Cells[row, 1, row, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[row, 1, row, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                row++;

                int startDataRow = row;

                for (int i = 0; i < names.Count; i++)
                {
                    ws.Cells[row, 1].Value = names[i];
                    ws.Cells[row, 2].Value = values[i];
                    row++;
                }

                int endDataRow = row - 1;

                // ✅ Добавляем график под таблицей
                var chart = ws.Drawings.AddChart(chartTitle, eChartType.ColumnClustered);
                chart.Title.Text = chartTitle;
                chart.SetPosition(endDataRow, 0, 0, 0);
                chart.SetSize(500, 230);

                var serie = chart.Series.Add($"B{startDataRow}:B{endDataRow}", $"A{startDataRow}:A{endDataRow}");
                serie.Header = chartTitle;

                // ✅ Универсально включаем подписи значений
                if (serie is ExcelBarChartSerie barSerie)
                {
                    barSerie.DataLabel.ShowValue = true;
                }

                chart.Style = eChartStyle.Style18;
                chart.XAxis.Title.Text = "Название";
                chart.YAxis.Title.Text = "Количество";
                chart.Legend.Remove();

                row += 12;
            }

            AddSection("📚 Топ книг", stats.TopBooks, stats.TopBooksCount, "Популярные книги", "#0d6efd");
            AddSection("🎭 Топ жанров", stats.TopGenres, stats.TopGenresCount, "Популярные жанры", "#198754");
            AddSection("✍️ Топ авторов", stats.TopAuthors, stats.TopAuthorsCount, "Популярные авторы", "#dc3545");

            ws.Cells[row, 1].Value = "Заказы за неделю";
            ws.Cells[row, 2].Value = stats.OrdersLastWeek;
            row++;
            ws.Cells[row, 1].Value = "Заказы за месяц";
            ws.Cells[row, 2].Value = stats.OrdersLastMonth;

            ws.Cells.AutoFitColumns();

            var stream = new MemoryStream(package.GetAsByteArray());
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Статистика_{period}.xlsx");
        }
    }
}