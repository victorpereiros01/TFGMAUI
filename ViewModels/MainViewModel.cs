using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.ObjectModel;
using TFGMaui.Models;
using TFGMaui.Services;

namespace TFGMaui.ViewModels
{
    [QueryProperty("UsuarioActivo", "UsuarioActivo")]
    internal partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        public class Pagina
        {
            public int Page { get; set; }
            public ObservableCollection<MovieModel> Results { get; set; }
            public int Total_pages { get; set; }
            public int Total_results { get; set; }
        }

        public MainViewModel()
        {
            var peli = GetMovie("438631");  // dune 2022
            var anime = GetAnime("2");
            var book = GetBook("2gk0EAAAQBAJ");

            var pagina = GetTrending("week");    // week o day
            var animes = SearchAnime("one piece");    // week o day

            int j = 0;
        }

        public async Task GetBook(string id)
        {
            var options = new RestClientOptions($"https://www.googleapis.com/books/v1/volumes/{id}");
            var client = new RestClient(options);
            var request = new RestRequest();

            request.AddParameter("language", "es-ES");
            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteAsync(request);

            BookModel b = JsonConvert.DeserializeObject<BookModel>(response.Content);

            int i = 0;
        }

        public async Task GetAnime(string id)
        {
            var options = new RestClientOptions($"https://api.jikan.moe/v4/manga/{id}");
            var client = new RestClient(options);
            var request = new RestRequest();

            request.AddParameter("language", "es-ES");
            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteAsync(request);

            AnimeData a = JsonConvert.DeserializeObject<AnimeData>(response.Content);

            int i = 0;
            var an = a.Data;
        }

        public async Task SearchAnime(string anime)
        {
            var options = new RestClientOptions($"https://api.jikan.moe/v4/anime?q={anime}");
            var client = new RestClient(options);
            var request = new RestRequest();

            request.AddParameter("language", "es-ES");
            request.AddHeader("Accept", "application/json");

            var response = await client.GetAsync(request);
            int i = 0;
            var s = JsonConvert.DeserializeObject<AnimeList>(response.Content);
        }

        public async Task GetTrending(string type)
        {
            var options = new RestClientOptions($"https://api.themoviedb.org/3/trending/movie/{type}");
            var client = new RestClient(options);
            var request = new RestRequest();
            request.AddParameter("api_key", IConstantes.ApiKey);
            request.AddParameter("page", 1);

            var response = await client.GetAsync(request);
            int i = 0;
            var t = JsonConvert.DeserializeObject<Pagina>(response.Content);
        }

        public async Task GetMovie(string id)
        {
            var options = new RestClientOptions($"https://api.themoviedb.org/3/movie/{id}");
            var client = new RestClient(options);
            var request = new RestRequest();

            request.AddParameter("api_key", IConstantes.ApiKey);
            request.AddParameter("language", "es-ES");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", IConstantes.Bearer);

            var response = await client.GetAsync(request);
            int i = 0;
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
