using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Mopups.Services;
using System.Globalization;
using TFGMaui.Models;
using TFGMaui.Repositories;
using TFGMaui.Services;
using TFGMaui.Utils;
using TFGMaui.ViewModels.Mopup;
using TFGMaui.Views.Mopups;

namespace TFGMaui.ViewModels
{
    [QueryProperty("UsuarioActivo", "UsuarioActivo")]
    [QueryProperty("IsGuest", "IsGuest")]
    internal partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        [ObservableProperty]
        private string saludos;

        [ObservableProperty]
        private PageM paginaTrendMovSerie;

        [ObservableProperty]
        private PageA paginaTopAnime;

        [ObservableProperty]
        private PageMa paginaTopManga;

        [ObservableProperty]
        private PageA paginaS;

        [ObservableProperty]
        private PageG paginaTopG, paginaTrendG;

        [ObservableProperty]
        private List<SavedHobbieModel> listFav, listSeen, listPend;

        [ObservableProperty]
        private SavedHobbieModel savF, savP, savS;

        [ObservableProperty]
        private int hobbieWidth;

        public string Bearer { get; set; }

        [ObservableProperty]
        private ImageSource season;

        [ObservableProperty]
        private bool isGuest;

        [ObservableProperty]
        private bool cVis, vVis, mVis, lVis;

        /// <summary>
        /// Inicializa los saludos, con el dia en formato dia de la semana, numero y mes. Y obtiene las listas de trending y top
        /// </summary>
        [RelayCommand]
        public async Task InitializeComponents()
        {
            Saludos = "Ten un buen " + DateTime.Now.ToString("dddd, d MMM", CultureInfo.CurrentCulture);

            switch (DateTime.Now.Month)
            {
                case 12 or 1 or 2:
                    Season = ImageSource.FromFile("winter.png");
                    break;
                case 3 or 4 or 5:
                    Season = ImageSource.FromFile("spring.png");
                    break;
                case 6 or 7 or 8:
                    Season = ImageSource.FromFile("summer.png");
                    break;
                case 9 or 10 or 11:
                    Season = ImageSource.FromFile("fall.png");
                    break;
            }

            await GetHobbies();

            Bearer = await GetBearerG();

            var hobbieC = 0;
            foreach (var item in UsuarioActivo.Hobbies)
            {
                if (item)
                {
                    hobbieC += 1;
                }
            }
            HobbieWidth = 1485 / hobbieC - 20;
            CVis = UsuarioActivo.Hobbies[0];
            VVis = UsuarioActivo.Hobbies[1];
            MVis = UsuarioActivo.Hobbies[2];
            LVis = UsuarioActivo.Hobbies[3];

            await GetTrending("day");
            await GetTopAM();
            await GetTopG();
            await GetTrendG();
        }

        [RelayCommand]
        public async Task AbrirTiempo()
        {
            Uri uri = new("https://www.msn.com/es-ES/eltiempo"); await Application.Current.MainPage.DisplayAlert("Abriendo tiempo", "Se esta abriendo una pagina en tu navegador", "Aceptar");
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }

        public async Task GetHobbies()
        {
            ListFav = new HobbieRepository().GetFavorites(UsuarioActivo.Id);
            ListSeen = new HobbieRepository().GetSeen(UsuarioActivo.Id);
            ListPend = new HobbieRepository().GetPending(UsuarioActivo.Id);

            SavF = ListFav.IsNullOrEmpty() ? new() : ListFav[0];
            SavS = ListSeen.IsNullOrEmpty() ? new() : ListSeen[0];
            SavP = ListPend.IsNullOrEmpty() ? new() : ListPend[0];
        }

        [RelayCommand]
        public async Task ShowMoviePMopup(string id) => await ShowMopup(id, SavP);

        [RelayCommand]
        public async Task ShowMovieFMopup(string id) => await ShowMopup(id, SavF);

        [RelayCommand]
        public async Task ShowMovieSMopup(string id) => await ShowMopup(id, SavS);

