using Microsoft.Data.SqlClient;
using System.Data;
using TFGMaui.Models;
using TFGMaui.Services;
using TFGMaui.Utils;

namespace TFGMaui.Repositories
{
    /// <summary>
    /// Clase para añadir hobbies y borrarlos de la base de datos
    /// </summary>
    internal class HobbieRepository : Repository
    {
        /// <summary>
        /// Inserta en la base de datos en el usuario correspondiente el hobbie
        /// </summary>
        /// <param name="addType">Ruta a la tabla a la que lo añadirá</param>
        /// <param name="idUser">Id del user que tiene ese hobbie</param>
        /// <param name="hobbieType">Tipo de hobbie</param>
        /// <param name="value">Valor del id del hobbie</param>
        /// <returns></returns>
        public bool AddHobbie(string addType, int idUser, string type, HobbieModel h)
        {
            SetCmdQuery($"SELECT 1 FROM {addType}Hobbies WHERE HobbieType = @HobbieType AND Value = @Value AND IdUser{addType.ToArray()[0]} = @IdUser");

            AddCmdParameters(
                new()
                {
                    { "@HobbieType", type },
                    { "@Value", h.Id },
                    { "@IdUser", idUser },
                    { "@Imagen", h.Imagen },
                    { "@Title", h.Title }
                }
            );

            Oconexion.Open();

            if (Cmd.ExecuteScalar() != null)
            {
                return false;
            }

            // Segunda query
            SetCmdQuery($"INSERT into {addType}Hobbies(HobbieType, Value, IdUser{addType.ToArray()[0]}, Imagen, Title) VALUES(@HobbieType, @Value, @IdUser, @Imagen, @Title)");

            Oconexion.Close();
            Oconexion.Open();

            if (Cmd.ExecuteNonQuery() == 0)
            {
                return false;
            }

            return true;
        }

        public bool RemoveHobbie(string addType, int idUser, string hobbieType, string value)
        {
            SetCmdQuery($"DELETE FROM {addType}Hobbies WHERE HobbieType = @HobbieType AND VALUE = @Value AND IdUser{addType.ToArray()[0]} = @IdUser ");

            AddCmdParameters(
                new()
                {
                    { "@IdUser", idUser },
                    { "@HobbieType", hobbieType },
                    { "@Value", value }
                }
            );

            Oconexion.Open();

            if (Cmd.ExecuteNonQuery() == 0)
            {
                return false;
            }

            return true;
        }

        public List<SavedHobbieModel> GetHobbies(int idUser, string type)
        {
            List<SavedHobbieModel> listHobbies = [];

            SetCmdQuery($"SELECT IdAdded, HobbieType, Value, Imagen, Title FROM {type}Hobbies WHERE IdUser{type.ToArray()[0]}=@IdUser ORDER by IdAdded DESC");

            AddCmdParameters(new() { { "@IdUser", idUser } });

            Cmd.CommandType = CommandType.Text;
            Oconexion.Open();
            using SqlDataReader dr = Cmd.ExecuteReader();

            while (dr.Read())
            {
                listHobbies.Add(new SavedHobbieModel()
                {
                    HobbieType = dr.GetString(1),
                    Color = MiscellaneousUtils.GetColorHobbie(dr.GetString(1))[0],
                    Color2 = MiscellaneousUtils.GetColorHobbie(dr.GetString(1))[1],
                    Value = dr.GetString(2),
                    Imagen = dr.GetString(3),
                    Title = dr.GetString(4)
                });
            }

            return listHobbies;
        }

        public List<SavedHobbieModel> GetFavorites(int idUser)
        {
            return GetHobbies(idUser, "Favorite");
        }

        public List<SavedHobbieModel> GetPending(int idUser)
        {
            return GetHobbies(idUser, "Pending");
        }

        public List<SavedHobbieModel> GetSeen(int idUser)
        {
            return GetHobbies(idUser, "Seen");
        }
    }
}
