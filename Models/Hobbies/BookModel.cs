using Newtonsoft.Json;

namespace TFGMaui.Models
{
    public class BookModel : HobbieModel
    {
        public string Id { get; set; }  // v

        public string SelfLink { get; set; }    // v

        public VolumeInfo VolumeInfo { get; set; }
    }

    public class VolumeInfo
    {
        public string Title { get; set; }   // v

        public string Subtitle { get; set; }    // v

        public List<string> Authors { get; set; }   // v

        public string Publisher { get; set; }   // v

        public string PublishedDate { get; set; }   // v

        public string Description { get; set; } // v

        public int PageCount { get; set; }  // v

        public int PrintedPageCount { get; set; }   // v

        [JsonIgnore]
        public string Categories { get; set; }  // v

        public double AverageRating { get; set; }  // v

        public string MaturityRating { get; set; }  // v

        public ImageLinks ImageLinks { get; set; }  // v
    }

    public class ImageLinks
    {
        public string Thumbnail;
    }
}
