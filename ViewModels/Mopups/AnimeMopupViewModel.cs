using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using TFGMaui.Models;
using TFGMaui.Repositories;
using TFGMaui.Services;

namespace TFGMaui.ViewModels.Mopup
{
    public partial class AnimeMopupViewModel : ObservableObject
    {
        [ObservableProperty]
        private AnimeModel anime;

        private int UserId;

        [ObservableProperty]
        private bool isVisibleEditor;

        public AnimeMopupViewModel()
        {
            IsVisibleEditor = false;
        }

        public void SendHobbieById(string id, int userId)
        {
            UserId = userId;
            Anime = new()
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
            var requestPelicula = new HttpRequestModel(url: IConstantes.BaseAnimeManga,
                endpoint: $"anime/{Anime.Id}/full",
                parameters: null,
                headers: new Dictionary<string, string> { { "Accept", "application/json" } });

            var m = (AnimeData)await HttpService.ExecuteRequestAsync<AnimeData>(requestPelicula); // v
            m.Data.Imagen = m.Data.Images.Jpg.Image_url;

            Anime = m.Data;
        }

        [RelayCommand]
        public async Task AddHobbie(string type)
        {
            if (new HobbieRepository().AddHobbie(type, UserId, "Anime", new HobbieModel() { Id = Anime.Id, Imagen = Anime.Imagen, Title = Anime.Title }))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Hobbie añadido satisfactoriamente", "Aceptar");
            }
        }

        [RelayCommand]
        public async Task RemoveHobbie(string type)
        {
            if (new HobbieRepository().RemoveHobbie(type, UserId, Anime.GetType().ToString(), Anime.Id))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Hobbie borrado satisfactoriamente", "Aceptar");
            }
        }
    }
}