        /// <summary>
        /// Abre el mopup
        /// </summary>
        /// <param name="id">Id de la pelicula seleccionada</param>
        /// <param name="source">Source del hobby (SavP, SavF, SavS)</param>
        public async Task ShowMopup(string id, object source)
        {
            if (id is null)
            {
                return;
            }

            switch (((SavedHobbieModel)source).HobbieType)
            {
                case "Movie":
                    {
                        var movieViewModel = new MovieMopupViewModel();
                        var movieMopup = new MovieMopup(movieViewModel);
                        movieViewModel.SendHobbieById(id, UsuarioActivo.Id, UsuarioActivo.Language);
                        await MopupService.Instance.PushAsync(movieMopup);
                        break;
                    }

                case "Serie":
                    {
                        var serieViewModel = new SerieMopupViewModel();
                        var serieMopup = new SerieMopup(serieViewModel);
                        serieViewModel.SendHobbieById(id, UsuarioActivo.Id, UsuarioActivo.Language);
                        await MopupService.Instance.PushAsync(serieMopup);
                        break;
                    }

                case "Manga":
                    {
                        var mangaViewModel = new MangaMopupViewModel();
                        var mangaMopup = new MangaMopup(mangaViewModel);
                        mangaViewModel.SendHobbieById(id, UsuarioActivo.Id);
                        await MopupService.Instance.PushAsync(mangaMopup);
                        break;
                    }

                case "Anime":
                    {
                        var animeViewModel = new AnimeMopupViewModel();
                        var animeMopup = new AnimeMopup(animeViewModel);
                        animeViewModel.SendHobbieById(id, UsuarioActivo.Id);
                        await MopupService.Instance.PushAsync(animeMopup);
                        break;
                    }

                case "Game":
                    {
                        var gameViewModel = new GameMopupViewModel();
                        var gameMopup = new GameMopup(gameViewModel);
                        gameViewModel.SendHobbieById(id, UsuarioActivo.Id, Bearer);
                        await MopupService.Instance.PushAsync(gameMopup);
                        break;
                    }

                case "Book":
                    {
                        var bookViewModel = new BookMopupViewModel();
                        var bookMopup = new BookMopup(bookViewModel);
                        bookViewModel.SendHobbieById(id, UsuarioActivo.Id);
                        await MopupService.Instance.PushAsync(bookMopup);
                        break;
                    }

                default:
                    break;
            }
        }

        private async Task<string> GetBearerG()
        {
            var requestPagina = new HttpRequestModel(url: "https://id.twitch.tv/oauth2/token",
                endpoint: "",
                parameters: new Dictionary<string, string> { { "client_id", IConstantes.client_id }, { "client_secret", IConstantes.client_secret }, { "grant_type", "client_credentials" } },
                headers: null, body: "");

            try
            {
                var b = (BearerModel)await HttpService.ExecuteRequestAsync<BearerModel>(requestPagina);
                return b.AccessToken;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Saludos", e.ToString(), "Aceptar");
            }

            return null;
        }

