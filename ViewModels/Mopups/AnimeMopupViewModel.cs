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

        [ObservableProperty]
        private bool isAddedFavorite, isAddedPending, isAddedSeen;

        [ObservableProperty]
        private List<ReviewModel> listReviews;

        [ObservableProperty]
        private bool guest;

        public AnimeMopupViewModel()
        {
            IsVisibleEditor = false;
        }

        public void SendHobbieById(string id, int userId, bool guest)
        {
            Guest = guest;
            UserId = userId;
            Anime = new()
            {
                Id = id
            };
            _ = GetMovieDetails();

            ListReviews = new HobbieRepository().GetReviewsHobbie(Anime.Id);
            ListReviews.Where(x => x.IdUser == UserId)
              .ToList()
              .ForEach(x => x.Name = "Tu");

            CheckAdded();
        }

        private void CheckAdded()
        {
            IsAddedFavorite = new HobbieRepository().Exists("Favorite", UserId, "Anime", Anime.Id);
            IsAddedSeen = new HobbieRepository().Exists("Seen", UserId, "Anime", Anime.Id);
            IsAddedPending = new HobbieRepository().Exists("Pending", UserId, "Anime", Anime.Id);

            ListReviews = new HobbieRepository().GetReviewsHobbie(Anime.Id);
            ListReviews.Where(x => x.IdUser == UserId)
              .ToList()
              .ForEach(x => x.Name = "Tu");
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
            string review = "", stars = "";

            if (type.Equals("Seen"))
            {
                stars = await App.Current.MainPage.DisplayPromptAsync("Puntua el hobbie", "Deja tu puntuación", placeholder: "Ej 8.25", maxLength: 4, keyboard: Keyboard.Numeric);

                if (stars is not null)
                {
                    review = await App.Current.MainPage.DisplayPromptAsync("Puntua el hobbie", "Deja tu reseña", maxLength: 20, keyboard: Keyboard.Default);
                }
            }

            if (new HobbieRepository().AddHobbie(type, UserId, "Anime", new HobbieModel() { Id = Anime.Id, Imagen = Anime.Imagen, Title = Anime.Title }, stars, review))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Hobbie cambiado satisfactoriamente", "Aceptar");
            }

            CheckAdded();
        }

        [RelayCommand]
        public async Task RemoveHobbie(string type)
        {
            if (new HobbieRepository().RemoveHobbie(type, UserId, Anime.GetType().ToString(), Anime.Id))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Hobbie cambiado satisfactoriamente", "Aceptar");
            }

            if (type.Equals("Seen"))
            {
                ListReviews = new HobbieRepository().GetReviewsHobbie(Anime.Id);
                ListReviews.Where(x => x.IdUser == UserId)
                  .ToList()
                  .ForEach(x => x.Name = "Tu");
            }

            CheckAdded();
        }
    }
}
