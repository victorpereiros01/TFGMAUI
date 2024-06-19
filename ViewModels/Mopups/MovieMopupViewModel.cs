using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using TFGMaui.Models;
using TFGMaui.Repositories;
using TFGMaui.Services;

namespace TFGMaui.ViewModels.Mopup
{
    public partial class MovieMopupViewModel : ObservableObject
    {
        [ObservableProperty]
        private MovieModel movie;

        private int UserId;

        [ObservableProperty]
        private bool isVisibleEditor;

        [ObservableProperty]
        private string lang;

        [ObservableProperty]
        private bool isAddedFavorite, isAddedPending, isAddedSeen;

        [ObservableProperty]
        private List<ReviewModel> listReviews;

        [ObservableProperty]
        private bool guest;

        public MovieMopupViewModel()
        {
            IsVisibleEditor = false;
        }

        public void SendHobbieById(string id, int userId, string lang, bool guest)
        {
            Guest = guest;
            UserId = userId;
            Lang = lang;
            Movie = new()
            {
                Id = id
            };
            _ = GetMovieDetails();

            ListReviews = new HobbieRepository().GetReviewsHobbie(Movie.Id);
            ListReviews.Where(x => x.IdUser == UserId)
              .ToList()
              .ForEach(x => x.Name = "Tu");

            CheckAdded();
        }

        private void CheckAdded()
        {
            IsAddedFavorite = new HobbieRepository().Exists("Favorite", UserId, "Movie", Movie.Id);
            IsAddedSeen = new HobbieRepository().Exists("Seen", UserId, "Movie", Movie.Id);
            IsAddedPending = new HobbieRepository().Exists("Pending", UserId, "Movie", Movie.Id);

            ListReviews = new HobbieRepository().GetReviewsHobbie(Movie.Id);
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
            var requestPelicula = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: $"movie/{Movie.Id}",
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", Lang.Trim() } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            var m = (MovieModel)await HttpService.ExecuteRequestAsync<MovieModel>(requestPelicula); // v
            m.Imagen = "https://image.tmdb.org/t/p/original" + m.Imagen;
            m.Vote_Average = Convert.ToDouble(string.Format("{0:N2}", m.Vote_Average));
            Movie = m;
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

            if (new HobbieRepository().AddHobbie(type, UserId, "Movie", new HobbieModel() { Id = Movie.Id, Imagen = Movie.Imagen, Title = Movie.Title }, stars, review))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Hobbie cambiado satisfactoriamente", "Aceptar");
            }

            CheckAdded();
        }

        [RelayCommand]
        public async Task RemoveHobbie(string type)
        {
            if (new HobbieRepository().RemoveHobbie(type, UserId, Movie.GetType().ToString(), Movie.Id))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Hobbie cambiado satisfactoriamente", "Aceptar");
            }

            CheckAdded();
        }
    }
}
