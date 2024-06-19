using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Globalization;
using TFGMaui.Models;
using TFGMaui.Utils;

namespace TFGMaui.Repositories
{
    /// <summary>
    /// Clase para añadir hobbies y borrarlos de la base de datos
    /// </summary>
    internal class HobbieRepository : Repository
    {
        public List<ReviewModel> GetReviewsHobbie(string value)
        {
            List<ReviewModel> listReviews = [];

            AddCmdParameters(new() { { "@Value", value } });

            SetCmdQuery("SELECT IdUserS, InternalScore, Review, u.Avatar, Value FROM SeenHobbies INNER JOIN Users u ON u.IdUser=SeenHobbies.IdUserS WHERE Value=@Value and InternalScore is NOT NULL AND Review IS NOT NULL");

            Oconexion.Open();
            using SqlDataReader dr = Cmd.ExecuteReader();

            while (dr.Read())
            {
                listReviews.Add(new ReviewModel()
                {
                    IdUser = dr.GetInt32(0),
                    Score = dr.GetDecimal(1),
                    Review = dr.GetString(2),
                    ImageAvatar = FileUtils.GetSource(dr.GetString(3))
                });
            }

            return listReviews;
        }

        public bool Exists(string addType, int idUser, string type, string value)
        {
            // Si ya esta en favoritos no lo añade otra vez
            SetCmdQuery($"SELECT 1 FROM {addType}Hobbies WHERE HobbieType = @HobbieType AND Value = @Value AND IdUser{addType.ToArray()[0]} = @IdUser");
            AddCmdParameters(
            new()
            {
                { "@HobbieType", type },
                { "@Value", value },
                { "@IdUser", idUser },
            });

            Oconexion.Open();

            return Cmd.ExecuteScalar() == null;
        }

        /// <summary>
        /// Inserta en la base de datos en el usuario correspondiente el hobbie
        /// </summary>
        /// <param name="addType">Ruta a la tabla a la que lo añadirá</param>
        /// <param name="idUser">Id del user que tiene ese hobbie</param>
        /// <param name="hobbieType">Tipo de hobbie</param>
        /// <param name="value">Valor del id del hobbie</param>
        /// <returns></returns>
        public bool AddHobbie(string addType, int idUser, string type, HobbieModel h, string st, string rev)
        {
            if (AreAnyNull(addType, idUser, type, h) || AreAnyNull(h.Id, h.Imagen, h.Title))
            {
                return false;
            }

            if (addType.Equals("Favorite"))
            {
                if (!Exists(addType, idUser, type, h.Id))
                {
                    return false;
                }

                AddCmdParameters(new()
                {
                    { "@Imagen", h.Imagen },
                    { "@Title", h.Title }
                });

                // Lo añade a favoritos
                SetCmdQuery($"INSERT into {addType}Hobbies(HobbieType, Value, IdUser{addType.ToArray()[0]}, Imagen, Title) VALUES(@HobbieType, @Value, @IdUser, @Imagen, @Title)");

                Oconexion.Close();
                Oconexion.Open();
            }
            else
            {
                if (!Exists(addType, idUser, type, h.Id))
                {
                    return false;
                }

                AddCmdParameters(new()
                {
                    { "@Imagen", h.Imagen },
                    { "@Title", h.Title }
                });

                // Lo borra del hobbie contrario
                var contrario = addType.Equals("Seen") ? "Pending" : "Seen";
                SetCmdQuery($"DELETE FROM {contrario}Hobbies WHERE HobbieType = @HobbieType AND Value = @Value AND IdUser{contrario.ToArray()[0]} = @IdUser");

                Oconexion.Close();
                Oconexion.Open();

                Cmd.ExecuteNonQuery();

                // Lo añade
                if (addType.Equals("Seen") && (!st.IsNullOrEmpty() || !rev.IsNullOrEmpty()))
                {
                    AddCmdParameters(new()
                    {
                        { "@InternalScore", Convert.ToDecimal(st, CultureInfo.InvariantCulture) },
                        { "@Review", rev}
                    });

                    SetCmdQuery($"INSERT into {addType}Hobbies(HobbieType, Value, IdUser{addType.ToArray()[0]}, Imagen, Title, InternalScore, Review) VALUES(@HobbieType, @Value, @IdUser, @Imagen, @Title, @InternalScore, @Review)");
                }
                else
                {
                    SetCmdQuery($"INSERT into {addType}Hobbies(HobbieType, Value, IdUser{addType.ToArray()[0]}, Imagen, Title) VALUES(@HobbieType, @Value, @IdUser, @Imagen, @Title)");
                }

                Oconexion.Close();
                Oconexion.Open();
            }

            if (Cmd.ExecuteNonQuery() == 0)
            {
                return false;
            }

            return true;

        }

        public bool RemoveHobbie(string addType, int idUser, string hobbieType, string value)
        {
            if (AreAnyNull(addType, idUser, hobbieType, value))
            {
                return false;
            }

            SetCmdQuery($"DELETE FROM {addType}Hobbies WHERE HobbieType = @HobbieType AND VALUE = @Value AND IdUser{addType.ToArray()[0]} = @IdUser ");

            var ht = hobbieType.Split(".")[2].Replace("Model", "");

            AddCmdParameters(
                new()
                {
                    { "@IdUser", idUser },
                    { "@HobbieType", ht},
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

        private bool AreAnyNull(params object[] parameters)
        {
            return parameters.Any(p => p is null);
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
                    Color = MiscellaneousUtils.GetColorHobbie(dr.GetString(1)),
                    Value = dr.GetString(2),
                    Imagen = dr.GetString(3),
                    Title = dr.GetString(4)
                });
            }

            return listHobbies;
        }

        public List<SavedHobbieModel> GetFavorites(int idUser) => GetHobbies(idUser, "Favorite");

        public List<SavedHobbieModel> GetPending(int idUser) => GetHobbies(idUser, "Pending");

        public List<SavedHobbieModel> GetSeen(int idUser) => GetHobbies(idUser, "Seen");
    }
}
