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
    public partial class ZonaAgregarViewModel : ObservableObject
    {
        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        readonly ZonaController _zonaController;

        [ObservableProperty]
        private Zona zonaAgregar = new();

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private ObservableCollection<Zona> listaZonas = [];

        [ObservableProperty]
        private Zona? zonaSeleccionado = null;


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------
        public ZonaAgregarViewModel()
        {
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


        [RelayCommand]
        public async Task AgregarZona()
        {
            try
            {
                if (ZonaAgregar.Nombre == null || ZonaAgregar.Nombre == "")
                {
                    await Shell.Current.DisplayAlert("Error!", "Escribe un nombre adecuado a la Zona", "OK");
                }
                var confirmacion = await Shell.Current.DisplayAlert("Agregar Zona", $"¿Está seguro que desea agregar zona {ZonaAgregar.Nombre}?", "Si", "No");

                if (!confirmacion)
                {
                    return;
                }

                var resultado = await _zonaController.AddZona(ZonaAgregar);
                string mensaje = resultado ? "La zona fue agregado con éxito" : "Ocurrió un error al agregar la zona";

                await Shell.Current.DisplayAlert(resultado ? "Tarea Exitosa" : "Error", mensaje, "OK");
                await CargarListasAsync();
                ZonaAgregar = new();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error!", "Ocurrio un error", "OK");
                Debug.WriteLine(ex.Message);
                return;
            }

        }

        [RelayCommand]
        public async Task ModificarZona()
        {
            if (ZonaSeleccionado != null )
            {
                try
                {
                    if (ZonaSeleccionado.Nombre == "" || ZonaSeleccionado.Nombre == null)
                    {
                        await Shell.Current.DisplayAlert("Error!", "Escribe un nombre adecuado a la Zona", "OK");
                        return;
                    }

                    var confirmacion = await Shell.Current.DisplayAlert("Modificar Zona", "¿Está seguro que desea modificar la zona?", "Si", "No");
                    if (confirmacion)
                    {
                        var resultado = await _zonaController.UpdateZona(ZonaSeleccionado);
                        if (resultado)
                        {
                            await Shell.Current.DisplayAlert("Zona Modificada", "La zona ha sido modificada correctamente.", "OK");
                            await CargarListasAsync();
                        }
                        else
                        {
                            await Shell.Current.DisplayAlert("Error!", "No se ha podido modificar la zona.", "OK");
                        }
                    }

                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error!", "Ocurrio un error", "OK");
                    Debug.WriteLine(ex.Message);
                    return;
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Error!", "No se ha seleccionado ninguna zona.", "OK");
            }
        }

        [RelayCommand]
        public async Task EliminarZonaAsync()
        {
            if (ZonaSeleccionado == null)
            {
                await Shell.Current.DisplayAlert("Error!", "No se ha seleccionado ninguna zona.", "OK");
                return;
            }
            var confirmacion = await Shell.Current.DisplayAlert("Eliminar zona", "¿Está seguro que desea eliminar la zona?", "Si", "No");

            if (confirmacion)
            {
                if (!_zonaController.VerificarEliminarZona(ZonaSeleccionado.Id))
                {
                    await Shell.Current.DisplayAlert("Error!", "No se puede eliminar la zona porque tiene cobradores asignados.", "OK");
                    return;
                }

                var resultado = await _zonaController.DeleteZona(ZonaSeleccionado);

                if (resultado)
                {
                    await Shell.Current.DisplayAlert("Zona Eliminada", "La Zona ha sido eliminado correctamente.", "OK");
                    await CargarListasAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error!", "No se ha podido eliminar la zona.", "OK");
                }
            }
        }

        public async Task CargarListasAsync()
        {
            IsBusy = true;
            await CargarZonas();
            IsBusy = false;
        }


    }
}
