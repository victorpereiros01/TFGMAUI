namespace TFGMaui.Services
{
    interface IConstantes
    {
        // Conexion para SQL Server
        public const string ConnectionString = "Data Source=DESKTOP-5UTJUQ6;Initial Catalog=APIHobbies;Integrated Security=True;TrustServerCertificate=True";

        // Url para libros
        public const string BaseBooks = "https://www.googleapis.com/books/v1";
        public const string ApiKeyBooks = "AIzaSyDrKFnqjlrj1ATuYXsclhGxaCRpyuY5OH0";

        // Url para anime y manga
        public const string BaseAnimeManga = "https://api.jikan.moe/v4";

        // Constantes para la api de MovieDB
        public const string BaseMovieDb = "https://api.themoviedb.org/3";
        public const string MovieDB_Bearer = "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI0MGFmYzNlODc1NTgzMDM2YTlhOTNjMTVjMzRhYWU2ZCIsInN1YiI6IjY1ZjgxYTE2MjQyZjk0MDE2NGNjZTM2ZCIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.uEBiF1avCHP0UeZaSQuvltxWLSN93rEYf9E2mO2mJz8";
        public const string MovieDB_ApiKey = "40afc3e875583036a9a93c15c34aae6d";

        // Constantes para los videojuegos
        public const string BaseGames = "https://api.igdb.com/v4";

        // Con estas tres hay que hacer una peticion POST a https://id.twitch.tv/oauth2/token y pasarle los tres parametros para obtener el bearer token
        public const string client_id = "uq8mxxgqhe22im2goshgjzcqar9kin";
        public const string client_secret = "dpayt5skeasu7xt9uuitz0nempqpsb";
        public const string grant_type = "client_credentials";

        // Para hacer una request hacer POST a BaseGames y pasarle como header client_id como Client-ID y el token como Authorization Bearer loquesea
    }
}
