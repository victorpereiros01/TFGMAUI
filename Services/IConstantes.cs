using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFGMaui.Services
{
    interface IConstantes
    {
        public static string ConnectionString => "Data Source=DESKTOP-5UTJUQ6;Initial Catalog=APIHobbies;Integrated Security=True";
        public static string Bearer => "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI0MGFmYzNlODc1NTgzMDM2YTlhOTNjMTVjMzRhYWU2ZCIsInN1YiI6IjY1ZjgxYTE2MjQyZjk0MDE2NGNjZTM2ZCIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.uEBiF1avCHP0UeZaSQuvltxWLSN93rEYf9E2mO2mJz8";
        public static string ApiKey => "40afc3e875583036a9a93c15c34aae6d";
    }
}
