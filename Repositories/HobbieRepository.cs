using Microsoft.Data.SqlClient;
using TFGMaui.Models;
using TFGMaui.Services;

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
        public bool AddHobbie(string addType, int idUser, string hobbieType, string value)
        {
            var letra = addType.ToArray()[0];

            SetCmdQuery($"SELECT 1 FROM {addType}Hobbies WHERE HobbieType = @HobbieType AND Value = @Value AND IdUser{letra} = @IdUser");

            AddCmdParameters(
                new()
                {
                    { "@HobbieType", hobbieType },
                    { "@Value", value },
                    { "@IdUser", idUser }
                }
            );

            Oconexion.Open();

            if (Cmd.ExecuteScalar() != null)
            {
                return false;
            }

            // Segunda query
            SetCmdQuery($"INSERT into {addType}Hobbies(HobbieType, Value, IdUser{letra}) VALUES(@HobbieType, @Value, @IdUser)");

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
            var letra = addType.ToArray()[0];

            SetCmdQuery($"DELETE FROM {addType}Hobbies WHERE HobbieType = @HobbieType AND VALUE = @Value AND IdUser{letra} = @IdUser ");

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
    }
}
