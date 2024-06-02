using CommunityToolkit.Mvvm.ComponentModel;

namespace TFGMaui.Models
{
    public class SavedHobbieModel
    {
        public string HobbieType { get; set; }

        public string Value { get; set; }

        public string Imagen { get; set; }

        public string Title { get; set; }

        public DateTime Date { get; set; }

        public double? InternalScore { get; set; }

        public string? Review { get; set; }

        public DateTime DateBegin { get; set; }

        public DateTime DateEnd { get; set; }

        public Color Color { get; set; }
    }
}