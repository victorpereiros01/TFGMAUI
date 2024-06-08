using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using TFGMaui.Models;
using TFGMaui.Utils;
using TFGMaui.ViewModels.Mopup;
using TFGMaui.Views.Mopups;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Maui.Graphics;
using TFGMaui.Services;
using Mopups.Services;

namespace TFGMaui.ViewModels
{
    [QueryProperty("UsuarioActivo", "UsuarioActivo")]
    [QueryProperty("PaginaTS", "PaginaTS")]
    [QueryProperty("PaginaSeriesTop", "PaginaSeriesTop")]
    internal partial class SerieViewModel : ObservableObject
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        [ObservableProperty]
        private bool isSearchFocus;

        [ObservableProperty]
        private SerieModel serie, serie2, serie3;

        [ObservableProperty]
        private PageS paginaTS, paginaSeriesTop;

        [ObservableProperty]
        private PageS paginaAux;

        private SerieMopup SerieMopup;
        private SerieMopupViewModel SerieMopupViewModel;

        [ObservableProperty]
        private string type;

        [ObservableProperty]
        private bool isDay;

        [ObservableProperty]
        private Color colorType;

        public SerieViewModel()
        {
            Type = "Day";
            IsDay = true;
            ColorType = MiscellaneousUtils.ConvertFromSystemDrawingColor(System.Drawing.Color.Green);

            IsSearchFocus = false;
            Serie = new();
            Serie2 = new();
            Serie3 = new();

            // Inicializa lo requerido para el mopup
            SerieMopupViewModel = new();
            SerieMopup = new(SerieMopupViewModel);
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

        /// <summary>
        /// Obtiene las peliculas trending de moviedb
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task GetTrending(string type, bool cut = true)
        {
            var requestPagina = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: $"trending/tv/{type}",
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", UsuarioActivo.Language }, { "page", 1.ToString() } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            try
            {
                var pagtrend = (PageS)await HttpService.ExecuteRequestAsync<PageS>(requestPagina);

                if (cut)
                {
                    pagtrend.Results = MiscellaneousUtils.GetNelements(pagtrend.Results, 10);
                }
                pagtrend.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);
                foreach (var item in pagtrend.Results)
                {
                    item.Color = MiscellaneousUtils.GetColorHobbie("Serie")[0];
                    item.Color2 = MiscellaneousUtils.GetColorHobbie("Serie")[1];
                }

                PaginaTS = pagtrend;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Saludos", e.ToString(), "Aceptar");
            }
        }

        [RelayCommand]
        public async Task NavegarT()
        {
            await GetTrending(Type.ToLower(), false);
            await NavegarSearch();
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
                endpoint: $"search/tv",
                parameters: new Dictionary<string, string> { { "query", busqueda }, { "api_key", IConstantes.MovieDB_ApiKey }, { "language", UsuarioActivo.Language } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            var pagtrend = (PageS)await HttpService.ExecuteRequestAsync<PageS>(requestPagina); // v

            pagtrend.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);
            foreach (var item in pagtrend.Results)
            {
                item.Color = MiscellaneousUtils.GetColorHobbie("Serie")[0];
                item.Color2 = MiscellaneousUtils.GetColorHobbie("Serie")[1];
            }

            PaginaAux = pagtrend;
            IsSearchFocus = true;
        }

        /// <summary>
        /// Abre el mopup
        /// </summary>
        /// <param name="id">Id de la pelicula seleccionada</param>
        [RelayCommand]
        public async Task ShowMovieMopup(string id)
        {
            SerieMopupViewModel.SendHobbieById(id, UsuarioActivo.Id, UsuarioActivo.Language);
            await MopupService.Instance.PushAsync(SerieMopup);
            //await Hide();
        }

        [RelayCommand]
        public async Task CambiarType()
        {
            if (IsDay)
            {
                Type = "Week";
                IsDay = false;
                ColorType = MiscellaneousUtils.ConvertFromSystemDrawingColor(System.Drawing.Color.Green).GetComplementary();
            }
            else
            {
                Type = "Day";
                IsDay = true;
                ColorType = MiscellaneousUtils.ConvertFromSystemDrawingColor(System.Drawing.Color.Green);
            }

            await GetTrending(Type.ToLower());
        }

        [RelayCommand]
        public async Task Navegar(string pagina)
        {
            await Shell.Current.GoToAsync("//" + pagina, new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioActivo
            });
        }

        [RelayCommand]
        public async Task NavegarSearch()
        {
            await Shell.Current.GoToAsync("//FilterPage", new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioActivo,
                ["PaginaPelis"] = PaginaTS
            });
        }
    }
}
