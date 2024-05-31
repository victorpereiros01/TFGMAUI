namespace TFGMaui.Models
{
    public class SavedHobbieModel
    {
        public int IdAdded { get; set; }

        public string HobbieType { get; set; }

        public string Value { get; set; }

        public int IdUser { get; set; }

        public string Imagen { get; set; }

        public string Title { get; set; }

        public DateTime Date { get; set; }

        public double? InternalScore { get; set; }

        public string? Review { get; set; }

        public DateTime DateBegin { get; set; }

        public DateTime DateEnd { get; set; }
    }
}