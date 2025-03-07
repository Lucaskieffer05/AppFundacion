using AppFundacion.Controllers;
using AppFundacion.Mensajes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppFundacion.Models;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace AppFundacion.ViewModels
{
    public partial class DonanteAgregarViewModel : ObservableObject
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        private readonly CobradorController _cobradorController;
        private readonly DonanteController _donanteController;

        [ObservableProperty]
        private Donante? donanteAgregar = null;

        [ObservableProperty]
        private ObservableCollection<Cobrador> listaCobradores = [];

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public DonanteAgregarViewModel()
        {
            _cobradorController = new CobradorController(new FundacionContext());
            _donanteController = new DonanteController(new FundacionContext());
            DonanteAgregar = new Donante();

            if (DonanteAgregar != null)
            {
                DonanteAgregar.FechaIngreso = DateTime.Now;
                DonanteAgregar.Pais = "Argentina";
            }
            

        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        [RelayCommand]
        async static Task VolverAtras()
        {
            try
            {
                //await Shell.Current.Navigation.PopAsync();
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                // Registra el error en un archivo de log o muestra un mensaje de error
                // Aquí se muestra un ejemplo de cómo registrar el error en un archivo de log
                string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "error_log.txt");
                logFilePath = "C:\\prueba\\error_log.txt";
                await File.AppendAllTextAsync(logFilePath, $"{DateTime.Now}: {ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}");

                // También puedes mostrar un mensaje de error al usuario
                await Shell.Current.DisplayAlert("Error", $"{ex}", "OK");
            }
        }

        [RelayCommand]
        async Task AgregarDonante()
        {
            if (DonanteAgregar == null || DonanteAgregar.IdCobradorNavigation == null || DonanteAgregar.NombreApellido == "" || DonanteAgregar.Monto < 0)
            {
                await Shell.Current.DisplayAlert("Error", "Ocurrió un error al agregar el donante. Verifica el campo de cobrador, nombre y monto", "OK");
                return;
            }

            DonanteAgregar.IdCobrador = DonanteAgregar.IdCobradorNavigation.Id;
            ReemplazarNulosConGuion(DonanteAgregar);

            // Evita que Entity Framework intente insertar un nuevo Cobrador
            DonanteAgregar.IdCobradorNavigation = null;

            var resultado = await _donanteController.AddDonante(DonanteAgregar);
            string mensaje = resultado ? "El donante fue agregado con éxito" : "Ocurrió un error al agregar el donante";

            await Shell.Current.DisplayAlert(resultado ? "Tarea Exitosa" : "Error", mensaje, "OK");

            if (resultado){
                WeakReferenceMessenger.Default.Send(new DonanteAgregadoMessage(true));
                await Shell.Current.GoToAsync("..");
            }
        }
        public async Task CargarCobradoresAsync()
        {
            var cobradoresList = await _cobradorController.GetAllCobradores();
            ListaCobradores = new ObservableCollection<Cobrador>(cobradoresList);
        }

        // -------------------------------------------------------------------
        // ----------------------- Funciones auxiliares ----------------------
        // -------------------------------------------------------------------

        void ReemplazarNulosConGuion(Donante donante)
        {
            foreach (var prop in typeof(Donante).GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    var valor = (string?)prop.GetValue(donante);
                    if (string.IsNullOrEmpty(valor))
                    {
                        prop.SetValue(donante, "-");
                    }
                }
            }
        }


    }
}
