using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Collections.ObjectModel;
using TFGMaui.Models;
using TFGMaui.Repositories;
using TFGMaui.Services;

namespace TFGMaui.ViewModels.Mopup
{
    public partial class BookMopupViewModel : ObservableObject
    {
        [ObservableProperty]
        private BookModel book;

        private int UserId;

        [ObservableProperty]
        private bool isVisibleEditor;

        public BookMopupViewModel()
        {
            IsVisibleEditor = false;
        }

        public void SendHobbieById(string id, int userId)
        {
            UserId = userId;
            Book = new()
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
            var requestPelicula = new HttpRequestModel(url: IConstantes.BaseBooks,
                endpoint: $"volumes/{Book.Id}",
                parameters: new Dictionary<string, string> { { "key", IConstantes.ApiKeyBooks } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" } });

            var m = (BookModel)await HttpService.ExecuteRequestAsync<BookModel>(requestPelicula); // v

            if (m.VolumeInfo.ImageLinks is not null && m.VolumeInfo.ImageLinks.Thumbnail is not null)
            {
                m.Imagen = m.VolumeInfo.ImageLinks.Thumbnail;
            }

            Book = m;
        }

        [RelayCommand]
        public async Task AddHobbie(string type)
        {
            if (new HobbieRepository().AddHobbie(type, UserId, Book.GetType().ToString(), Book.Id))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Hobbie añadido satisfactoriamente", "Aceptar");
            }
        }

        [RelayCommand]
        public async Task RemoveHobbie(string type)
        {
            if (new HobbieRepository().RemoveHobbie(type, UserId, Book.GetType().ToString(), Book.Id))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Hobbie borrado satisfactoriamente", "Aceptar");
            }
        }
    }
}
