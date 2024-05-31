using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Mopups.Services;
using TFGMaui.Models;
using TFGMaui.Services;
using TFGMaui.Utils;
using TFGMaui.ViewModels.Mopup;
using TFGMaui.Views.Mopups;

namespace TFGMaui.ViewModels
{
    [QueryProperty("UsuarioActivo", "UsuarioActivo")]
    [QueryProperty("PaginaT", "PaginaT")]
    [QueryProperty("PaginaPelisTop", "PaginaPelisTop")]
    internal partial class MovieViewModel : ObservableObject
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        [ObservableProperty]
        private bool isSearchFocus;

        [ObservableProperty]
        private MovieModel movie, movie2, movie3;

        [ObservableProperty]
        private PageM paginaT;
        [ObservableProperty]
        private PageM paginaPelisTop;

        [ObservableProperty]
        private PageM paginaAux;

        private MovieMopup MovieMopup;
        private MovieMopupViewModel MovieMopupViewModel;

        [ObservableProperty]
        private string type;

        [ObservableProperty]
        private bool isDay;

        [ObservableProperty]
        private Color colorType;

        public MovieViewModel()
        {
            Type = "Day";
            IsDay = true;
            ColorType = MiscellaneousUtils.ConvertFromSystemDrawingColor(System.Drawing.Color.LightGoldenrodYellow);

            IsSearchFocus = false;
            Movie = new();
            Movie2 = new();
            Movie3 = new();

            // Inicializa lo requerido para el mopup
            MovieMopupViewModel = new MovieMopupViewModel();
            MovieMopup = new MovieMopup(MovieMopupViewModel);
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

            await GetTrending(Type.ToLower());
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
        public async Task Hide()
        {
            IsSearchFocus = false;
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
        /// Obtiene las peliculas trending de moviedb
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task GetTrending(string type, bool cut = true)
        {
            var requestPagina = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: $"trending/movie/{type}",
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", UsuarioActivo.Language }, { "page", 1.ToString() } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            try
            {
                var pagtrend = (PageM)await HttpService.ExecuteRequestAsync<PageM>(requestPagina);

                if (cut)
                {
                    pagtrend.Results = MiscellaneousUtils.GetNelements(pagtrend.Results, 8);
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
        /// Busca las peliculas que coincidan con un termino
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task GetSearch(string busqueda)
        {
            if (busqueda.IsNullOrEmpty())
            {
                await Hide();
                return;
            }

            var requestPagina = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: $"search/movie",
                parameters: new Dictionary<string, string> { { "query", busqueda }, { "api_key", IConstantes.MovieDB_ApiKey }, { "language", UsuarioActivo.Language } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            var pagtrend = (PageM)await HttpService.ExecuteRequestAsync<PageM>(requestPagina); // v

            pagtrend.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);

            PaginaAux = pagtrend;
            IsSearchFocus = true;
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
                var pagtrend = (PageM)await HttpService.ExecuteRequestAsync<PageM>(requestPagina);

                if (cut)
                {
                    pagtrend.Results = MiscellaneousUtils.GetNelements(pagtrend.Results, 8);
                }
                pagtrend.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);

                PaginaPelisTop = pagtrend;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Saludos", e.ToString(), "Aceptar");
            }
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
            //await Hide();
        }

        [RelayCommand]
        public async Task NavegarT()
        {
            await GetTrending(Type.ToLower(), false);
            await NavegarSearch();
        }

        [RelayCommand]
        public async Task NavegarTop()
        {
            await GetTop(false);
            await Shell.Current.GoToAsync("//FilterPage", new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioActivo,
                ["PaginaPelis"] = PaginaPelisTop
            });
        }

        [RelayCommand]
        public async Task NavegarSearch()
        {
            await Shell.Current.GoToAsync("//FilterPage", new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioActivo,
                ["PaginaPelis"] = PaginaT
            });
        }
    }
}
