using CommunityToolkit.Mvvm.ComponentModel;
using AppFundacion.Models;
using AppFundacion.Controllers;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using AppFundacion.Mensajes;
using CommunityToolkit.Mvvm.Messaging;
using System.Runtime.CompilerServices;

namespace AppFundacion.ViewModels
{
    [QueryProperty(nameof(CobradorModificar), "cobradorModificar")]
    public partial class CobradorModificarViewModel : ObservableObject
    {
        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        readonly ZonaController _zonaController;
        readonly CobradorController _cobradorController;

        private int CodigoCobrador;

        [ObservableProperty]
        private Cobrador cobradorModificar = new();

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private ObservableCollection<Zona> listaZonas = [];


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------
        public CobradorModificarViewModel()
        {
            _zonaController = new ZonaController(new FundacionContext());
            _cobradorController = new CobradorController(new FundacionContext());

        }

        partial void OnCobradorModificarChanged(Cobrador value)
        {
            if (value != null)
            {
                CodigoCobrador = value.Codigo;
            }
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public async Task CargarZonas()
        {
            var zonaList = await _zonaController.GetAllZonas();
            ListaZonas = new ObservableCollection<Zona>(zonaList);
        }
        
        public async Task CargarListasAsync()
        {
            await CargarZonas();
        }

        [RelayCommand]
        public async Task ModificarCobrador()
        {
            // Validar que la zona seleccionada no sea null
            if (CobradorModificar.IdZonaNavigation == null)
            {
                await Shell.Current.DisplayAlert("Error", "Debe seleccionar una zona", "OK");
                return;
            }
            if (CobradorModificar.Nombre == null || CobradorModificar?.Codigo == null || CobradorModificar.Nombre == "" )
            {
                await Shell.Current.DisplayAlert("Error", "Debe completar todos los campos", "OK");
                return;
            }

            if (CodigoCobrador != CobradorModificar.Codigo)
            {
                var validarCodigo = _cobradorController.VerificarCodigoDuplicado(CobradorModificar.Codigo);               
                if (validarCodigo)
                {
                    await Shell.Current.DisplayAlert("Error", "El código ya existe", "OK");
                    return;
                }
            }



            if (CobradorModificar.IdZonaNavigation != null && CobradorModificar.IdZona != CobradorModificar.IdZonaNavigation.Id)
                CobradorModificar.IdZona = CobradorModificar.IdZonaNavigation.Id;
            var resultado = await _cobradorController.UpdateCobrador(CobradorModificar);

            if (resultado)
            {
                await Shell.Current.DisplayAlert("Exito", "Cobrador modificado con exito. Recuerda actualizar la tabla del menú 'Donantes'.", "OK");
                await Shell.Current.GoToAsync("..");
                WeakReferenceMessenger.Default.Send(new CobradorModificarMessage(true));
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Ocurrió un error al modificar el cobrador", "OK");
            }

        }

        [RelayCommand]
        public async Task VolverAtras()
        {
            await Shell.Current.GoToAsync("..");
        }


    }
}
