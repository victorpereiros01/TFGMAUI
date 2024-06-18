using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Collections.ObjectModel;
using TFGMaui.Models;
using TFGMaui.Repositories;
using TFGMaui.Services;
using TFGMaui.Utils;
using Windows.Data.Html;

namespace TFGMaui.ViewModels.Mopup
{
    public partial class BookMopupViewModel : ObservableObject
    {
        [ObservableProperty]
        private BookModel book;

        private int UserId;

        [ObservableProperty]
        private bool isVisibleEditor;

        [ObservableProperty]
        private bool isAddedFavorite, isAddedPending, isAddedSeen;

        private string lang;

        public BookMopupViewModel()
        {
            IsVisibleEditor = false;
        }

        public void SendHobbieById(string id, int userId, string lang)
        {
            this.lang = lang;
            UserId = userId;
            Book = new()
            {
                Id = id
            };
            _ = GetMovieDetails();

            CheckAdded();
        }

        private void CheckAdded()
        {
            IsAddedFavorite = new HobbieRepository().Exists("Favorite", UserId, "Book", Book.Id);
            IsAddedSeen = new HobbieRepository().Exists("Seen", UserId, "Book", Book.Id);
            IsAddedPending = new HobbieRepository().Exists("Pending", UserId, "Book", Book.Id);
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
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "langRestriction", lang } });

            var m = (BookModel)await HttpService.ExecuteRequestAsync<BookModel>(requestPelicula); // v

            if (m.VolumeInfo.ImageLinks.Large is not null)
            {
                m.Imagen = m.VolumeInfo.ImageLinks.Large;
            }
            else if (m.VolumeInfo.ImageLinks.Medium is not null)
            {
                m.Imagen = m.VolumeInfo.ImageLinks.Medium;
            }
            else if (m.VolumeInfo.ImageLinks.Small is not null)
            {
                m.Imagen = m.VolumeInfo.ImageLinks.Small;
            }
            else if (m.VolumeInfo.ImageLinks.Thumbnail is not null)
            {
                m.Imagen = m.VolumeInfo.ImageLinks.Thumbnail;
            }

            m.VolumeInfo.Description = MiscellaneousUtils.HtmlToPlainText(m.VolumeInfo.Description);
            Book = m;
        }

        [RelayCommand]
        public async Task AddHobbie(string type)
        {
            if (new HobbieRepository().AddHobbie(type, UserId, "Book", new HobbieModel() { Id = Book.Id, Imagen = Book.Imagen, Title = Book.VolumeInfo.Title }))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Hobbie cambiado satisfactoriamente", "Aceptar");
            }

            CheckAdded();
        }

        [RelayCommand]
        public async Task RemoveHobbie(string type)
        {
            if (new HobbieRepository().RemoveHobbie(type, UserId, Book.GetType().ToString(), Book.Id))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Hobbie cambiado satisfactoriamente", "Aceptar");
            }

            CheckAdded();
        }
    }
}
