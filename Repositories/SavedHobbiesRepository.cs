using System.Data;
using Microsoft.Data.SqlClient;
using TFGMaui.Models;
using TFGMaui.Utils;

namespace TFGMaui.Repositories
{
    /// <summary>
    /// Clase que contiene los metodos para obtener los hobbies guardados
    /// </summary>
    internal class SavedHobbiesRepository : Repository
    {
        public List<SavedHobbieModel> GetFavorites(int idUser)
        {
            List<SavedHobbieModel> listHobbies = [];

            SetCmdQuery("SELECT HobbieType, Value, Imagen, Title FROM FavoriteHobbies WHERE IdUserF=@IdUser");

            AddCmdParameters(new() { { "@IdUser", idUser } });

            Cmd.CommandType = CommandType.Text;
            Oconexion.Open();
            using SqlDataReader dr = Cmd.ExecuteReader();

            while (dr.Read())
            {
                listHobbies.Add(new SavedHobbieModel()
                {
                    HobbieType = dr.GetString(0),
                    Color = MiscellaneousUtils.GetColorHobbie(dr.GetString(0)),
                    Value = dr.GetString(1),
                    Imagen = dr.GetString(2),
                    Title = dr.GetString(3)
                });
            }

            return listHobbies;
        }

        public List<SavedHobbieModel> GetPending(int idUser)
        {
            List<SavedHobbieModel> listHobbies = [];

            SetCmdQuery("SELECT HobbieType, Value, Imagen, Title FROM PendingHobbies WHERE IdUserP=@IdUser");

            AddCmdParameters(new() { { "@IdUser", idUser } });

            Cmd.CommandType = CommandType.Text;
            Oconexion.Open();
            using SqlDataReader dr = Cmd.ExecuteReader();

            while (dr.Read())
            {
                listHobbies.Add(new SavedHobbieModel()
                {
                    HobbieType = dr.GetString(0),
                    Color = MiscellaneousUtils.GetColorHobbie(dr.GetString(0)),
                    Value = dr.GetString(1),
                    Imagen = dr.GetString(2),
                    Title = dr.GetString(3)
                });
            }

            return listHobbies;
        }

        public List<SavedHobbieModel> GetSeen(int idUser)
        {
            List<SavedHobbieModel> listHobbies = [];

            SetCmdQuery("SELECT HobbieType, Value, Imagen, Title FROM SeenHobbies WHERE IdUserS=@IdUser");

            AddCmdParameters(new() { { "@IdUser", idUser } });

            Cmd.CommandType = CommandType.Text;
            Oconexion.Open();
            using SqlDataReader dr = Cmd.ExecuteReader();

            while (dr.Read())
            {
                listHobbies.Add(new SavedHobbieModel()
                {
                    HobbieType = dr.GetString(0),
                    Color = MiscellaneousUtils.GetColorHobbie(dr.GetString(0)),
                    Value = dr.GetString(1),
                    Imagen = dr.GetString(2),
                    Title = dr.GetString(3)
                });
            }

            return listHobbies;
        }
    }
}
