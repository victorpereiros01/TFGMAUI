using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
y using TFGMaui.Models;
using TFGMaui.Repositories;
using Windows.UI.ViewManagement;
using TFGMaui.Utils;

namespace TFGMaui.ViewModels
{
    internal partial class RegisterViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool firstPageReg;

        [ObservableProperty]
        private UsuarioModel usuarioReg;

        [ObservableProperty]
        private string repContra;

        [ObservableProperty]
        private Color colorAcc, txColor;

        [ObservableProperty]
        private ObservableCollection<HobbieModel> items;

        public RegisterViewModel()
        {
            ColorAcc = Color.Parse(new UISettings().GetColorValue(UIColorType.Accent).ToString());
            TxColor = MiscellaneousUtils.ColorIsDarkOrLight(colorAcc);

            UsuarioReg = new();
            FirstPageReg = true;
            Items =
            [
                new HobbieModel() { IsChecked = false, NombreHobbie = "Cinema" },
                new HobbieModel() { IsChecked = false, NombreHobbie = "Manganime" },
                new HobbieModel() { IsChecked = false, NombreHobbie = "Games" },
                new HobbieModel() { IsChecked = false, NombreHobbie = "Books & comics" }
            ];
            UsuarioReg.Hobbies = [];
        }

        [RelayCommand]
        public async Task Navegar(string pagina)
        {
            await Shell.Current.GoToAsync("//" + pagina, new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioReg
            });
        }

        /// <summary>
        /// Cambia entre las dos paginas del registro
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task CambiarPagina()
        {
            // Esta en la primera pagina
            if (FirstPageReg)
            {
                // Si no existe el usuario esta valido para crearlo
                if (new AuthRepository().UserDoesntExists(UsuarioReg))
                {
                    if (RepContra.Equals(UsuarioReg.Password))
                    {
                        // Cambia a la segunda pagina
                        FirstPageReg = false;
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Las contraseñas no coinciden", "Aceptar");
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "El nombre de usuario está en uso", "Aceptar");
                }
            }
            else
            {
                // Esta en la segunda pagina y cambia a la primera
                FirstPageReg = true;
            }
        }

        /// <summary>
        /// Sube a la base de datos los datos del usuario, como los hobbies y la imagen
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task Registrar()
        {
            Items.ToList().ForEach(x => UsuarioReg.Hobbies.Add(x.IsChecked));

            if (new AuthRepository().Registrar(UsuarioReg))
            {
                new AuthRepository().SetImageDefault(usuarioReg.Username);

                await App.Current.MainPage.DisplayAlert("Usuario creado", "Se ha registrado el usuario correctamente", "Aceptar");
                await Navegar("LoginPage");
            }
        }
    }
}
