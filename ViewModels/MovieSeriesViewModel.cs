using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.PreBaked.Services;
using Mopups.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using TFGMaui.Models;
using TFGMaui.Repositories;
using TFGMaui.Views.Mopups;
using Color = System.Drawing.Color;
using Page = TFGMaui.Models.Page;
using TFGMaui.Services;

namespace TFGMaui.ViewModels
{
    [QueryProperty("UsuarioActivo", "UsuarioActivo")]
    internal partial class MovieSeriesViewModel : ObservableObject
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
        private int selectedPage;

        private MovieMopup MovieMopup;
        private MovieMopupViewModel MovieMopupViewModel;

        [ObservableProperty]
        private ObservableCollection<QuoteModel> quotes;

        [ObservableProperty]
        private MovieModel movie;

        public MovieSeriesViewModel()
        {
            // Inicializa lo requerido para el mopup
            MovieMopupViewModel = new MovieMopupViewModel();
            MovieMopup = new MovieMopup(MovieMopupViewModel);
        }

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

                SelectedPage = 1;

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
                endpoint: $"trending/movie/{type}",
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", UsuarioActivo.Language }, { "page", SelectedPage.ToString() } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            var pagtrend = (Page)await HttpService.ExecuteRequestAsync<Page>(requestPagina); // v

            pagtrend.Results = GetNelements(pagtrend.Results, 6);
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
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", UsuarioActivo.Language }, { "page", SelectedPage.ToString() } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            var pagtop = (Page)await HttpService.ExecuteRequestAsync<Page>(requestPagina); // v

            pagtop.Results = GetNelements(pagtop.Results, 6);
            pagtop.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);

            PaginaPelisTop = pagtop;
        }

        /// <summary>
        /// Obtiene los primeros n elementos de la lista
        /// </summary>
        /// <param name="results"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
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
            MovieMopupViewModel.SendHobbieById(id, UsuarioActivo.Id, UsuarioActivo.Language);
            await MopupService.Instance.PushAsync(MovieMopup);
        }


        /// <summary>
        /// Abre el mopup de loading
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task ShowLoadingMopup()
        {
            var quote = new HobbieRepository().GetQuoteRandom();
            if (quote is null)
            {
                return;
            }

            List<string> list = [quote.Source, quote.Value];

            // Loader
            await PreBakedMopupService.GetInstance().WrapTaskInLoader(Task.Delay(4000), ColorConverterUtil.ConvertFromSystemDrawingColor(Color.Red), ColorConverterUtil.ConvertFromSystemDrawingColor(Color.White), list, ColorConverterUtil.ConvertFromSystemDrawingColor(Color.Black));
        }
    }
}
