using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using TFGMaui.Models;
using TFGMaui.Repositories;
using TFGMaui.Services;
namespace TFGMaui.ViewModels.Mopup
{
    public partial class GameMopupViewModel : ObservableObject
    {
        [ObservableProperty]
        private GameModel game;

        private int UserId;

        [ObservableProperty]
        private bool isVisibleEditor;

        public string Bearer { get; set; }

        [ObservableProperty]
        private bool isAddedFavorite, isAddedPending, isAddedSeen;

        [ObservableProperty]
        private List<ReviewModel> listReviews;

        [ObservableProperty]
        private bool guest;

        public GameMopupViewModel()
        {
            IsVisibleEditor = false;
        }

        public void SendHobbieById(string id, int userId, string bearer, bool guest)
        {
            Guest = guest;
            Bearer = bearer;
            UserId = userId;
            Game = new()
            {
                Id = id
            };
            _ = GetGameDetails();

            ListReviews = new HobbieRepository().GetReviewsHobbie(Game.Id);
            ListReviews.Where(x => x.IdUser == UserId)
              .ToList()
              .ForEach(x => x.Name = "Tu");

            CheckAdded();
        }

        private void CheckAdded()
        {
            IsAddedFavorite = new HobbieRepository().Exists("Favorite", UserId, "Game", Game.Id);
            IsAddedSeen = new HobbieRepository().Exists("Seen", UserId, "Game", Game.Id);
            IsAddedPending = new HobbieRepository().Exists("Pending", UserId, "Game", Game.Id);

            ListReviews = new HobbieRepository().GetReviewsHobbie(Game.Id);
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
        public async Task GetGameDetails()
        {
            var requestPagina = new HttpRequestModel(url: IConstantes.BaseGames,
            endpoint: $"games",
            parameters: null,
                headers: new Dictionary<string, string> { { "Client-ID", IConstantes.client_id }, { "Authorization", $"Bearer {Bearer}" }, { "Accept", "application/json" } },
                body: $"""
                fields *;
                limit 1;
                where id = {Game.Id};
                """);

            try
            {
                var listTrend = (List<GameModel>)await HttpService.ExecuteRequestAsync<List<GameModel>>(requestPagina);

                var g = listTrend[0];
                g.Imagen = await GetImage(g.Cover);
                switch (g.Status)
                {
                    case 0:
                        g.StatusString = "Released";
                        break;
                    case 1:
                        g.StatusString = "Alpha";
                        break;
                    case 2:
                        g.StatusString = "Beta";
                        break;
                    case 3:
                        g.StatusString = "Early Access";
                        break;
                    case 4:
                        g.StatusString = "Offline";
                        break;
                    case 5:
                        g.StatusString = "Cancelled";
                        break;
                    case 6:
                        g.StatusString = "Rumored";
                        break;
                    case 7:
                        g.StatusString = "Delisted";
                        break;
                }

                g.Rating = Convert.ToDouble(string.Format("{0:N2}", g.Rating / 10));
                Game = g;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Saludos", e.ToString(), "Aceptar");
            }
        }

        private async Task<string> GetImage(int cover)
        {
            var requestPagina = new HttpRequestModel(url: "https://api.igdb.com/v4/covers",
                endpoint: "",
                parameters: null,
                headers: new Dictionary<string, string> { { "Client-ID", IConstantes.client_id }, { "Authorization", $"Bearer {Bearer}" }, { "Accept", "application/json" } },
                body: $"""
                fields *;
                where id={cover};
                """);

            try
            {
                var images = (List<CoverModel>)await HttpService.ExecuteRequestAsync<List<CoverModel>>(requestPagina);

                var split = images.First().Url.Split("/");
                split[6] = "t_cover_big_2x";
                var duplicado = "https:/";

                for (int i = 0; i < split.Length; i++)
                {
                    if (i > 1)
                    {
                        duplicado += "/" + split[i];
                    }
                }

                return duplicado;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Saludos", e.ToString(), "Aceptar");
            }

            return null;
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

            if (new HobbieRepository().AddHobbie(type, UserId, "Game", new HobbieModel() { Id = Game.Id, Imagen = Game.Imagen, Title = Game.Title }, stars, review))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Hobbie cambiado satisfactoriamente", "Aceptar");
            }

            CheckAdded();
        }

        [RelayCommand]
        public async Task RemoveHobbie(string type)
        {
            if (new HobbieRepository().RemoveHobbie(type, UserId, Game.GetType().ToString(), Game.Id))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Hobbie cambiado satisfactoriamente", "Aceptar");
            }

            CheckAdded();
        }
    }
}
