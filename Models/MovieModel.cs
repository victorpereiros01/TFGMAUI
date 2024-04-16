using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFGMaui.Models
{
    public class MovieModel : HobbieModel
    {
        public int Id { get; set; } // v

        public bool Adult { get; set; } // Solo para adultos v

        public List<GenreM> Genres { get; set; } // Generos v

        public List<int> Genre_ids { get; set; }

        public string Title { get; set; }   // titulo v

        public string Release_Date { get; set; } // v

        public string Original_Language { get; set; } // Idioma original v

        public string OverView { get; set; }    // Descripcion v

        public double Popularity { get; set; }  // Popularidad v

        public string Poster_Path { get; set; }  // Miniatura estilo portrait v, no confundir con el backdrop_path, que es otra imagen distinta: la que se puede poner de fondo al ver los detalles de la imagen

        public long Budget { get; set; }    // Coste v

        public long Revenue { get; set; }   // Dinero obtenido v

        public string Tagline { get; set; } // Frase gancho v

        public string Status { get; set; }  // Como proximamente o released v

        public double VoteAverage { get; set; }

        public List<MovieModel> ListRecommendations { get; set; }    // recomendaciones

        public List<ProductionCompany> Production_Companies { get; set; }    // productoras

        public List<WatchProvider> FilmProviders { get; set; }  // plataformas

        public Credits FilmCredits { get; set; }    // actores
    }

    public class Page
    {
        public int PageC { get; set; }
        public ObservableCollection<MovieModel> Results { get; set; }
        public int Total_pages { get; set; }
        public int Total_results { get; set; }
    }

    public class GenreM
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class WatchProvider
    {
        public int IdProvider { get; set; }
        public List<Country> Countries { get; set; }
    }

    public class Country
    {
        public string CountryAcronym { get; set; }
        public List<Provider> BuyProviders { get; set; }
        public List<Provider> FlatRateProviders { get; set; }
        public List<Provider> AdsProviders { get; set; }
        public List<Provider> RentProviders { get; set; }
    }

    public class Provider
    {
        public int IdProvider { get; set; }
        public string LogoPath { get; set; }
        public string Provider_Name { get; set; }
    }

    public class Credits
    {
        public int IdCredit { get; set; }
        public List<Person> Cast { get; set; }
        public List<Person> Crew { get; set; }
    }

    public class Person
    {
        public string Name { get; set; }
        public string ProfilePath { get; set; }
        public string Character { get; set; }
        public string Job { get; set; }
    }

    public class ProductionCompany
    {
        public int Id { get; set; }
        public string Logo_Path { get; set; }
        public string Name { get; set; }
        public string Origin_Country { get; set; }
    }
}
