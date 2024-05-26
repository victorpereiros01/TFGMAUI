using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Maui.Graphics;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Mopups.PreBaked.Services;
using Mopups.Services;
using System.Collections.ObjectModel;
using TFGMaui.Models;
using TFGMaui.Repositories;
using TFGMaui.Services;
using TFGMaui.Utils;
using TFGMaui.ViewModels.Mopup;
using TFGMaui.Views.Mopups;
using Page = TFGMaui.Models.Page;

namespace TFGMaui.ViewModels
{
    [QueryProperty("UsuarioActivo", "UsuarioActivo")]
    [QueryProperty("PaginaT", "PaginaT")]
    internal partial class MovieSeriesViewModel : ObservableObject
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        [ObservableProperty]
        private Page paginaT, paginaPelisTop;

        private MovieMopup MovieMopup;
        private MovieMopupViewModel MovieMopupViewModel;

        [ObservableProperty]
        private string type;

        [ObservableProperty]
        private bool isDay;

        [ObservableProperty]
        private MovieModel movie;

        [ObservableProperty]
        private Color colorType;

        public MovieSeriesViewModel()
        {
            Type = "Day";
            IsDay = true;
            ColorType = MiscellaneousUtils.ConvertFromSystemDrawingColor(System.Drawing.Color.LightGoldenrodYellow);

            Movie = new();

            // Inicializa lo requerido para el mopup
            MovieMopupViewModel = new MovieMopupViewModel();
            MovieMopup = new MovieMopup(MovieMopupViewModel);
        }

        [RelayCommand]
        private async Task InitializeComponents()
        {
            await GetTop();
        }

        [RelayCommand]
        public async Task MinimizeApp()
        {
#if WINDOWS
            var nativeWindow = App.Current.Windows.First().Handler.PlatformView;
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);

            AppWindow appWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(windowHandle));

            if (appWindow.Presenter is OverlappedPresenter p)
            {
                p.Minimize();
            }
#endif
        }

        [RelayCommand]
        public async Task CloseApp()
        {
            Application.Current.Quit();
        }

        [RelayCommand]
        public async Task CambiarType()
        {
            if (IsDay)
            {
                Type = "Week";
                IsDay = false;
                ColorType = MiscellaneousUtils.ConvertFromSystemDrawingColor(System.Drawing.Color.RebeccaPurple);
            }
            else
            {
                Type = "Day";
                IsDay = true;
                ColorType = MiscellaneousUtils.ConvertFromSystemDrawingColor(System.Drawing.Color.LightGoldenrodYellow);
            }

            await GetTrending(Type.ToLower(), false);
        }

        /// <summary>
        /// Obtiene las peliculas trending de moviedb
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task GetTrending(string type, bool cut = true)
        {
            var movOrAll = cut ? "movie" : "all";
            var requestPagina = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: $"trending/{movOrAll}/{type}",
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", UsuarioActivo.Language }, { "page", 1.ToString() } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            try
            {
                var pagtrend = (Page)await HttpService.ExecuteRequestAsync<Page>(requestPagina);

                if (cut)
                {
                    pagtrend.Results = MiscellaneousUtils.GetNelementsM(pagtrend.Results, 8);
                }
                else
                {
                    pagtrend.Results = MiscellaneousUtils.GetNelementsM(pagtrend.Results, 6);
                }

                pagtrend.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);

                PaginaT = pagtrend;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Saludos", e.ToString(), "Aceptar");
            }
        }

        /// <summary>
        /// Obtiene las peliculas trending de moviedb
        /// </summary>
        /// <returns></returns>
        public async Task GetTop(bool cut = true)
        {
            var requestPagina = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: $"movie/top_rated",
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", UsuarioActivo.Language }, { "page", 1.ToString() } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            try
            {
                var pagtrend = (Page)await HttpService.ExecuteRequestAsync<Page>(requestPagina);

                if (cut)
                {
                    pagtrend.Results = MiscellaneousUtils.GetNelementsM(pagtrend.Results, 8);
                }
                pagtrend.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);

                PaginaPelisTop = pagtrend;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Saludos", e.ToString(), "Aceptar");
            }
        }

        [RelayCommand]
        public async Task NavegarM(string pagina)
        {
            await ShowLoadingMopup();
            await GetTrending(Type.ToLower());

            await Shell.Current.GoToAsync("//" + pagina, new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioActivo,
                ["PaginaPelisTop"] = PaginaPelisTop,
                ["PaginaT"] = PaginaT
            });
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
            var quote = new Repository().GetQuoteRandom();

            if (quote is null)
            {
                return;
            }

            List<string> list = [quote.Source, quote.Value];

            // Loader
            await PreBakedMopupService.GetInstance().WrapTaskInLoader(Task.Delay(4000), MiscellaneousUtils.ConvertFromSystemDrawingColor(System.Drawing.Color.Red), MiscellaneousUtils.ConvertFromSystemDrawingColor(System.Drawing.Color.White), list, MiscellaneousUtils.ConvertFromSystemDrawingColor(System.Drawing.Color.Black));
        }
    }
}
