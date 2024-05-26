using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using TFGMaui.Models;
using TFGMaui.ViewModels.Mopup;
using TFGMaui.Views.Mopups;
using Microsoft.IdentityModel.Tokens;
using TFGMaui.Services;
using Mopups.Services;

namespace TFGMaui.ViewModels
{
    [QueryProperty("UsuarioActivo", "UsuarioActivo")]
    [QueryProperty("PaginaS", "PaginaS")]
    [QueryProperty("PaginaTopAnime", "PaginaTopAnime")]
    [QueryProperty("PaginaTopManga", "PaginaTopManga")]
    internal partial class AnimeMangaViewModel : ObservableObject
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        [ObservableProperty]
        private PageAM paginaS;

        [ObservableProperty]
        private PageAM paginaTopAnime, paginaTopManga;

        [ObservableProperty]
        private PageAM paginaAux;

        [ObservableProperty]
        private AnimeModel anime, anime2, anime3, anime4;

        private AnimeMopup AnimeMopup;
        private AnimeMopupViewModel AnimeMopupViewModel;

        [ObservableProperty]
        private bool isSearchFocus;

        public AnimeMangaViewModel()
        {
            IsSearchFocus = false;
            Anime = new();
            Anime2 = new();
            Anime3 = new();
            Anime4 = new();

            // Inicializa lo requerido para el mopup
            AnimeMopupViewModel = new AnimeMopupViewModel();
            AnimeMopup = new AnimeMopup(AnimeMopupViewModel);
        }

        /// <summary>
        /// Abre el mopup
        /// </summary>
        /// <param name="id">Id de la pelicula seleccionada</param>
        [RelayCommand]
        public async Task ShowAnimeMangaMopup(string id)
        {
            AnimeMopupViewModel.SendHobbieById(id, UsuarioActivo.Id);
            await MopupService.Instance.PushAsync(AnimeMopup);
            //await Hide();
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

            var requestPagina = new HttpRequestModel(url: IConstantes.BaseAnimeManga,
                endpoint: $"anime",
                parameters: new Dictionary<string, string> { { "q", busqueda } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" } });

            var pagtrend = (PageAM)await HttpService.ExecuteRequestAsync<PageAM>(requestPagina); // v
            foreach (var item in pagtrend.Data)
            {
                item.Imagen = item.Images.Jpg.Image_url;
            }

            PaginaAux = pagtrend;
            IsSearchFocus = true;
        }

        [RelayCommand]
        public async Task Hide()
        {
            IsSearchFocus = false;
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
        public async Task NavegarSearch()
        {
            await Shell.Current.GoToAsync("//FilterPage", new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioActivo
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
    }
}
