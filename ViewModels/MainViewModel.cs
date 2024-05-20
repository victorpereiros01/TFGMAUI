using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.ObjectModel;
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

        [ObservableProperty]
        private Page paginaPelis;
        [ObservableProperty]
        private Page paginaPelisTop;

        [ObservableProperty]
        private Page paginaAux;

        [ObservableProperty]
        private ObservableCollection<QuoteModel> quotes;

        /// <summary>
        /// Inicializa los saludos, con el dia en formato dia de la semana, numero y mes. Y obtiene las listas de trending y top
        /// </summary>
        [RelayCommand]
        public async Task InitializeComponents()
        {
            try
            {
                Saludos = "Have a nice " + new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
    .ToString("dddd, d MMM", CultureInfo.InvariantCulture);


                await GetTrending("day");
                await GetTop();
            }
            catch
            {
                Application.Current.MainPage.DisplayAlert("Error", "Fatal", "Aceptar");
            }
        }

        /// <summary>
        /// Obtiene las peliculas trending de moviedb
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task GetTrending(string type)
        {
            var requestPagina = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: $"trending/all/{type}",
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", UsuarioActivo.Language }, { "page", 1.ToString() } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            var pagtrend = (Page)await HttpService.ExecuteRequestAsync<Page>(requestPagina); // v

            pagtrend.Results = MiscellaneousUtils.GetNelements(pagtrend.Results, 6);
            pagtrend.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);

            PaginaPelis = pagtrend;
        }

        /// <summary>
        /// Obtiene las peliculas top de moviedb
        /// </summary>
        /// <returns></returns>
        public async Task GetTop()
        {
            var requestPagina = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: "movie/top_rated",
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", UsuarioActivo.Language }, { "page", 1.ToString() } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            var pagtop = (Page)await HttpService.ExecuteRequestAsync<Page>(requestPagina); // v

            pagtop.Results = MiscellaneousUtils.GetNelements(pagtop.Results, 6);
            pagtop.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);

            PaginaPelisTop = pagtop;
        }

        [RelayCommand]
        public async Task Navegar(string pagina)
        {
            await Shell.Current.GoToAsync("//" + pagina, new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioActivo,
                ["PaginaPelis"] = PaginaPelis
            });
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
}
