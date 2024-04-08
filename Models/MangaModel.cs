using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFGMaui.Models
{
    public class MangaModel : HobbieModel
    {
        public int Mal_id { get; set; } // -

        public Imagen Imagen { get; set; }  // -

        public string Title { get; set; }   // -

        public string Type { get; set; }    // -

        public string Status { get; set; }  // -

        public string Rank { get; set; }    // -

        public AiredIn Aired { get; set; } // -

        public List<Parameters> Authors { get; set; }

        public List<Parameters> Genres { get; set; } // -

        public List<Parameters> Demographics { get; set; }   // -

        public double Score { get; set; }   // -

        public string Synopsis { get; set; }    // -
    }
}
