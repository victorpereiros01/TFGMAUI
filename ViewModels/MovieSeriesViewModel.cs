using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.PreBaked.Services;
using Mopups.Services;
using System.Collections.ObjectModel;
using TFGMaui.Models;
using TFGMaui.Repositories;
using TFGMaui.Views.Mopups;
using Color = System.Drawing.Color;
using Page = TFGMaui.Models.Page;

namespace TFGMaui.ViewModels
{
    [QueryProperty("UsuarioActivo", "UsuarioActivo")]
    [QueryProperty("PaginaPelisTop", "PaginaPelisTop")]
    [QueryProperty("PaginaPelis", "PaginaPelis")]
    internal partial class MovieSeriesViewModel : ObservableObject
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        [ObservableProperty]
        private Page paginaPelis;
        [ObservableProperty]
        private Page paginaPelisTop;

        [ObservableProperty]
        private int selectedPage;

        private MovieMopup MovieMopup;
        private MovieMopupViewModel MovieMopupViewModel;

        [ObservableProperty]
        private ObservableCollection<QuoteModel> quotes;

        [ObservableProperty]
        private MovieModel movie;

        public MovieSeriesViewModel()
        {
            // Inicializa lo requerido para el mopup
            MovieMopupViewModel = new MovieMopupViewModel();
            MovieMopup = new MovieMopup(MovieMopupViewModel);
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

        /// <summary>
        /// Abre el mopup de loading
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task ShowLoadingMopup()
        {
            var quote = new HobbieRepository().GetQuoteRandom();
            if (quote is null)
            {
                return;
            }

            List<string> list = [quote.Source, quote.Value];

            // Loader
            await PreBakedMopupService.GetInstance().WrapTaskInLoader(Task.Delay(4000), ColorConverterUtil.ConvertFromSystemDrawingColor(Color.Red), ColorConverterUtil.ConvertFromSystemDrawingColor(Color.White), list, ColorConverterUtil.ConvertFromSystemDrawingColor(Color.Black));
        }
    }
}
