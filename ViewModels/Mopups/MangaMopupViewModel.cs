using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Collections.ObjectModel;
using TFGMaui.Models;
using TFGMaui.Repositories;
using TFGMaui.Services;

namespace TFGMaui.ViewModels.Mopup
{
    public partial class MangaMopupViewModel : ObservableObject
    {
        [ObservableProperty]
        private MovieModel movie;

        private int UserId;

        [ObservableProperty]
        private bool isVisibleEditor;

        [ObservableProperty]
        private string lang;

        public MangaMopupViewModel()
        {
            IsVisibleEditor = false;
        }

        public void SendHobbieById(string id, int userId, string lang)
        {
            UserId = userId;
            Lang = lang;
            Movie = new()
            {
                Id = id
            };
            _ = GetMovieDetails();
        }

        [RelayCommand]
        public async Task CambiarEditor()
        {
            IsVisibleEditor = !IsVisibleEditor;
        }

        /// <summary>
        /// Cierra el mopup
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task CloseInfoMopup() => await MopupService.Instance.PopAllAsync();

        /// <summary>
        /// Obtiene los detalles de la pelicula
        /// </summary>
        /// <returns></returns>
        public async Task GetMovieDetails()
        {
            var requestPelicula = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: $"movie/{Movie.Id}",
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", Lang.Trim() } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            var m = (MovieModel)await HttpService.ExecuteRequestAsync<MovieModel>(requestPelicula); // v
            m.Imagen = "https://image.tmdb.org/t/p/original" + m.Imagen;
            Movie = m;
        }

        [RelayCommand]
        public async Task AddHobbie(string type)
        {
            if (new HobbieRepository().AddHobbie(type, UserId, Movie.GetType().ToString(), Movie.Id))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Hobbie añadido satisfactoriamente", "Aceptar");
            }
        }

        [RelayCommand]
        public async Task RemoveHobbie(string type)
        {
            if (new HobbieRepository().RemoveHobbie(type, UserId, Movie.GetType().ToString(), Movie.Id))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Hobbie borrado satisfactoriamente", "Aceptar");
            }
        }
    }
}
