using AppFundacion.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using AppFundacion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using AppFundacion.Views;

namespace AppFundacion.ViewModels
{
    [QueryProperty(nameof(DonanteModificar), "donanteModificar")]
    public partial class DonantesModificarViewModel : ObservableObject
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        private readonly CobradorController _cobradorController;
        private readonly DonanteController _donanteController;

        [ObservableProperty]
        private Donante? donanteModificar;

        [ObservableProperty]
        private ObservableCollection<Cobrador> listaCobradores = [];


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public DonantesModificarViewModel()
        {
            _donanteController = new DonanteController(new FundacionContext());
            _cobradorController = new CobradorController(new FundacionContext());
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------


        [RelayCommand]
        async Task VolverAtras()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        async Task ModficiarDonador()
        {
            if (DonanteModificar == null || DonanteModificar.IdCobradorNavigation == null)
            {
                await Shell.Current.DisplayAlert("Error", "Ocurrió un error al modificar el donante", "OK");
                return;
            }
            var resultado = await _donanteController.UpdateDonante(DonanteModificar);
            if (resultado)
            {
                await Shell.Current.DisplayAlert("Exito", "Donante modificado con exito", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Ocurrió un error al modificar el donante", "OK");
            }
        }


        public async Task CargarCobradoresAsync()
        {
            var cobradoresList = await _cobradorController.GetAllCobradores();
            ListaCobradores = new ObservableCollection<Cobrador>(cobradoresList);
        }
    }
}
