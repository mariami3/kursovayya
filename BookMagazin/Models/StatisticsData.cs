namespace BookMagazin.Models
{
    public class StatisticsData
    {
        public string Period { get; set; } = "month";

        public List<string> TopBooks { get; set; } = new();
        public List<int> TopBooksCount { get; set; } = new();

        public List<string> TopGenres { get; set; } = new();
        public List<int> TopGenresCount { get; set; } = new();

        public List<string> TopAuthors { get; set; } = new();
        public List<int> TopAuthorsCount { get; set; } = new();

        public int OrdersLastWeek { get; set; } 
        public int OrdersLastMonth { get; set; }
        public int NewUsersLastWeek { get; set; }
        public int NewUsersLastMonth { get; set; }
    }
}