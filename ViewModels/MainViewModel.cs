using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using TFGMaui.Models;
using TFGMaui.Services;
using TFGMaui.Utils;
using Page = TFGMaui.Models.Page;

namespace TFGMaui.ViewModels
{
    [QueryProperty("UsuarioActivo", "UsuarioActivo")]
    internal partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        [ObservableProperty]
        private string saludos;

        [ObservableProperty]
        private Page paginaT;

        [ObservableProperty]
        private int hobbieWidth;

        /// <summary>
        /// Inicializa los saludos, con el dia en formato dia de la semana, numero y mes. Y obtiene las listas de trending y top
        /// </summary>
        [RelayCommand]
        public async Task InitializeComponents()
        {
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

            await GetTrending("day");
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
                var pagtrend = (Page)await HttpService.ExecuteRequestAsync<Page>(requestPagina);

                pagtrend.Results = MiscellaneousUtils.GetNelements(pagtrend.Results, 6);
                pagtrend.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);

                PaginaT = pagtrend;
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
        public async Task CloseApp()
        {
            Application.Current.Quit();
        }

        [RelayCommand]
        public async Task Navegar(string pagina)
        {
            await Shell.Current.GoToAsync("//" + pagina, new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioActivo,
                ["PaginaT"] = PaginaT
            });
        }

        [RelayCommand]
        public async Task BtnEntered()
        {
            //await Application.Current.MainPage.DisplayAlert("Saludos", relativeToContainerPosition.ToString(), "Aceptar");
        }
    }
}
