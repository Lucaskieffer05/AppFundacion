using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;

namespace AppFundacion.ViewModels
{
    public partial class ConfiguracionViewModel : ObservableObject
    {

        [ObservableProperty]
        private bool isPasswordFieldVisible = true;

        [ObservableProperty]
        private bool isConfigFieldVisible = false;

        [ObservableProperty]
        private string? password = null;

        private string? passwordAcces;

        [ObservableProperty]
        private string? stringConnection;

        [ObservableProperty]
        private string? pathSello = null;

        public ConfiguracionViewModel()
        {
            StringConnection = Preferences.Get("stringConnection", defaultValue: null);
            PathSello = Preferences.Get("pathSello", defaultValue: null);
            if (StringConnection == null)
            {
                Preferences.Set("stringConnection", "Server=localhost;Initial Catalog=Fundacion;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
            }
            passwordAcces = Preferences.Get("password", defaultValue: null);

        }

        [RelayCommand]
        private async Task Acceder()
        {
            // Lógica para el comando Acceder

            if (Password == null || Password == "")
            {
                await Shell.Current.DisplayAlert("Error", "Introduzca una contraseñs valida", "OK");
                return;
            }

            if (passwordAcces == null)
            {
                Preferences.Set("password", Password);
                IsPasswordFieldVisible = false;
                IsConfigFieldVisible = true;
                return;
            }

            if (Password != passwordAcces)
            {
                await Shell.Current.DisplayAlert("Error", "Contraseña incorrecta", "OK");
                return;
            }
            else
            {
                IsPasswordFieldVisible = false;
                IsConfigFieldVisible = true;
            }
        }

        [RelayCommand]
        private async Task Guardar()
        {
            // Lógica para el comando Guardar
            if (StringConnection == null || StringConnection == "")
            {
                await Shell.Current.DisplayAlert("Error", "Introduzca una cadena de conexión valida", "OK");
                return;
            }
            Preferences.Set("stringConnection", StringConnection);

            if (PathSello == null || PathSello == "")
            {
                await Shell.Current.DisplayAlert("Error", "Introduzca una cadena de conexión valida", "OK");
                return;
            }
            Preferences.Set("pathSello", PathSello);

            await Shell.Current.DisplayAlert("Exito", "Configuración guardada, reinicie la aplicacion", "OK");
        }


    }
}
