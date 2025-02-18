using CommunityToolkit.Mvvm.ComponentModel;
using AppFundacion.Models;
using AppFundacion.Controllers;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace AppFundacion.ViewModels
{
    public partial class CobradorAgregarViewModel : ObservableObject
    {
        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        readonly CobradorController _cobradorController;
        readonly ZonaController _zonaController;

        [ObservableProperty]
        private Cobrador cobradorAgregar = new();

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private ObservableCollection<Zona> listaZonas = [];

        [ObservableProperty]
        private Zona zonaSeleccionada = new();

        [ObservableProperty]
        private ObservableCollection<Cobrador> listaCobradores = [];

        [ObservableProperty]
        private Cobrador cobradorSeleccionado = new();


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------
        public CobradorAgregarViewModel()
        {
            _cobradorController = new CobradorController(new FundacionContext());
            _zonaController = new ZonaController(new FundacionContext());
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public async Task CargarZonas()
        {
            var zonaList = await _zonaController.GetAllZonas();
            ListaZonas = new ObservableCollection<Zona>(zonaList);
        }

        public async Task CargarCobradores()
        {
            var cobradorList = await _cobradorController.GetAllCobradores();
            ListaCobradores = new ObservableCollection<Cobrador>(cobradorList);
        }


        [RelayCommand]
        public async Task AgregarCobrador()
        {
            try
            {
                if (ZonaSeleccionada == null)
                {
                    await Shell.Current.DisplayAlert("Error!", "No se ha seleccionado ninguna zona.", "OK");
                    return;
                }
                if (_cobradorController.VerificarCodigoDuplicado(CobradorAgregar.Codigo))
                {
                    await Shell.Current.DisplayAlert("Error!", "El código del cobrador ya existe.", "OK");
                    return;
                }

                CobradorAgregar.IdZona = ZonaSeleccionada.Id;
                CobradorAgregar.IdZonaNavigation = null;

                var resultado = await _cobradorController.AddCobrador(CobradorAgregar);
                string mensaje = resultado ? "El donante fue agregado con éxito" : "Ocurrió un error al agregar el donante";

                await Shell.Current.DisplayAlert(resultado ? "Tarea Exitosa" : "Error", mensaje, "OK");
                await CargarListasAsync();
                CobradorAgregar = new();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error!", "Ocurrio un error", "OK");
                Debug.WriteLine(ex.Message);
                return;
            }

        }

        [RelayCommand]
        public async Task EliminarCobradorAsync()
        {
            if (CobradorSeleccionado == null)
            {
                await Shell.Current.DisplayAlert("Error!", "No se ha seleccionado ningun cobrador.", "OK");
                return;
            }
            var confirmacion = await Shell.Current.DisplayAlert("Eliminar Donante", "¿Está seguro que desea eliminar el cobrador?", "Si", "No");

            if (confirmacion)
            {
                if (!_cobradorController.VerificarEliminarCobrador(CobradorSeleccionado.Id))
                {
                    await Shell.Current.DisplayAlert("Error!", "No se puede eliminar el cobrador porque tiene donantes asignados.", "OK");
                    return;
                }

                var resultado = await _cobradorController.DeleteCobrador(CobradorSeleccionado.Id);

                if (resultado)
                {
                    await Shell.Current.DisplayAlert("Cobrador Eliminado", "El cobrador ha sido eliminado correctamente.", "OK");
                    await CargarListasAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error!", "No se ha podido eliminar el cobrador.", "OK");
                }
            }
        }

        public async Task CargarListasAsync()
        {
            IsBusy = true;
            await CargarZonas();
            await CargarCobradores();
            IsBusy = false;
        }


    }
}
