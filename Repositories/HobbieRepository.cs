using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// <param name="hobbieId">Valor del id del hobbie</param>
        /// <returns></returns>
        public bool AddHobbie(string addType, int idUser, string hobbieType, string hobbieId)
        {
            var letra = addType.ToArray()[0];

            SetCmdQuery(
                $"INSERT into {addType}"
                    + $"Hobbies(IdUser{letra}, HobbieType, [Value]) VALUES(@IdUser, @HobbieType, @IdHobbie)"
            );

            AddCmdParameters(
                new()
                {
                    { "@IdUser", idUser },
                    { "@HobbieType", hobbieType },
                    { "@IdHobbie", hobbieId }
                }
            );

            Oconexion.Open();

            if (Cmd.ExecuteNonQuery() == 0)
            {
                return false;
            }

            return true;
        }

        public bool RemoveHobbie(string addType, string id)
        {
            SetCmdQuery($"DELETE FROM {addType}" + $"Hobbies WHERE[Value] = @Value");

            AddCmdParameters(new() { { "@Value", id } });

            Oconexion.Open();

            if (Cmd.ExecuteNonQuery() == 0)
            {
                return false;
            }

            return true;
        }
    }
}
