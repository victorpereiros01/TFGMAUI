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
        private PageG paginaAux;

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
        /// Obtiene las peliculas trending de moviedb
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [RelayCommand]
        public async Task GetTrendG()
        {
            long start_date = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds();
            var rand = new Random().Next(5, 30);

            var requestPagina = new HttpRequestModel(url: IConstantes.BaseGames,
                endpoint: $"games",
                parameters: null,
                headers: new Dictionary<string, string> { { "Client-ID", IConstantes.client_id }, { "Authorization", $"Bearer {Bearer}" }, { "Accept", "application/json" } },
                body: $"""
                fields id, name, first_release_date, rating, rating_count,cover;
                sort popularity desc;
                where first_release_date >= {start_date} & rating_count > {rand};
                limit 20;
                """);

            try
            {
                var listTrend = (List<GameModel>)await HttpService.ExecuteRequestAsync<List<GameModel>>(requestPagina);
                foreach (var item in listTrend)
                {
                    item.Imagen = await GetImage(item.Cover);
                    item.Color = MiscellaneousUtils.GetColorHobbie("Game")[0];
                    item.Color2 = MiscellaneousUtils.GetColorHobbie("Game")[1];
                }

                PageG pageG = new() { Items = listTrend, Total = listTrend.Count, Pages = Math.DivRem(listTrend.Count, 20, out int str) }; pageG.Items = MiscellaneousUtils.GetNelements(pageG.Items, 5);


                PaginaTrendG = pageG;
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
                headers: new Dictionary<string, string> { { "Client-ID", IConstantes.client_id }, { "Authorization", $"Bearer {Bearer}" }, { "Accept", "application/json" } },
                body: $"""
                fields *;
                search "{busqueda}";           
                """);

            try
            {
                var listTrend = (List<GameModel>)await HttpService.ExecuteRequestAsync<List<GameModel>>(requestPagina);
                foreach (var item in listTrend)
                {
                    item.Imagen = await GetImage(item.Cover);
                    item.Color = MiscellaneousUtils.GetColorHobbie("Game")[0];
                    item.Color2 = MiscellaneousUtils.GetColorHobbie("Game")[1];
                }

                PageG pageG = new() { Items = listTrend, Total = listTrend.Count, Pages = Math.DivRem(listTrend.Count, 20, out int str) };

                PaginaAux = pageG;
                IsSearchFocus = true;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Saludos", e.ToString(), "Aceptar");
            }
        }

        private async Task<string> GetImage(int cover)
        {
            var requestPagina = new HttpRequestModel(url: "https://api.igdb.com/v4/covers",
                endpoint: "",
                parameters: null,
                headers: new Dictionary<string, string> { { "Client-ID", IConstantes.client_id }, { "Authorization", $"Bearer {Bearer}" }, { "Accept", "application/json" } },
                body: $"""
                fields *;
                where id={cover};
                """);

            try
            {
                var images = (List<CoverModel>)await HttpService.ExecuteRequestAsync<List<CoverModel>>(requestPagina);

                var split = images[0].Url.Split("/");
                split[6] = "t_cover_big_2x";
                var duplicado = "https:/";

                for (int i = 0; i < split.Length; i++)
                {
                    if (i > 1)
                    {
                        duplicado += "/" + split[i];
                    }
                }

                return duplicado;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Saludos", e.ToString(), "Aceptar");
            }

            return null;
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
