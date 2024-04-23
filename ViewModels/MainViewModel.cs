using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using TFGMaui.Models;
using TFGMaui.Services;
using TFGMaui.Views.Mopups;

namespace TFGMaui.ViewModels
{
    [QueryProperty("UsuarioActivo", "UsuarioActivo")]
    internal partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        [ObservableProperty]
        private string saludos;

        [ObservableProperty]
        private Page paginaPelis;
        [ObservableProperty]
        private Page paginaPelisTop;

        [ObservableProperty]
        private ObservableCollection<string> languages;

        [ObservableProperty]
        private string selectedLanguage;

        [ObservableProperty]
        private int selectedPage;

        private MovieMopup MovieMopup;
        private MovieMopupViewModel MovieMopupViewModel;

        public MainViewModel()
        {
            _ = InitializeComponents();

            // Inicializa lo requerido para el mopup
            MovieMopupViewModel = new MovieMopupViewModel();
            MovieMopup = new MovieMopup(MovieMopupViewModel);
        }

        /// <summary>
        /// Inicializa los saludos, con el dia en formato Dia de la semana, numero y mes. Y obtiene las listas de trending y top
        /// </summary>
        public async Task InitializeComponents()
        {
            try
            {
                Saludos = "Have a nice " + new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
    .ToString("dddd, d MMM", CultureInfo.InvariantCulture);

                Languages = ["es-ES", "en-US", "en-UK"];
                SelectedLanguage = Languages[0];

                SelectedPage = 1;

                await GetTrending("day");
                await GetTop();
            }
            catch
            {
                Application.Current.MainPage.DisplayAlert("Error", "Fatal", "Aceptar");
            }
        }

        public async Task GetTrending(string type)
        {
            var requestPagina = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: $"trending/movie/{type}",
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", SelectedLanguage }, { "page", SelectedPage.ToString() } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            PaginaPelis = (Page)await HttpService.ExecuteRequestAsync<Page>(requestPagina); // v

            foreach (var item in PaginaPelis.Results)
            {
                item.Imagen = "https://image.tmdb.org/t/p/original" + item.Imagen;
            }
        }

        public async Task GetTop()
        {
            var requestPagina = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: $"movie/top_rated",
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", SelectedLanguage }, { "page", SelectedPage.ToString() } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            PaginaPelisTop = (Page)await HttpService.ExecuteRequestAsync<Page>(requestPagina); // v

            foreach (var item in PaginaPelisTop.Results)
            {
                item.Imagen = "https://image.tmdb.org/t/p/original" + item.Imagen;
            }
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
        /// Abre el mopup
        /// </summary>
        /// <param name="id">Id de la pelicula seleccionada</param>
        [RelayCommand]
        public async Task ShowMovieMopup(string id)
        {
            MovieMopupViewModel.SendHobbieById(id);
            await MopupService.Instance.PushAsync(MovieMopup);
        }

        /// <summary>
        /// Pruebas
        /// </summary>
        private async void pruebas()
        {
            string webmarvel = IConstantes.MarvelPage;

            // 438631 // dune 2022
            var options = new RestClientOptions($"{IConstantes.BaseAnimeManga}/manga/2");
            var client = new RestClient(options);
            var request = new RestRequest();

            request.AddParameter("language", "es-ES");
            request.AddHeader("Accept", "application/json");

            //
            // libro por id
            //

            var options2 = new RestClientOptions($"{IConstantes.BaseBooks}/volumes/2gk0EAAAQBAJ");
            var client2 = new RestClient(options2);
            var request2 = new RestRequest();

            request2.AddParameter("language", "es-ES");
            request2.AddHeader("Accept", "application/json");

            var response2 = await client.ExecuteAsync(request2);

            BookModel b = JsonConvert.DeserializeObject<BookModel>(response2.Content);

            //
            // anime search
            //

            var options3 = new RestClientOptions($"{IConstantes.BaseAnimeManga}/anime?q=one piece");
            var client3 = new RestClient(options3);
            var request3 = new RestRequest();

            request3.AddParameter("language", "es-ES");
            request.AddHeader("Accept", "application/json");

            var response3 = await client.GetAsync(request3);

            var s = JsonConvert.DeserializeObject<AnimeList>(response3.Content);

            //
            // Get the marvel api_key
            //

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

    internal class Page
    {
        public int PageC { get; set; }
        public ObservableCollection<MovieModel> Results { get; set; }
        public int Total_pages { get; set; }
        public int Total_results { get; set; }
    }
}
