﻿using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using TFGMaui.Utils;
using TFGMaui.ViewModels;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace TFGMaui.Repositories
{
    /// <summary>
    /// Clase que contiene los metodos para hacer login y registro
    /// </summary>
    internal class AuthRepository : Repository
    {
        /// <summary>
        /// Metodo que evalua que el usuario exista o no
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UserExists(UsuarioModel user)
        {
            if (user.Email.IsNullOrEmpty() || user.Email.Equals(" "))
            {
                SetCmdQuery("SELECT 1 FROM Users WHERE Username = @Username");

                AddCmdParameters(new() { { "@Username", user.Username } });

                Oconexion.Open();

                // Si devuelve null el usuario existe
                return Cmd.ExecuteScalar() != null;
            }
            else
            {
                SetCmdQuery("SELECT 1 FROM Users WHERE Username = @Username or Email = @Email");

                AddCmdParameters(new() { { "@Username", user.Username }, { "@Email", user.Email is null ? "" : user.Email } });

                Oconexion.Open();

                // Si devuelve null el usuario existe
                return Cmd.ExecuteScalar() != null;
            }
        }

        public bool SetImageDefault(string username)
        {
            SetCmdQuery("SELECT Avatar FROM Users WHERE Username = 'admin'");

            // Abre la conexion e inicializa el reader para obtener los datos
            Oconexion.Open();
            using (SqlDataReader dr = Cmd.ExecuteReader())
            {
                if (!dr.Read())
                {
                    return false;
                }

                SetCmdQuery("UPDATE [dbo].[Users] SET [Avatar] = @Avatar WHERE Username = @Username");

                AddCmdParameters(new() { { "@Username", username }, { "@Avatar", dr.GetString(0) } });
            }

            // A la conexion y ejecuta la query devolviendo las columnas afectadas
            Oconexion.Close();
            Oconexion.Open();

            // Si no hay columnas afectadas da error
            if (Cmd.ExecuteNonQuery() == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Devuelve el usuario de la base de datos
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UsuarioModel? Login(string username, string password)
        {
            try
            {
                SetCmdQuery("SELECT IdUser, Username, Name, Email, Avatar, Password, Hobbie1, Hobbie2, Hobbie3, Hobbie4, Adult, Language FROM Users WHERE Username = @Username or Email = @Username AND Password = @Pass");

                AddCmdParameters(new() { { "@Username", username }, { "@Pass", password } });

                // Abre la conexion e inicializa el reader para obtener los datos
                Oconexion.Open();
                using SqlDataReader dr = Cmd.ExecuteReader();

                if (!dr.Read())
                {
                    return null;
                }

                var user = new UsuarioModel
                {
                    Id = dr.GetInt32(0),
                    Username = dr.GetString(1).Trim(),
                    Name = dr.GetString(2).Trim(),
                    Email = dr.GetString(3).Trim(),
                    Avatar = FileUtils.GetSource(dr.GetString(4)),
                    Password = dr.GetString(5).Trim(),
                    Hobbies = [dr.GetBoolean(6), dr.GetBoolean(7), dr.GetBoolean(8), dr.GetBoolean(9)],
                    Adulto = dr.GetBoolean(10),
                    Language = dr.GetString(11).Trim(),
                    Guest = dr.GetString(1).Equals("admin")
                };

                return user;
            }
            catch { throw new Exception(); }
        }

        public ImageSource GetImage(string username)
        {
            SetCmdQuery("SELECT Avatar FROM Users WHERE Username = @Username");

            AddCmdParameters(new() { { "@Username", username } });

            // Abre la conexion e inicializa el reader para obtener los datos
            Oconexion.Open();
            using SqlDataReader dr = Cmd.ExecuteReader();

            if (!dr.Read())
            {
                return null;
            }

            return FileUtils.GetSource(dr.GetString(0));
        }

        public bool Registrar(UsuarioModel user)
        {
            SetCmdQuery("INSERT INTO Users ([Username], [Name], [Email], [Password], [Hobbie1], [Hobbie2], [Hobbie3], [Hobbie4], [Adult], [Language], [Guest]) \nVALUES (@Username, @Name, @Email, @Password, @Hobbie1, @Hobbie2, @Hobbie3, @Hobbie4, @Adulto, 'en-US', @Guest)");

            AddCmdParameters(
                new()
                {
                    { "@Username", user.Username },
                    { "@Name", user.Name },
                    { "@Password", user.Password },
                    { "@Hobbie1", user.Hobbies[0] },
                    { "@Hobbie2", user.Hobbies[1] },
                    { "@Hobbie3", user.Hobbies[2] },
                    { "@Hobbie4", user.Hobbies[3] },
                    { "@Adulto", true },
                    { "@Email", user.Email is null? " ": user.Email },
                    { "@Guest", false }
                }
            );

            Oconexion.Open();

            // Check Error
            if (Cmd.ExecuteNonQuery() == 0)
            {
                return false;
            }

            return true;
        }

        public static string HashPasswordSHA512(string password)
        {
            byte[] data = SHA3_512.HashData(Encoding.UTF8.GetBytes(password));

            StringBuilder sBuilder = new();

            foreach (byte v in data)
            {
                sBuilder.Append(v.ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            string hashOfInput = HashPasswordSHA512(inputPassword);

            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, storedHash) == 0;
        }
    }
}
