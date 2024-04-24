namespace TFGMaui.ViewModels
{
    internal class UsuarioModel
    {
        public string NombreUsuario { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public ImageSource Avatar { get; set; }

        public List<bool> Hobbies { get; set; }

        public bool Adulto { get; set; }

        /// <summary>
        /// Si el usuario tiene los mismos nombre o correo y contraseña
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(UsuarioModel obj) => obj.NombreUsuario.Equals(NombreUsuario) || obj.Email.Equals(Email) && obj.Password.Equals(Password);
    }
}