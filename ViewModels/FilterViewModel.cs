using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using TFGMaui.Models;
using TFGMaui.Services;
using TFGMaui.ViewModels.Mopup;
using TFGMaui.Views.Mopups;
using static TFGMaui.Utils.SortByExtensionsUtils;
using Page = TFGMaui.Models.Page;

namespace TFGMaui.ViewModels
{
    [QueryProperty("UsuarioActivo", "UsuarioActivo")]
    [QueryProperty("PaginaPelis", "PaginaPelis")]
    internal partial class FilterViewModel : ObservableObject
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        [ObservableProperty]
        private int selectedPage, ultimoNumero;

        [ObservableProperty]
        private Page paginaPelis;

        private readonly MovieMopup MovieMopup;
        private readonly MovieMopupViewModel MovieMopupViewModel;

        [ObservableProperty]
        private MovieModel movie;

        [ObservableProperty]
        private string type;

        [ObservableProperty]
        private int primeraFecha, segundaFecha;

        [ObservableProperty]
        private List<SortBy> sortByList;

        [ObservableProperty]
        private SortBy valueSort;

        public FilterViewModel()
        {
            SelectedPage = 1;
            UltimoNumero = 500;
            Type = "day";
            // Inicializa lo requerido para el mopup
            MovieMopupViewModel = new MovieMopupViewModel();
            MovieMopup = new MovieMopup(MovieMopupViewModel);

            PrimeraFecha = 1;
            SegundaFecha = 1;
            // Rellena la lista para los filtros
            SortByList = Enum.GetValues(typeof(SortBy)).Cast<SortBy>().ToList();
            ValueSort = SortByList[0];
        }

        /// <summary>
        /// Busca las peliculas que coincidan con un termino
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task GetSearch(string busqueda)
        {
            try
            {
                var requestPagina = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: $"search/movie",
                parameters: new Dictionary<string, string> { { "query", busqueda }, { "api_key", IConstantes.MovieDB_ApiKey }, { "language", UsuarioActivo.Language } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

                var pagtrend = (Page)await HttpService.ExecuteRequestAsync<Page>(requestPagina); // v

                pagtrend.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);

                PaginaPelis = pagtrend;
            }
            catch { }
        }

        [RelayCommand]
        public async Task FiltrarFecha()
        {
            var prim = PrimeraFecha + 1873;
            var seg = Math.Abs(SegundaFecha - 151) + 1 + 1873;
            await Application.Current.MainPage.DisplayAlert("Saludos", prim + "seg" + seg, "Aceptar");

            try
            {
                var requestPagina = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: $"discover/movie",
                parameters: new Dictionary<string, string> {
                    { "api_key", IConstantes.MovieDB_ApiKey },
                    { "primary_release_date.gte", String.Format(new DateTime(day: 1, month: 1, year: prim).ToString(), "yyyy-mm-dd") },
                    { "primary_release_date.lte", String.Format(new DateTime(day: 31, month: 12, year: seg).ToString(), "yyyy-mm-dd") },
                    { "language", UsuarioActivo.Language },
                    { "page", SelectedPage.ToString() },
                    { "sort_by",ValueSort.ToQueryString()}
                },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } }); ;

                var pagtrend = (Page)await HttpService.ExecuteRequestAsync<Page>(requestPagina); // v

                pagtrend.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);

                PaginaPelis = pagtrend;
            }
            catch { }
        }

        /// <summary>
        /// Obtiene las peliculas trending de moviedb
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task GetTrending()
        {
            try
            {
                var requestPagina = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: $"trending/movie/{Type}",
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", UsuarioActivo.Language }, { "page", SelectedPage.ToString() } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

                var pagtrend = (Page)await HttpService.ExecuteRequestAsync<Page>(requestPagina); // v

                pagtrend.Results.ToList().ForEach(x => x.Imagen = "https://image.tmdb.org/t/p/original" + x.Imagen);

                PaginaPelis = pagtrend;
            }
            catch { }
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
        public async Task ShowMovieMopup(string id)
        {
            MovieMopupViewModel.SendHobbieById(id, UsuarioActivo.Id, UsuarioActivo.Language);
            await MopupService.Instance.PushAsync(MovieMopup);
        }

        [RelayCommand]
        public async Task FirstPage()
        {
            if (SelectedPage == 1)
            {
                return;
            }

            SelectedPage = 1;
            await GetTrending();
        }

        [RelayCommand]
        public async Task PreviousPage()
        {
            if (SelectedPage > 1)
            {
                SelectedPage -= 1;
                await GetTrending();
            }
        }

        [RelayCommand]
        public async Task NextPage()
        {
            if (SelectedPage < UltimoNumero)
            {
                SelectedPage += 1;
                await GetTrending();
            }
        }

        [RelayCommand]
        public async Task LastPage()
        {
            if (SelectedPage == UltimoNumero)
            {
                return;
            }

            SelectedPage = UltimoNumero;
            await GetTrending();
        }
    }
}
