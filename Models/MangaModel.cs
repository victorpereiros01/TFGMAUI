using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFGMaui.Models
{
    public class MangaModel
    {
        public int Mal_id { get; set; } // -

        public class Images
        {
            private class Jpg
            {
                public string Image_url { get; set; }
            }
        }

        public Images Imagen { get; set; }  // -

        public string Title { get; set; }   // -

        public string Type { get; set; }    // -

        public string Status { get; set; }  // -

        public string Rank { get; set; }    // -

        public class Genre
        {
            public int Mal_Id { get; set; }
            public string Type { get; set; }
            public string Name { get; set; }
        }

        public class PublishedIn
        {
            public string String { get; set; }
        }

        public PublishedIn Aired { get; set; } // -

        public class Author
        {
            public int Mal_Id { get; set; }
            public string Type { get; set; }
            public string Name { get; set; }
        }

        public List<Author> Authors { get; set; }

        public List<Genre> Genres { get; set; } // -

        public class Demography
        {
            public int Mal_Id { get; set; }
            public string Type { get; set; }
            public string Name { get; set; }
        }

        public List<Demography> Demographics { get; set; }   // -

        public double Score { get; set; }   // -

        public string Synopsis { get; set; }    // -
    }
}
