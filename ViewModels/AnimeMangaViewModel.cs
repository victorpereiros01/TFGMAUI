﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using TFGMaui.Models;
using TFGMaui.ViewModels.Mopup;
using TFGMaui.Views.Mopups;
using Microsoft.IdentityModel.Tokens;
using TFGMaui.Services;
using Mopups.Services;
using TFGMaui.Utils;

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
        private PageA paginaS;

        [ObservableProperty]
        private PageA paginaTopAnime;

        [ObservableProperty]
        private PageMa paginaTopManga;

        [ObservableProperty]
        private PageA paginaAux;

        [ObservableProperty]
        private PageMa paginaAux2;

        [ObservableProperty]
        private AnimeModel anime, anime2, anime3;

        [ObservableProperty]
        private MangaModel manga, manga2;

        private AnimeMopup AnimeMopup;
        private AnimeMopupViewModel AnimeMopupViewModel;

        private MangaMopup MangaMopup;
        private MangaMopupViewModel MangaMopupViewModel;

        [ObservableProperty]
        private bool isSearchFocus, isSearchFocus2;

        [ObservableProperty]
        private bool isFocusScr1, isFocusScr2;

        [ObservableProperty]
        private bool isSearchSeasonFocus;

        [ObservableProperty]
        private bool isSeasonSelected;

        [ObservableProperty]
        private ImageSource imageSeason;

        [ObservableProperty]
        private string textSeason;

        public AnimeMangaViewModel()
        {
            ImageSeason = "";
            IsSearchFocus = false;
            IsSearchFocus2 = false;

            IsSeasonSelected = false;
            TextSeason = "Select season";

            IsFocusScr1 = false;
            IsFocusScr2 = false;

            IsSearchSeasonFocus = false;

            Anime = new();
            Anime2 = new();
            Anime3 = new();
            Manga2 = new();
            Manga = new();

            // Inicializa lo requerido para el mopup
            AnimeMopupViewModel = new();
            AnimeMopup = new AnimeMopup(AnimeMopupViewModel);

            MangaMopupViewModel = new();
            MangaMopup = new(MangaMopupViewModel);
        }

        [RelayCommand]
        public async Task ShowHideScroll(string action)
        {
            switch (action)
            {
                case "ShowTopA": IsFocusScr1 = true; break;
                case "HideTopA": IsFocusScr1 = false; break;
                case "ShowTopM": IsFocusScr2 = true; break;
                case "HideTopM": IsFocusScr2 = false; break;

                default: break;
            }
        }

        [RelayCommand]
        public async Task ChangeSeason(string season)
        {
            if (IsSeasonSelected)
            {
                TextSeason = "";
                ImageSeason = ImageSource.FromFile(season);

                await GetSeason(season);
                IsSeasonSelected = false;
            }
            else
            {
                IsSeasonSelected = false;
                IsSeasonSelected = true;
            }
        }

        [RelayCommand]
        public async Task GetSeason(string season)
        {
            var year = IsSeasonSelected ? "2024" : season.Split('-')[0];
            var seasonSplit = IsSeasonSelected ? season.Split(".")[0] : season.Split('-')[1].ToLower();

            var requestPagina = new HttpRequestModel(url: IConstantes.BaseAnimeManga,
                endpoint: $"seasons/{year}/{seasonSplit}",
                parameters: null,
                headers: new Dictionary<string, string> { { "Accept", "application/json" } });

            var pagtrend = (PageA)await HttpService.ExecuteRequestAsync<PageA>(requestPagina); // v

            foreach (var item in pagtrend.Data)
            {
                item.Imagen = item.Images.Jpg.Image_url;
                item.Color = MiscellaneousUtils.GetColorHobbie("Anime");
            }
            pagtrend.Data = MiscellaneousUtils.GetNelements(pagtrend.Data, 10);

            PaginaS = pagtrend;
        }

        /// <summary>
        /// Abre el mopup
        /// </summary>
        /// <param name="id">Id de la pelicula seleccionada</param>
        [RelayCommand]
        public async Task ShowAnimeMopup(string id)
        {
            AnimeMopupViewModel.SendHobbieById(id, UsuarioActivo.Id);
            await MopupService.Instance.PushAsync(AnimeMopup);
            //await Hide();
        }

        /// <summary>
        /// Abre el mopup
        /// </summary>
        /// <param name="id">Id de la pelicula seleccionada</param>
        [RelayCommand]
        public async Task ShowMangaMopup(string id)
        {
            MangaMopupViewModel.SendHobbieById(id, UsuarioActivo.Id);
            await MopupService.Instance.PushAsync(MangaMopup);
            //await Hide();
        }

        /// <summary>
        /// Busca las peliculas que coincidan con un termino
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task GetSearchA(string busqueda)
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

            var pagtrend = (PageA)await HttpService.ExecuteRequestAsync<PageA>(requestPagina); // v
            foreach (var item in pagtrend.Data)
            {
                item.Imagen = item.Images.Jpg.Image_url;
            }

            PaginaAux = pagtrend;
            IsSearchFocus = true;
        }

        /// <summary>
        /// Busca las peliculas que coincidan con un termino
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task GetSearchM(string busqueda)
        {
            if (busqueda.IsNullOrEmpty())
            {
                await HideM();
                return;
            }

            var requestPagina = new HttpRequestModel(url: IConstantes.BaseAnimeManga,
                endpoint: $"manga",
                parameters: new Dictionary<string, string> { { "q", busqueda } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" } });

            var pagtrend = (PageMa)await HttpService.ExecuteRequestAsync<PageMa>(requestPagina); // v
            foreach (var item in pagtrend.Data)
            {
                item.Imagen = item.Images.Jpg.Image_url;
            }

            PaginaAux2 = pagtrend;
            IsSearchFocus2 = true;
        }

        [RelayCommand]
        public async Task Hide()
        {
            IsSearchFocus = false;
        }

        [RelayCommand]
        public async Task HideM()
        {
            IsSearchFocus2 = false;
        }

        [RelayCommand]
        public async Task ShowSeasonTool()
        {
            if (!IsSeasonSelected)
            {
                IsSearchSeasonFocus = true;
            }
        }

        [RelayCommand]
        public async Task HideSeasonTool()
        {
            IsSearchSeasonFocus = false;
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