        /// <summary>
        /// Obtiene las peliculas trending de moviedb
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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
                    item.Color = MiscellaneousUtils.GetColorHobbie("Game");
                }

                PageG pageG = new() { Items = listTrend, Total = listTrend.Count, Pages = Math.DivRem(listTrend.Count, 20, out int str) };

                PaginaTrendG = pageG;
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

        /// <summary>
        /// Obtiene las peliculas trending de moviedb
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task GetTopG()
        {
            var requestPagina = new HttpRequestModel(url: IConstantes.BaseGames,
                endpoint: $"games",
                parameters: null,
                headers: new Dictionary<string, string> { { "Client-ID", IConstantes.client_id }, { "Authorization", $"Bearer {Bearer}" }, { "Accept", "application/json" } },
                body: """
                fields id, cover, name, rating, rating_count;
                sort rating desc;
                where rating_count > 200 & rating != null & rating_count != null & version_parent = null;
                limit 20;
                """);

            try
            {
                var listTrend = (List<GameModel>)await HttpService.ExecuteRequestAsync<List<GameModel>>(requestPagina);
                foreach (var item in listTrend)
                {
                    item.Imagen = await GetImage(item.Cover);
                    item.Color = MiscellaneousUtils.GetColorHobbie("Game");
                }

                PageG pageG = new() { Items = listTrend, Total = listTrend.Count, Pages = Math.DivRem(listTrend.Count, 20, out int str) };

                PaginaTopG = pageG;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Saludos", e.ToString(), "Aceptar");
            }
        }

        private async Task GetTopAM()
        {
            var requestPaginas = new HttpRequestModel(url: IConstantes.BaseAnimeManga,
                endpoint: $"seasons/now",
                parameters: new Dictionary<string, string> { /*{ "filter", "tv" },*/ { "page", 1.ToString() } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" } });

            try
            {
                var pagseason = (PageA)await HttpService.ExecuteRequestAsync<PageA>(requestPaginas);

                pagseason.Data = MiscellaneousUtils.GetNelements(pagseason.Data, 8);
                foreach (var item in pagseason.Data)
                {
                    item.Imagen = item.Images.Jpg.Image_url;
                    item.Color = MiscellaneousUtils.GetColorHobbie("Anime");
                }

                PaginaS = pagseason;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Saludos", e.ToString(), "Aceptar");
            }

            var requestPagina = new HttpRequestModel(url: IConstantes.BaseAnimeManga,
                endpoint: $"top/manga",
                parameters: null,
                headers: new Dictionary<string, string> { { "Accept", "application/json" } });

            var pagtrend = (PageMa)await HttpService.ExecuteRequestAsync<PageMa>(requestPagina); // v

            foreach (var item in pagtrend.Data)
            {
                item.Imagen = item.Images.Jpg.Image_url;
                item.Color = MiscellaneousUtils.GetColorHobbie("Manga");
            }

            PaginaTopManga = pagtrend;

            var requestPagina2 = new HttpRequestModel(url: IConstantes.BaseAnimeManga,
                endpoint: $"top/anime",
                parameters: null,
                headers: new Dictionary<string, string> { { "Accept", "application/json" } });

            var pagtrend2 = (PageA)await HttpService.ExecuteRequestAsync<PageA>(requestPagina2); // v

            foreach (var item in pagtrend2.Data)
            {
                item.Imagen = item.Images.Jpg.Image_url;
                item.Color = MiscellaneousUtils.GetColorHobbie("Anime");
            }

            PaginaTopAnime = pagtrend2;
        }

        /// <summary>
        /// Obtiene las peliculas trending de moviedb
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task GetTrending(string type)
        {
            var requestPagina = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: $"trending/all/{type}",
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", UsuarioActivo.Language }, { "page", 1.ToString() } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            try
            {
                var pagtrend = (PageM)await HttpService.ExecuteRequestAsync<PageM>(requestPagina);

                pagtrend.Results = MiscellaneousUtils.GetNelements(pagtrend.Results, 14);
                pagtrend.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);
                foreach (var item in pagtrend.Results)
                {
                    item.Color = MiscellaneousUtils.GetColorHobbie(item.MediaType.Equals("tv") ? "Serie" : "Movie");
                }

                PaginaTrendMovSerie = pagtrend;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Saludos", e.ToString(), "Aceptar");
            }
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
        public async Task CloseApp() => Application.Current.Quit();

        [RelayCommand]
        public async Task Navegar(string pagina)
        {
            if (pagina.Equals("LoginPage"))
            {
                await SecureStorage.SetAsync("credentialsStored", false.ToString());
                await SecureStorage.SetAsync("username", " ");
                await SecureStorage.SetAsync("password", " ");
                UsuarioActivo = new();
            }
            else if (pagina.Equals("MainPage"))
            {
                await GetHobbies();
                //
            }

            await Shell.Current.GoToAsync("//" + pagina, new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioActivo,
                ["PaginaT"] = PaginaTrendMovSerie,
                ["PaginaTopAnime"] = PaginaTopAnime,
                ["PaginaS"] = PaginaS,
                ["PaginaTopManga"] = PaginaTopManga
            });
        }

        [RelayCommand]
        public async Task NavegarG(string pagina)
        {
            await Shell.Current.GoToAsync("//" + pagina, new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioActivo,
                ["PaginaTopG"] = PaginaTopG,
                ["PaginaTrendG"] = PaginaTrendG
            });
        }
    }
}
