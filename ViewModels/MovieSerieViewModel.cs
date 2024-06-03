using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Mopups.PreBaked.Services;
using Mopups.Services;
using TFGMaui.Models;
using TFGMaui.Repositories;
using TFGMaui.Services;
using TFGMaui.Utils;
using TFGMaui.ViewModels.Mopup;
using TFGMaui.Views.Mopups;

namespace TFGMaui.ViewModels
{
    [QueryProperty("UsuarioActivo", "UsuarioActivo")]
    [QueryProperty("PaginaT", "PaginaT")]
    internal partial class MovieSerieViewModel : ObservableObject
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        [ObservableProperty]
        private PageM paginaT, paginaPelisTop;

        [ObservableProperty]
        private PageS paginaTS, paginaSeriesTop;

        private MovieMopup MovieMopup;
        private MovieMopupViewModel MovieMopupViewModel;

        private SerieMopup SerieMopup;
        private SerieMopupViewModel SerieMopupViewModel;

        [ObservableProperty]
        private string type;

        [ObservableProperty]
        private bool isDay;

        [ObservableProperty]
        private MovieModel movie;

        [ObservableProperty]
        private Color colorType;

        public MovieSerieViewModel()
        {
            IsDay = true;
            ColorType = MiscellaneousUtils.ConvertFromSystemDrawingColor(System.Drawing.Color.Green);

            Movie = new();

            // Inicializa lo requerido para el mopup
            MovieMopupViewModel = new();
            MovieMopup = new(MovieMopupViewModel);
            SerieMopupViewModel = new();
            SerieMopup = new(SerieMopupViewModel);
        }

        [RelayCommand]
        private async Task InitializeComponents()
        {
            Type = "Day";

            await GetTopM();
            await GetTopS();
            await GetTrendS(Type.ToLower());
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
                ColorType = MiscellaneousUtils.ConvertFromSystemDrawingColor(System.Drawing.Color.Blue);
            }
            else
            {
                Type = "Day";
                IsDay = true;
                ColorType = MiscellaneousUtils.ConvertFromSystemDrawingColor(System.Drawing.Color.Green);
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
                var pagtrend = (PageM)await HttpService.ExecuteRequestAsync<PageM>(requestPagina);

                if (cut)
                {
                    pagtrend.Results = MiscellaneousUtils.GetNelements(pagtrend.Results, 5);
                }
                else
                {
                    pagtrend.Results = MiscellaneousUtils.GetNelements(pagtrend.Results, 14);
                }

                pagtrend.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);
                foreach (var item in pagtrend.Results)
                {
                    item.Color = MiscellaneousUtils.GetColorHobbie(item.MediaType.Equals("tv") ? "Serie" : "Movie");
                }

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
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task GetTrendS(string type)
        {
            var requestPagina = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: $"trending/tv/{type}",
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", UsuarioActivo.Language }, { "page", 1.ToString() } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            try
            {
                var pagtrend = (PageS)await HttpService.ExecuteRequestAsync<PageS>(requestPagina);

                pagtrend.Results = MiscellaneousUtils.GetNelements(pagtrend.Results, 5);

                pagtrend.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);
                foreach (var item in pagtrend.Results)
                {
                    item.Color = MiscellaneousUtils.GetColorHobbie("Serie");
                }

                PaginaTS = pagtrend;
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
        public async Task GetTopM(bool cut = true)
        {
            var requestPagina = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: $"movie/top_rated",
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", UsuarioActivo.Language }, { "page", 1.ToString() } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            try
            {
                var pagtrend = (PageM)await HttpService.ExecuteRequestAsync<PageM>(requestPagina);

                if (cut)
                {
                    pagtrend.Results = MiscellaneousUtils.GetNelements(pagtrend.Results, 5);
                }
                pagtrend.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);
                foreach (var item in pagtrend.Results)
                {
                    item.Color = MiscellaneousUtils.GetColorHobbie("Movie");
                }

                PaginaPelisTop = pagtrend;
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
        public async Task GetTopS(bool cut = true)
        {
            var requestPagina = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: $"tv/top_rated",
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", UsuarioActivo.Language }, { "page", 1.ToString() } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            try
            {
                var pagtrend = (PageS)await HttpService.ExecuteRequestAsync<PageS>(requestPagina);

                if (cut)
                {
                    pagtrend.Results = MiscellaneousUtils.GetNelements(pagtrend.Results, 5);
                }
                pagtrend.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);
                foreach (var item in pagtrend.Results)
                {
                    item.Color = MiscellaneousUtils.GetColorHobbie("Serie");
                }

                PaginaSeriesTop = pagtrend;
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
                ["PaginaT"] = PaginaT,
                ["IsGuest"] = UsuarioActivo.Username.Equals("admin")
            });
        }

        [RelayCommand]
        public async Task Navegar(string pagina)
        {
            await Shell.Current.GoToAsync("//" + pagina, new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioActivo,
                ["IsGuest"] = UsuarioActivo.Username.Equals("admin")
            });
        }

        [RelayCommand]
        public async Task NavegarS(string pagina)
        {
            await ShowLoadingMopup();

            await Shell.Current.GoToAsync("//" + pagina, new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioActivo,
                ["PaginaSeriesTop"] = PaginaSeriesTop,
                ["PaginaTS"] = PaginaTS,
                ["IsGuest"] = UsuarioActivo.Username.Equals("admin")
            });
        }

        /// <summary>
        /// Abre el mopup
        /// </summary>
        /// <param name="id">Id de la pelicula seleccionada</param>
        [RelayCommand]
        public async Task ShowMovieMopup(string id)
        {
            if (Movie.MediaType.Equals("movie"))
            {
                MovieMopupViewModel.SendHobbieById(id, UsuarioActivo.Id, UsuarioActivo.Language);
                await MopupService.Instance.PushAsync(MovieMopup);
            }
            else if (Movie.MediaType.Equals("tv"))
            {
                SerieMopupViewModel.SendHobbieById(id, UsuarioActivo.Id, UsuarioActivo.Language);
                await MopupService.Instance.PushAsync(SerieMopup);
            }
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
