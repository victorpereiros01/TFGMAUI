﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.PreBaked.PopupPages.DualResponse;
using Mopups.PreBaked.PopupPages.EntryInput;
using Mopups.PreBaked.PopupPages.Loader;
using Mopups.PreBaked.PopupPages.SingleResponse;
using Mopups.PreBaked.PopupPages.TextInput;
using Mopups.PreBaked.Services;
using Mopups.Services;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using TFGMaui.Models;
using TFGMaui.Services;
using TFGMaui.Views.Mopups;
using Color = System.Drawing.Color;

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

        [ObservableProperty]
        private ObservableCollection<QuoteModel> quotes;

        public MainViewModel()
        {
            _ = InitializeComponents();

            // Inicializa lo requerido para el mopup
            MovieMopupViewModel = new MovieMopupViewModel();
            MovieMopup = new MovieMopup(MovieMopupViewModel);
        }

        /// <summary>
        /// Inicializa los saludos, con el dia en formato dia de la semana, numero y mes. Y obtiene las listas de trending y top
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

            var pagtrend = (Page)await HttpService.ExecuteRequestAsync<Page>(requestPagina); // v

            pagtrend.Results = GetNelements(pagtrend.Results, 6);
            pagtrend.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);

            PaginaPelis = pagtrend;
        }

        public async Task GetTop()
        {
            var requestPagina = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: "movie/top_rated",
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", SelectedLanguage }, { "page", SelectedPage.ToString() } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            var pagtop = (Page)await HttpService.ExecuteRequestAsync<Page>(requestPagina); // v

            pagtop.Results = GetNelements(pagtop.Results, 6);
            pagtop.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);

            PaginaPelisTop = pagtop;
        }

        private List<MovieModel> GetNelements(List<MovieModel> results, int v2)
        {
            List<MovieModel> list = [];

            for (int i = 0; i < v2; i++)
            {
                list.Add(results[i]);
            }

            return list;
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

        [RelayCommand]
        public async Task ShowLoadingMopup()
        {
            // Loader
            await PreBakedMopupService.GetInstance().WrapTaskInLoader(Task.Delay(5000), ColorConverterUtil.ConvertFromSystemDrawingColor(Color.Blue), ColorConverterUtil.ConvertFromSystemDrawingColor(Color.White), LoadingReasons(), ColorConverterUtil.ConvertFromSystemDrawingColor(Color.Black));

            //// Single response
            // return await SingleResponseViewModel.AutoGenerateBasicPopup(Color.HotPink, Color.Black, "I Accept", Color.Gray, "Good Job, enjoy this single response example", "thumbsup.png");

            //// Dual response
            // return await DualResponseViewModel.AutoGenerateBasicPopup(Color.WhiteSmoke, Color.Red, "Okay", Color.WhiteSmoke, Color.Green, "Looks Good!", Color.DimGray, "This is an example of a dual response popup page", "thumbsup.png");

            //// Text input
            //await TextInputViewModel.AutoGenerateBasicPopup(Color.WhiteSmoke, Color.Red, "Cancel", Color.WhiteSmoke, Color.Green, "Submit", Color.DimGray, "Text input Example", string.Empty);

            //// Entry input
            //await EntryInputViewModel.AutoGenerateBasicPopup(Color.WhiteSmoke, Color.Red, "Cancel", Color.WhiteSmoke, Color.Green, "Submit", Color.DimGray, "Text input Example", string.Empty);

            //// Login page popup
            //var (username, password) = await LoginViewModel.AutoGenerateBasicPopup(Color.WhiteSmoke, Color.Red, "Cancel", Color.WhiteSmoke, Color.Green, "Submit", Color.DimGray, string.Empty, "Username Here", string.Empty, "Password here", "thumbsup.png", 0, 0);
            //// or to return from the loader value
            //await PreBakedMopupService.GetInstance().WrapReturnableTaskInLoader<bool, LoaderPopupPage>(IndepthCheckAgainstDatabase(), Color.Blue, Color.White, LoadingReasons(), Color.Black);
        }

        private List<string> LoadingReasons()
        {
            _ = GetQuoteRandomAsync();

            //return ["Huevo", "Mi"];
            return [Quotes[0].Author, Quotes[0].Content];
        }

        private async Task GetQuoteRandomAsync()
        {
            var requestPagina = new HttpRequestModel(url: IConstantes.BaseQuotes,
                endpoint: "quotes/random",
                parameters: new Dictionary<string, string> { { "maxLength", "50" } },
                headers: []);

            Quotes = (ObservableCollection<QuoteModel>)await HttpService.ExecuteRequestAsync<ObservableCollection<QuoteModel>>(requestPagina);
        }
    }

    internal class Page
    {
        public int PageC { get; set; }
        public List<MovieModel> Results { get; set; }
        public int Total_pages { get; set; }
        public int Total_results { get; set; }
    }
}
