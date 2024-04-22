using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using RestSharp;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using TFGMaui.Models;
using TFGMaui.Services;
using Page = TFGMaui.Models.Page;

namespace TFGMaui.ViewModels
{
    [QueryProperty("UsuarioActivo", "UsuarioActivo")]
    internal partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        [ObservableProperty]
        private string saludos;

        public MainViewModel()
        {
            var peli = GetMovieAsync("438631");
            var pag = GetTrending("week");

            Saludos = "Have a nice " + new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
    .ToString("dddd, d MMM", CultureInfo.InvariantCulture);
        }

        public async Task<Page> GetTrending(string type)
        {
            var requestPagina = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: $"trending/movie/{type}",
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", "es-ES" }, { "page", "1" } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            return (Page)await HttpService.ExecuteRequestAsync<Page>(requestPagina);    // v
        }

        public async Task<MovieModel> GetMovieAsync(string id)
        {
            var requestPelicula = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: $"movie/{id}",
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", "es-ES" } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            return (MovieModel)await HttpService.ExecuteRequestAsync<MovieModel>(requestPelicula);  // v
        }

        [RelayCommand]
        public async Task Navegar(string pagina)
        {
            await Shell.Current.GoToAsync("//" + pagina, new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioActivo
            });
        }

        /// <summary>
        /// Pruebas
        /// </summary>
        private void pruebas()
        {
            SetMarvelUrl();

            string webmarvel = IConstantes.MarvelPage;

            // var peli = MainViewModel.GetMovie("");  // dune 2022
            var anime = GetAnime("2");
            var book = GetBook("2gk0EAAAQBAJ");

            var paginat = GetTrending("week");    // week o day
            var animes = SearchAnime("one piece");    // week o day
        }

        public async Task GetBook(string id)
        {
            var options = new RestClientOptions($"{IConstantes.BaseBooks}/volumes/{id}");
            var client = new RestClient(options);
            var request = new RestRequest();

            request.AddParameter("language", "es-ES");
            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteAsync(request);

            BookModel b = JsonConvert.DeserializeObject<BookModel>(response.Content);
        }

        public async Task GetAnime(string id)
        {
            var options = new RestClientOptions($"{IConstantes.BaseAnimeManga}/manga/{id}");
            var client = new RestClient(options);
            var request = new RestRequest();

            request.AddParameter("language", "es-ES");
            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteAsync(request);

            AnimeData a = JsonConvert.DeserializeObject<AnimeData>(response.Content);

            var an = a.Data;
        }

        public async Task SearchAnime(string anime)
        {
            var options = new RestClientOptions($"{IConstantes.BaseAnimeManga}/anime?q={anime}");
            var client = new RestClient(options);
            var request = new RestRequest();

            request.AddParameter("language", "es-ES");
            request.AddHeader("Accept", "application/json");

            var response = await client.GetAsync(request);

            var s = JsonConvert.DeserializeObject<AnimeList>(response.Content);
        }

        public void SetMarvelUrl()
        {
            // Generate a random TimeStamp
            IConstantes.Ts = new Random().Next().ToString();

            // Prepare the input string for hashing
            string input = $"{IConstantes.Ts}{IConstantes.PrivateMarvelKey}{IConstantes.PublicMarvelKey}";

            // Convert the input string to a byte array and then compute the MD5 hash         
            byte[] hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(input));

            // Convert the hash bytes to a hexadecimal string
            string hash = string.Join(string.Empty,
                hashBytes.Select(b => b.ToString("x2")));

            // Assign the hash to IConstantes.Hash
            IConstantes.Hash = hash;
        }
    }
}
