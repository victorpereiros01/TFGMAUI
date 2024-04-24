using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Collections.ObjectModel;
using TFGMaui.Models;
using TFGMaui.Services;

namespace TFGMaui.ViewModels
{
    public partial class MovieMopupViewModel : ObservableObject
    {
        [ObservableProperty]
        private MovieModel movie;

        public int id;

        public void SendHobbieById(string id)
        {
            this.id = Convert.ToInt32(id);
            _ = GetMovieDetails();
        }

        [RelayCommand]
        public async Task CloseInfoMopup() => await MopupService.Instance.PopAllAsync();

        public async Task GetMovieDetails()
        {
            var requestPelicula = new HttpRequestModel(url: IConstantes.BaseMovieDb,
                endpoint: $"movie/{id}",
                parameters: new Dictionary<string, string> { { "api_key", IConstantes.MovieDB_ApiKey }, { "language", "es-ES" } },
                headers: new Dictionary<string, string> { { "Accept", "application/json" }, { "Authorization", IConstantes.MovieDB_Bearer } });

            var m = (MovieModel)await HttpService.ExecuteRequestAsync<MovieModel>(requestPelicula); // v
            m.Imagen = "https://image.tmdb.org/t/p/original" + m.Imagen;
            Movie = m;
        }

        [RelayCommand]
        public async Task AddToFavorites() { }

        [RelayCommand]
        public async Task AddToPending() { }

        [RelayCommand]
        public async Task AddToSeen() { }
    }
}
