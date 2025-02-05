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
using CommunityToolkit.Mvvm.Messaging;
using AppFundacion.Mensajes;
using Microsoft.Maui.ApplicationModel.Communication;

namespace AppFundacion.ViewModels
{
    public partial class DonantesViewModel : ObservableObject
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        private readonly DonanteController _donanteController;

        private List<Donante> _todosLosDonantes = [];

        [ObservableProperty]
        private string textoBusqueda = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private ObservableCollection<Donante> listaDonantes = [];

        [ObservableProperty]
        private Donante donanteSeleccionado = null!;

        private readonly object _donanteAgregadoToken = new();


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------


        public DonantesViewModel()
        {
            _donanteController = new DonanteController(new FundacionContext());
            IsBusy = true;

            WeakReferenceMessenger.Default.Register<DonanteAgregadoMessage>(this, async (r, m) =>
            {
                if (m.Value) { 
                    await CargarDonantesAsync();
                    FiltrarDonantes();
                }
            });

        }

        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        [RelayCommand]
        public async Task EliminarDonanteAsync()
        {

            if (DonanteSeleccionado == null)
            {
                await Shell.Current.DisplayAlert("Error!", "No se ha seleccionado ningun donante.", "OK");
                return;
            }

            var confirmacion = await Shell.Current.DisplayAlert("Eliminar Donante", "¿Está seguro que desea eliminar el donante?", "Si", "No");

            if (confirmacion)
            {
                DonanteSeleccionado.IdCobradorNavigation = null;
                var resultado = await _donanteController.DeleteDonante(DonanteSeleccionado.Id);

                if (resultado)
                {
                    await Shell.Current.DisplayAlert("Donante Eliminado", "El donante ha sido eliminado correctamente.", "OK");
                    await CargarDonantesAsync();
                    FiltrarDonantes();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error!", "No se ha podido eliminar el donante.", "OK");
                }
            }
        }

        [RelayCommand]
        public void FiltrarDonantes()
        {
            if (string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                ListaDonantes = new ObservableCollection<Donante>(_todosLosDonantes);
            }
            else
            {
                // Separamos las palabras del texto de búsqueda
                var palabrasBusqueda = TextoBusqueda.Split([' '], StringSplitOptions.RemoveEmptyEntries)
                                                     .Select(p => p.ToLower()) // Convertimos todo a minúsculas para una búsqueda insensible a mayúsculas
                                                     .ToList();

                ListaDonantes = new ObservableCollection<Donante>(
                    _todosLosDonantes.Where(d =>
                        // Comprobamos si todas las palabras de búsqueda están presentes en los campos de los donantes
                        palabrasBusqueda.All(palabra =>
                            (d.NombreApellido?.ToLower() ?? "").Contains(palabra) ||
                            (d.Dni?.ToLower() ?? "").Contains(palabra) ||
                            (d.Ciudad?.ToLower() ?? "").Contains(palabra) ||
                            (d.Provincia?.ToLower() ?? "").Contains(palabra)
                        )
                    )
                );

            }
        }


        [RelayCommand]
        async Task ModificarDonante()
        {
            if (DonanteSeleccionado != null)
            {
                var parametroNavigation = new Dictionary<string, object>
                {
                    {"donanteModificar",this.DonanteSeleccionado}
                };

                await Shell.Current.GoToAsync(nameof(DonanteModificarView), parametroNavigation);
            }
            else
            {
                await Shell.Current.DisplayAlert("Error!", "No se ha seleccionado ningun donante.", "OK");
            }
        }

        [RelayCommand]
        public async Task AgregarDonante()
        {
            await Shell.Current.GoToAsync(nameof(DonanteAgregarView));
        }



        public async Task CargarDonantesAsync()
        {
            var donantesList = await _donanteController.GetAllDonantes();
            _todosLosDonantes = donantesList;
            ListaDonantes = new ObservableCollection<Donante>(donantesList);
            IsBusy = false;
        }
    }
}
