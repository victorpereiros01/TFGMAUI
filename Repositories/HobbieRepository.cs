using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFGMaui.Models;
using TFGMaui.Services;

namespace TFGMaui.Repositories
{
    internal class HobbieRepository
    {
        /// <summary>
        /// Clase para añadir hobbies y borrarlos de la base de datos
        /// </summary>
        private readonly SqlConnection Oconexion;

        public SqlCommand Cmd { get; set; }

        /// <summary>
        /// Añade la query
        /// </summary>
        /// <param name="query"></param>
        public void SetCmdQuery(string query)
        {
            Cmd.CommandText = query;
        }

        /// <summary>
        /// Añade los parametros al comando
        /// </summary>
        /// <param name="p"></param>
        private void AddCmdParameters(Dictionary<string, object> p)
        {
            foreach (var item in p)
            {
                if (p.TryGetValue(item.Key, out var value))
                    Cmd.Parameters.AddWithValue(item.Key, value);
            }
        }

        public HobbieRepository()
        {
            Oconexion = new(IConstantes.ConnectionString);
            Cmd = new SqlCommand { Connection = Oconexion };
        }

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

        public QuoteModel GetQuoteRandom()
        {
            SetCmdQuery("SELECT TOP 1 [Value], Source FROM[dbo].[Quotes] ORDER by newid()");

            Oconexion.Open();

            using SqlDataReader dr = Cmd.ExecuteReader();

            if (!dr.Read())
            {
                return null;
            }

            return new QuoteModel()
            {
                Value = dr.GetString(0),
                Source = dr.GetString(1)
            };
        }
    }
}
