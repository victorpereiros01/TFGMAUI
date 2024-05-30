using Newtonsoft.Json;

namespace TFGMaui.Models
{
    internal class PageM
    {
        public int PageC { get; set; }
        public int Total_pages { get; set; }
        public int Total_results { get; set; }
        public List<MovieModel> Results { get; set; }
    }

    internal class PageAM
    {
        public Pagination Pagination { get; set; }
        public List<AnimeModel> Data { get; set; }
    }

    internal class PageB
    {
        public string Kind { get; set; }
        public int PageNumber { get; set; }
        public int TotalItems { get; set; }
        public List<BookModel> Items { get; set; }
    }

    internal class PageG
    {
        public int Total { get; set; }
        public int Pages { get; set; }
        public List<GameModel> Items { get; set; }
    }

    internal class Pagination
    {
        [JsonProperty("last_visible_page")]
        public int LastVisiblePage { get; set; }

        [JsonProperty("has_next_page")]
        public bool HasNextPage { get; set; }

        [JsonProperty("current_page")]
        public int CurrentPage { get; set; }

        public Items Items { get; set; }
    }

    internal class Items
    {
        public int Count { get; set; }

        public int Total { get; set; }

        [JsonProperty("per_page")]
        public int PerPage { get; set; }
    }

    public class HobbieModel
    {
        public string Imagen { get; set; }

        public bool IsChecked { get; set; }

        public string NombreHobbie { get; set; }
    }
}
