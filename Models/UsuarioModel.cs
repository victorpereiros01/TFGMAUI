namespace TFGMaui.ViewModels
{
    public class UsuarioModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public ImageSource Avatar { get; set; }

        public List<bool> Hobbies { get; set; }

        public bool Adulto { get; set; }

        public string Language { get; set; }

        /// <summary>
        /// Si el usuario tiene los mismos nombre o correo y contraseña
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(UsuarioModel obj) => obj.Username.Equals(Username) || obj.Email.Equals(Email) && obj.Password.Equals(Password);
    }
}