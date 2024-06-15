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
        private MangaModel manga;

        private int UserId;

        [ObservableProperty]
        private bool isVisibleEditor;

        [ObservableProperty]
        private bool isAddedFavorite, isAddedPending, isAddedSeen;

        public MangaMopupViewModel()
        {
            IsVisibleEditor = false;
        }

        public void SendHobbieById(string id, int userId)
        {
            UserId = userId;
            Manga = new()
            {
                Id = id
            };
            _ = GetMangaDetails();

            CheckAdded();
        }

        private void CheckAdded()
        {
            IsAddedFavorite = new HobbieRepository().Exists("Favorite", UserId, "Manga", Manga.Id);
            IsAddedSeen = new HobbieRepository().Exists("Seen", UserId, "Manga", Manga.Id);
            IsAddedPending = new HobbieRepository().Exists("Pending", UserId, "Manga", Manga.Id);
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
        public async Task GetMangaDetails()
        {
            var requestPelicula = new HttpRequestModel(url: IConstantes.BaseAnimeManga,
                 endpoint: $"manga/{Manga.Id}/full",
                 parameters: null,
                 headers: new Dictionary<string, string> { { "Accept", "application/json" } });

            var m = (MangaData)await HttpService.ExecuteRequestAsync<MangaData>(requestPelicula); // v
            m.Data.Imagen = m.Data.Images.Jpg.Image_url;

            Manga = m.Data;
        }

        [RelayCommand]
        public async Task AddHobbie(string type)
        {
            if (new HobbieRepository().AddHobbie(type, UserId, "Manga", new HobbieModel() { Id = Manga.Id, Imagen = Manga.Imagen, Title = Manga.Title }))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Hobbie cambiado satisfactoriamente", "Aceptar");
            }

            CheckAdded();
        }

        [RelayCommand]
        public async Task RemoveHobbie(string type)
        {
            if (new HobbieRepository().RemoveHobbie(type, UserId, Manga.GetType().ToString(), Manga.Id))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Hobbie cambiado satisfactoriamente", "Aceptar");
            }

            CheckAdded();
        }
    }
}
