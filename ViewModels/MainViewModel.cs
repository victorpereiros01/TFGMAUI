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
    internal partial class MainViewModel : ObservableObject, IConstantes
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        [ObservableProperty]
        private string saludos;

        public void SetMarvelUrl()
        {
            // Generate a random TimeStamp
            IConstantes.Ts = new Random().Next().ToString();

            // Prepare the input string for hashing
            string input = $"{IConstantes.Ts}{IConstantes.PrivateMarvelKey}{IConstantes.PublicMarvelKey}";

            // Convert the input string to a byte array and then compute the MD5 hash         
            byte[] hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(input));

            // Convert the hash bytes to a hexadecimal string
            string hash = string.Join(string.Empty, hashBytes.Select(b => b.ToString("x2")));

            // Assign the hash to IConstantes.Hash
            IConstantes.Hash = hash;
        }

        public MainViewModel()
        {
            SetMarvelUrl();

            string cosa = IConstantes.MarvelPage;

            var peli = GetMovie("438631");  // dune 2022
            var anime = GetAnime("2");
            var book = GetBook("2gk0EAAAQBAJ");

            var pagina = GetTrending("week");    // week o day
            var animes = SearchAnime("one piece");    // week o day

            Saludos = "Have a nice " + DateTime.Now.DayOfWeek.ToString() + " " + DateTime.Now.Day + " - " + new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
    .ToString("MMM", CultureInfo.InvariantCulture);
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

        public async Task GetTrending(string type)
        {
            var options = new RestClientOptions($"{IConstantes.BaseMovieDb}/trending/movie/{type}");
            var client = new RestClient(options);
            var request = new RestRequest();
            request.AddParameter("api_key", IConstantes.ApiKey);
            request.AddParameter("page", 1);

            var response = await client.GetAsync(request);

            var t = JsonConvert.DeserializeObject<Page>(response.Content);
        }

        public async Task GetMovie(string id)
        {
            var options = new RestClientOptions($"{IConstantes.BaseAnimeManga}/movie/{id}");
            var client = new RestClient(options);
            var request = new RestRequest();

            request.AddParameter("api_key", IConstantes.ApiKey);
            request.AddParameter("language", "es-ES");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", IConstantes.Bearer);

            var response = await client.GetAsync(request);

            var m = JsonConvert.DeserializeObject<MovieModel>(response.Content);
        }

        [RelayCommand]
        public async Task Navegar(string pagina)
        {
            await Shell.Current.GoToAsync("//" + pagina, new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioActivo
            });
        }
    }
}
