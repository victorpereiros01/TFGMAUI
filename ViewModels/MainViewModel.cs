﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        /// <summary>
        /// Inicializa los saludos, con el dia en formato dia de la semana, numero y mes. Y obtiene las listas de trending y top
        /// </summary>
        [RelayCommand]
        public async Task InitializeComponents()
        {
            //IsGuest = UsuarioActivo.Guest;

            switch (DateTime.Now.Month)
            {
                case 12 or 1 or 2:
                    Season = ImageSource.FromFile("copo.png");
                    break;
                case 3 or 4 or 5:
                    Season = ImageSource.FromFile("flor.png");
                    break;
                case 6 or 7 or 8:
                    Season = ImageSource.FromFile("sol.png");
                    break;
                case 9 or 10 or 11:
                    Season = ImageSource.FromFile("hoja.png");
                    break;
            }

            ListFav = new SavedHobbiesRepository().GetFavorites(UsuarioActivo.Id);
            ListSeen = new SavedHobbiesRepository().GetSeen(UsuarioActivo.Id);
            ListPend = new SavedHobbiesRepository().GetPending(UsuarioActivo.Id);

            Bearer = await GetBearerG();
            Saludos = "Have a nice " + new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).ToString("dddd, d MMM", CultureInfo.InvariantCulture);

            var hobbieC = 0;
            foreach (var item in UsuarioActivo.Hobbies)
            {
                if (item)
                {
                    hobbieC += 1;
                }
            }
            HobbieWidth = 1140 / hobbieC - 20;

            SavF = ListFav is null ? new() : ListFav[0];
            SavS = ListSeen is null ? new() : ListSeen[0];
            SavP = ListPend is null ? new() : ListPend[0];

            await GetTrending("day");
            await GetTopAM();

            await GetTrendG();
            await GetTopG();
        }

        /// <summary>
        /// Abre el mopup
        /// </summary>
        /// <param name="id">Id de la pelicula seleccionada</param>
        [RelayCommand]
        public async Task ShowMoviePMopup(string id)
        {
            if (SavP.HobbieType.Equals("Movie"))
            {
                MovieMopupViewModel MovieMopupViewModel = new();
                MovieMopup MovieMopup = new(MovieMopupViewModel);

                MovieMopupViewModel.SendHobbieById(id, UsuarioActivo.Id, UsuarioActivo.Language);
                await MopupService.Instance.PushAsync(MovieMopup);
            }
            else if (SavP.HobbieType.Equals("Serie"))
            {
                SerieMopupViewModel SerieMopupViewModel = new();
                SerieMopup SerieMopup = new(SerieMopupViewModel);

                SerieMopupViewModel.SendHobbieById(id, UsuarioActivo.Id, UsuarioActivo.Language);
                await MopupService.Instance.PushAsync(SerieMopup);
            }
            else if (SavP.HobbieType.Equals("Manga"))
            {
                MangaMopupViewModel MangaMopupViewModel = new();
                MangaMopup MangaMopup = new(MangaMopupViewModel);

                MangaMopupViewModel.SendHobbieById(id, UsuarioActivo.Id);
                await MopupService.Instance.PushAsync(MangaMopup);
            }
            else if (SavP.HobbieType.Equals("Anime"))
            {
                AnimeMopupViewModel AnimeMopupViewModel = new();
                AnimeMopup AnimeMopup = new(AnimeMopupViewModel);

                AnimeMopupViewModel.SendHobbieById(id, UsuarioActivo.Id);
                await MopupService.Instance.PushAsync(AnimeMopup);
            }
            else if (SavP.HobbieType.Equals("Game"))
            {
                GameMopupViewModel GameMopupViewModel = new();
                GameMopup GameMopup = new(GameMopupViewModel);

                GameMopupViewModel.SendHobbieById(id, UsuarioActivo.Id, Bearer);
                await MopupService.Instance.PushAsync(GameMopup);
            }
            else if (SavP.HobbieType.Equals("Book"))
            {
                BookMopupViewModel BookMopupViewModel = new();
                BookMopup BookMopup = new(BookMopupViewModel);

                BookMopupViewModel.SendHobbieById(id, UsuarioActivo.Id);
                await MopupService.Instance.PushAsync(BookMopup);
            }
        }

        /// <summary>
        /// Abre el mopup
        /// </summary>
        /// <param name="id">Id de la pelicula seleccionada</param>
        [RelayCommand]
        public async Task ShowMovieFMopup(string id)
        {
            if (SavF.HobbieType.Equals("Movie"))
            {
                MovieMopupViewModel MovieMopupViewModel = new();
                MovieMopup MovieMopup = new(MovieMopupViewModel);

                MovieMopupViewModel.SendHobbieById(id, UsuarioActivo.Id, UsuarioActivo.Language);
                await MopupService.Instance.PushAsync(MovieMopup);
            }
            else if (SavF.HobbieType.Equals("Serie"))
            {
                SerieMopupViewModel SerieMopupViewModel = new();
                SerieMopup SerieMopup = new(SerieMopupViewModel);

                SerieMopupViewModel.SendHobbieById(id, UsuarioActivo.Id, UsuarioActivo.Language);
                await MopupService.Instance.PushAsync(SerieMopup);
            }
            else if (SavF.HobbieType.Equals("Manga"))
            {
                MangaMopupViewModel MangaMopupViewModel = new();
                MangaMopup MangaMopup = new(MangaMopupViewModel);

                MangaMopupViewModel.SendHobbieById(id, UsuarioActivo.Id);
                await MopupService.Instance.PushAsync(MangaMopup);
            }
            else if (SavF.HobbieType.Equals("Anime"))
            {
                AnimeMopupViewModel AnimeMopupViewModel = new();
                AnimeMopup AnimeMopup = new(AnimeMopupViewModel);

                AnimeMopupViewModel.SendHobbieById(id, UsuarioActivo.Id);
                await MopupService.Instance.PushAsync(AnimeMopup);
            }
            else if (SavF.HobbieType.Equals("Game"))
            {
                GameMopupViewModel GameMopupViewModel = new();
                GameMopup GameMopup = new(GameMopupViewModel);

                GameMopupViewModel.SendHobbieById(id, UsuarioActivo.Id, Bearer);
                await MopupService.Instance.PushAsync(GameMopup);
            }
            else if (SavF.HobbieType.Equals("Book"))
            {
                BookMopupViewModel BookMopupViewModel = new();
                BookMopup BookMopup = new(BookMopupViewModel);

                BookMopupViewModel.SendHobbieById(id, UsuarioActivo.Id);
                await MopupService.Instance.PushAsync(BookMopup);
            }
        }

        /// <summary>
        /// Abre el mopup
        /// </summary>
        /// <param name="id">Id de la pelicula seleccionada</param>
        [RelayCommand]
        public async Task ShowMovieSMopup(string id)
        {
            if (SavS.HobbieType.Equals("Movie"))
            {
                MovieMopupViewModel MovieMopupViewModel = new();
                MovieMopup MovieMopup = new(MovieMopupViewModel);

                MovieMopupViewModel.SendHobbieById(id, UsuarioActivo.Id, UsuarioActivo.Language);
                await MopupService.Instance.PushAsync(MovieMopup);
            }
            else if (SavS.HobbieType.Equals("Serie"))
            {
                SerieMopupViewModel SerieMopupViewModel = new();
                SerieMopup SerieMopup = new(SerieMopupViewModel);

                SerieMopupViewModel.SendHobbieById(id, UsuarioActivo.Id, UsuarioActivo.Language);
                await MopupService.Instance.PushAsync(SerieMopup);
            }
            else if (SavS.HobbieType.Equals("Manga"))
            {
                MangaMopupViewModel MangaMopupViewModel = new();
                MangaMopup MangaMopup = new(MangaMopupViewModel);

                MangaMopupViewModel.SendHobbieById(id, UsuarioActivo.Id);
                await MopupService.Instance.PushAsync(MangaMopup);
            }
            else if (SavS.HobbieType.Equals("Anime"))
            {
                AnimeMopupViewModel AnimeMopupViewModel = new();
                AnimeMopup AnimeMopup = new(AnimeMopupViewModel);

                AnimeMopupViewModel.SendHobbieById(id, UsuarioActivo.Id);
                await MopupService.Instance.PushAsync(AnimeMopup);
            }
            else if (SavS.HobbieType.Equals("Game"))
            {
                GameMopupViewModel GameMopupViewModel = new();
                GameMopup GameMopup = new(GameMopupViewModel);

                GameMopupViewModel.SendHobbieById(id, UsuarioActivo.Id, Bearer);
                await MopupService.Instance.PushAsync(GameMopup);
            }
            else if (SavS.HobbieType.Equals("Book"))
            {
                BookMopupViewModel BookMopupViewModel = new();
                BookMopup BookMopup = new(BookMopupViewModel);

                BookMopupViewModel.SendHobbieById(id, UsuarioActivo.Id);
                await MopupService.Instance.PushAsync(BookMopup);
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
                limit 8;
                """);

            try
            {
                var listTrend = (List<GameModel>)await HttpService.ExecuteRequestAsync<List<GameModel>>(requestPagina);
                foreach (var item in listTrend)
                {
                    item.Imagen = await GetImage(item.Cover);
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
                where rating_count > 200 & rating != null & rating_count != null;
                limit 8;
                """);

            try
            {
                var listTrend = (List<GameModel>)await HttpService.ExecuteRequestAsync<List<GameModel>>(requestPagina);
                foreach (var item in listTrend)
                {
                    item.Imagen = await GetImage(item.Cover);
                }

                PageG pageG = new() { Items = listTrend, Total = listTrend.Count, Pages = Math.DivRem(listTrend.Count, 20, out int str) };
                foreach (var item in pageG.Items)
                {
                    item.Color = MiscellaneousUtils.GetColorHobbie("Game");
                }

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
            pagtrend.Data = MiscellaneousUtils.GetNelements(pagtrend.Data, 3);
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
            pagtrend2.Data = MiscellaneousUtils.GetNelements(pagtrend2.Data, 3);
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

        [RelayCommand]
        public async Task BtnEntered()
        {
            //await Application.Current.MainPage.DisplayAlert("Saludos", relativeToContainerPosition.ToString(), "Aceptar");
        }
    }
}
