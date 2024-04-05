using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFGMaui.Models
{
    public class AnimeModel()
    {
        // - En teoria funciona
        // x Mirarlo como seria

        public int Mal_id { get; set; } // -

        public class Images { 
            private class Jpg
            {
                public string Image_url { get; set; }
            }
        }

        public Images Imagen { get; set; }  // -

        public string Title { get; set; }   // -

        public string Type { get; set; }    // -

        public int Episodes { get; set; }   // -

        public string Status { get; set; }  // -

        public class AiredIn {
            public string String { get; set; }
        }  

        public AiredIn Aired { get; set; } // -

        public int Year { get; set; }   // -

        public string Rating { get; set; }  // -

        public string Duration { get; set; }    // -

        public class Studio
        {
            public int Mal_id { get; set; }
            public string Type { get; set; }
            public string Name { get; set; }
        }

        public List<Studio> Studios{ get; set; } // -

        public class Genre
        {
            public int Mal_Id { get; set; }
            public string Type { get; set; }
            public string Name { get; set; }
        }

        public List<Genre> Genres { get; set; } // -

        public class Demography
        {
            public int Mal_Id { get; set; }
            public string Type { get; set; }
            public string Name { get; set; }
        }

        public List<Demography> Demographics { get; set; }   // -

        public string Synopsis { get; set; }    // -

        public string Rank { get; set; }    // -

        public double Score { get; set; }   // -
    }
}