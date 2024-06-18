using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Mopups.Services;
using TFGMaui.Models;
using TFGMaui.Services;
using TFGMaui.ViewModels.Mopup;
using TFGMaui.Views.Mopups;
using TFGMaui.Utils;

namespace TFGMaui.ViewModels
{
    [QueryProperty("UsuarioActivo", "UsuarioActivo")]
    internal partial class BookViewModel : ObservableObject
    {
        [ObservableProperty]
        private PageB paginaAux, paginaGenero;

        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        private BookMopup BookMopup;
        private BookMopupViewModel BookMopupViewModel;

        [ObservableProperty]
        private bool isSearchFocus, isGenero;

        [ObservableProperty]
        private BookModel book, book2, book3;

        public BookViewModel()
        {
            IsSearchFocus = false;
            IsGenero = false;

            Book = new();
            Book2 = new();
            Book3 = new();

            // Inicializa lo requerido para el mopup
            BookMopupViewModel = new();
            BookMopup = new(BookMopupViewModel);
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
        public async Task ShowBookMopup(string id)
        {
            BookMopupViewModel.SendHobbieById(id, UsuarioActivo.Id, UsuarioActivo.Language.Split("-")[0]);
            await MopupService.Instance.PushAsync(BookMopup);
            //await Hide();
        }

        [RelayCommand]
        public async Task VolverGeneros()
        {
            IsGenero = !IsGenero;
        }

        [RelayCommand]
        public async Task GetGenre(string busqueda)
        {
            if (busqueda.IsNullOrEmpty())
            {
                await Hide();
                return;
            }

            var requestPagina = new HttpRequestModel(url: IConstantes.BaseBooks,
                endpoint: $"volumes",
                parameters: new Dictionary<string, string> { { "q", $"subject:\"{busqueda}\"" }, { "orderBy", "relevance" }, { "langRestrict", UsuarioActivo.Language.Split("-")[0] }, { "key", IConstantes.ApiKeyBooks }, { "maxResults", "40" } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" } });

            var pagtrend = (PageB)await HttpService.ExecuteRequestAsync<PageB>(requestPagina); // v

            if (pagtrend.Items is null)
            {
                return;
            }

            pagtrend.Items = pagtrend.Items
                .Where(item => item.VolumeInfo.AverageRating != 0
                               && item.VolumeInfo.Title != null
                               && item.VolumeInfo.ImageLinks != null)
                .ToList();

            if (pagtrend.Items.Count >= 10)
            {
                pagtrend.Items = MiscellaneousUtils.GetNelements(pagtrend.Items, 10);
            }

            foreach (var item in pagtrend.Items)
            {
                if (item.VolumeInfo.ImageLinks.Large is not null)
                {
                    item.Imagen = item.VolumeInfo.ImageLinks.Large;
                }
                else if (item.VolumeInfo.ImageLinks.Medium is not null)
                {
                    item.Imagen = item.VolumeInfo.ImageLinks.Medium;
                }
                else if (item.VolumeInfo.ImageLinks.Small is not null)
                {
                    item.Imagen = item.VolumeInfo.ImageLinks.Small;
                }
                else if (item.VolumeInfo.ImageLinks.Thumbnail is not null)
                {
                    item.Imagen = item.VolumeInfo.ImageLinks.Thumbnail;
                }

                item.Color = MiscellaneousUtils.GetColorHobbie("Book");
            }

            PaginaGenero = pagtrend;

            await VolverGeneros();
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

            var requestPagina = new HttpRequestModel(url: IConstantes.BaseBooks,
                endpoint: $"volumes",
                parameters: new Dictionary<string, string> { { "q", $"intitle:\"{busqueda}\"" }, { "langRestrict", UsuarioActivo.Language.Split("-")[0] }, { "key", IConstantes.ApiKeyBooks } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" } });

            var pagtrend = (PageB)await HttpService.ExecuteRequestAsync<PageB>(requestPagina); // v

            if (pagtrend.Items is null)
            {
                return;
            }

            foreach (var item in pagtrend.Items)
            {
                if (item.VolumeInfo.ImageLinks is not null)
                {
                    if (item.VolumeInfo.ImageLinks.Large is not null)
                    {
                        item.Imagen = item.VolumeInfo.ImageLinks.Large;
                    }
                    else if (item.VolumeInfo.ImageLinks.Medium is not null)
                    {
                        item.Imagen = item.VolumeInfo.ImageLinks.Medium;
                    }
                    else if (item.VolumeInfo.ImageLinks.Small is not null)
                    {
                        item.Imagen = item.VolumeInfo.ImageLinks.Small;
                    }
                    else if (item.VolumeInfo.ImageLinks.Thumbnail is not null)
                    {
                        item.Imagen = item.VolumeInfo.ImageLinks.Thumbnail;
                    }
                }
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
        public async Task NavegarSearch()
        {
            await Shell.Current.GoToAsync("//FilterPage", new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioActivo
            });
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
    }
}
