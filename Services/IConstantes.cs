namespace TFGMaui.Services
{
    interface IConstantes
    {
        public const string BaseMarvel = "https://gateway.marvel.com:443/v1/public";
        public const string BaseBooks = "https://www.googleapis.com/books/v1";
        public const string BaseAnimeManga = "https://api.jikan.moe/v4";

        public const string ConnectionString = "Data Source=DESKTOP-5UTJUQ6;Initial Catalog=APIHobbies;Integrated Security=True;TrustServerCertificate=True";

        public const string MovieDB_Bearer = "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI0MGFmYzNlODc1NTgzMDM2YTlhOTNjMTVjMzRhYWU2ZCIsInN1YiI6IjY1ZjgxYTE2MjQyZjk0MDE2NGNjZTM2ZCIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.uEBiF1avCHP0UeZaSQuvltxWLSN93rEYf9E2mO2mJz8";
        public const string BaseMovieDb = "https://api.themoviedb.org/3";
        public const string MovieDB_ApiKey = "40afc3e875583036a9a93c15c34aae6d";

        public static string Ts { get; set; }
        public const string PrivateMarvelKey = "3a49aacc1ca60cd874e846c9efe9572a7d7254be";
        public const string PublicMarvelKey = "7a0f91017a8b64c6abced551029b78ee";
        public static string Hash { get; set; }
        public static string MarvelPage => $"{BaseMarvel}?ts={Ts}&apikey={PublicMarvelKey}&hash={Hash}";  // Digest md5 ts+privateKey+publicKey
    }
}
