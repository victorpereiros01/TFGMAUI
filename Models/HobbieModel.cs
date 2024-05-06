namespace TFGMaui.Models
{
    internal class Page
    {
        public int PageC { get; set; }
        public List<MovieModel> Results { get; set; }
        public int Total_pages { get; set; }
        public int Total_results { get; set; }
    }

    public class HobbieModel
    {
        public string Imagen { get; set; }

        public bool IsChecked { get; set; }

        public string NombreHobbie { get; set; }
    }
}
