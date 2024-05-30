using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Mopups.Services;
using TFGMaui.Models;
using TFGMaui.Services;
using TFGMaui.ViewModels.Mopup;
using TFGMaui.Views.Mopups;

namespace TFGMaui.ViewModels
{
    [QueryProperty("UsuarioActivo", "UsuarioActivo")]
    [QueryProperty("PaginaTrendG", "PaginaTrendG")]
    [QueryProperty("PaginaTopG", "PaginaTopG")]
    internal partial class GameViewModel : ObservableObject
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        [ObservableProperty]
        private PageG paginaTopG, paginaTrendG;

        private GameMopup GameMopup;
        private GameMopupViewModel GameMopupViewModel;

        [ObservableProperty]
        private PageM paginaAux;

        [ObservableProperty]
        private bool isSearchFocus;

        [ObservableProperty]
        private GameModel game, game2, game3;

        public string Bearer { get; set; }

        public GameViewModel()
        {
            IsSearchFocus = false;
            Game = new();
            Game2 = new();
            Game3 = new();

            // Inicializa lo requerido para el mopup
            GameMopupViewModel = new();
            GameMopup = new(GameMopupViewModel);
            GetBearerG();
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

        private async void GetBearerG()
        {
            var requestPagina = new HttpRequestModel(url: "https://id.twitch.tv/oauth2/token",
                endpoint: "",
                parameters: new Dictionary<string, string> { { "client_id", IConstantes.client_id }, { "client_secret", IConstantes.client_secret }, { "grant_type", "client_credentials" } },
                headers: null, body: "");

            try
            {
                var b = (BearerModel)await HttpService.ExecuteRequestAsync<BearerModel>(requestPagina);
                Bearer = b.AccessToken;
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

            var requestPagina = new HttpRequestModel(url: IConstantes.BaseGames,
                endpoint: $"games",
                parameters: null,
                headers: new Dictionary<string, string> { { "Client-ID", IConstantes.client_id }, { "Authorization", $"Bearer jxq1u2drybuywugxmjqzauigzrbcim" }, { "Accept", "application/json" } },
                body: """
                fields *;
                sort popularity desc;
                limit 8;
                """);

            var pagtrend = (PageM)await HttpService.ExecuteRequestAsync<PageM>(requestPagina); // v

            pagtrend.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);

            PaginaAux = pagtrend;
            IsSearchFocus = true;
        }

        [RelayCommand]
        public async Task NavegarSearch()
        {
            await Shell.Current.GoToAsync("//FilterPage", new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioActivo,
                ["PaginaPelis"] = paginaTrendG
            });
        }

        /// <summary>
        /// Abre el mopup
        /// </summary>
        /// <param name="id">Id de la pelicula seleccionada</param>
        [RelayCommand]
        public async Task ShowMovieMopup(string id)
        {
            GameMopupViewModel.SendHobbieById(id, UsuarioActivo.Id, Bearer);
            await MopupService.Instance.PushAsync(GameMopup);
            //await Hide();
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
