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

            string type = "week";    // week o day
            var options = new RestClientOptions($"https://api.themoviedb.org/3/trending/movie/{type}");
            var client = new RestClient(options);
            var request = new RestRequest();
            request.AddParameter("api_key", IConstantes.ApiKey);
            request.AddParameter("page", 3);

            var response = client.Get(request).Content;

            Pagina p = JsonConvert.DeserializeObject<Pagina>(response);

            int j = 0;
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
            MovieModel m = JsonConvert.DeserializeObject<MovieModel>(response.Content);

            int i = 0;
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
