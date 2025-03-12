using CommunityToolkit.Mvvm.ComponentModel;
using AppFundacion.Models;
using AppFundacion.Controllers;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using AppFundacion.Views;
using AppFundacion.Mensajes;
using CommunityToolkit.Mvvm.Messaging;

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
        private Zona? zonaSeleccionada = null;

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

            WeakReferenceMessenger.Default.Register<CobradorModificarMessage>(this, async (r, m) =>
            {
                if (m.Value)
                {
                    await CargarListasAsync();
                }
            });

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
                string mensaje = resultado ? "El donante fue agregado con éxito. Recuerda actualizar la tabla del menú 'Donantes'." : "Ocurrió un error al agregar el donante";

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
        public async Task ModificarCobrador()
        {
            // Validar que la zona seleccionada no sea null
            if (CobradorSeleccionado.IdZonaNavigation == null)
            {
                await Shell.Current.DisplayAlert("Error", "Debe seleccionar una zona", "OK");
                return;
            }
            if (CobradorSeleccionado.Nombre == null || CobradorSeleccionado?.Codigo == null || CobradorSeleccionado.Nombre == "")
            {
                await Shell.Current.DisplayAlert("Error", "Debe completar todos los campos", "OK");
                return;
            }

            var validarCodigo = _cobradorController.VerificarCodigoDuplicado(CobradorSeleccionado.Codigo, CobradorSeleccionado.Id);
            if (validarCodigo)
            {
                await Shell.Current.DisplayAlert("Error", "El código ya existe", "OK");
                CobradorSeleccionado = new();
                return;
            }



            if (CobradorSeleccionado.IdZonaNavigation != null && CobradorSeleccionado.IdZona != CobradorSeleccionado.IdZonaNavigation.Id)
                CobradorSeleccionado.IdZona = CobradorSeleccionado.IdZonaNavigation.Id;
            var resultado = await _cobradorController.UpdateCobrador(CobradorSeleccionado);

            if (resultado)
            {
                await Shell.Current.DisplayAlert("Exito", "Cobrador modificado con exito. Recuerda actualizar la tabla del menú 'Donantes'.", "OK");
                await CargarListasAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Ocurrió un error al modificar el cobrador", "OK");
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
                    await Shell.Current.DisplayAlert("Cobrador Eliminado. Recuerda actualizar la tabla del menú 'Donantes'.", "El cobrador ha sido eliminado correctamente.", "OK");
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
