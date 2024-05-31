using System.Data;
using Microsoft.Data.SqlClient;
using TFGMaui.Models;

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

            SetCmdQuery("SELECT IdAdded, HobbieType, Value, IdUserF, Imagen, Title FROM FavoriteHobbies WHERE IdUserF=@IdUser");

            AddCmdParameters(new() { { "@IdUser", idUser } });

            Cmd.CommandType = CommandType.Text;
            Oconexion.Open();
            using SqlDataReader dr = Cmd.ExecuteReader();

            if (!dr.Read())
            {
                return null;
            }

            while (dr.Read())
            {
                listHobbies.Add(new SavedHobbieModel()
                {
                    IdAdded = dr.GetInt32(0),
                    HobbieType = dr.GetString(1),
                    Value = dr.GetString(2),
                    IdUser = dr.GetInt32(3),
                    Imagen = dr.GetString(4),
                    Title = dr.GetString(5)
                });
            }

            return listHobbies;
        }

        public List<SavedHobbieModel> GetPending(int idUser)
        {
            List<SavedHobbieModel> listHobbies = [];

            SetCmdQuery("SELECT IdAdded, HobbieType, Value, IdUserP, Imagen, Title FROM PendingHobbies WHERE IdUserP=@IdUser");

            AddCmdParameters(new() { { "@IdUser", idUser } });

            Cmd.CommandType = CommandType.Text;
            Oconexion.Open();
            using SqlDataReader dr = Cmd.ExecuteReader();

            if (!dr.Read())
            {
                return null;
            }

            while (dr.Read())
            {
                listHobbies.Add(new SavedHobbieModel()
                {
                    IdAdded = dr.GetInt32(0),
                    HobbieType = dr.GetString(1),
                    Value = dr.GetString(2),
                    IdUser = dr.GetInt32(3),
                    Imagen = dr.GetString(4),
                    Title = dr.GetString(5)
                });
            }

            return listHobbies;
        }

        public List<SavedHobbieModel> GetSeen(int idUser)
        {
            List<SavedHobbieModel> listHobbies = [];

            SetCmdQuery("SELECT IdAdded, HobbieType, Value, IdUserS, Imagen, Title FROM SeenHobbies WHERE IdUserS=@IdUser");

            AddCmdParameters(new() { { "@IdUser", idUser } });

            Cmd.CommandType = CommandType.Text;
            Oconexion.Open();
            using SqlDataReader dr = Cmd.ExecuteReader();

            if (!dr.Read())
            {
                return null;
            }

            while (dr.Read())
            {
                listHobbies.Add(new SavedHobbieModel()
                {
                    IdAdded = dr.GetInt32(0),
                    HobbieType = dr.GetString(1),
                    Value = dr.GetString(2),
                    IdUser = dr.GetInt32(3),
                    Imagen = dr.GetString(4),
                    Title = dr.GetString(5)
                });
            }

            return listHobbies;
        }
    }
}
