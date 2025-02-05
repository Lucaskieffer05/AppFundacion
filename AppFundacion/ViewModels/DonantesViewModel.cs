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

namespace AppFundacion.ViewModels
{
    public partial class DonantesViewModel : ObservableObject
    {
        private readonly DonanteController _donanteController;

        private List<Donante> _todosLosDonantes = [];

        private string textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => textoBusqueda;
            set
            {
                if (textoBusqueda != value)
                {
                    textoBusqueda = value;
                    OnPropertyChanged(); // Notificar que la propiedad ha cambiado
                }
            }
        }
       
        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    OnPropertyChanged(); // Notificar que la propiedad ha cambiado
                }
            }
        }

        private ObservableCollection<Donante> _listaDonantes = [];
        public ObservableCollection<Donante> ListaDonantes
        {
            get => _listaDonantes;
            set
            {
                if (_listaDonantes != value)
                {
                    _listaDonantes = value;
                    OnPropertyChanged(); // Notificar que la propiedad ha cambiado
                }
            }
        }

        public DonantesViewModel()
        {
            _donanteController = new DonanteController(new FundacionContext());
            IsBusy = true;

        }

        public async Task CargarDonantesAsync()
        {
            var donantesList = await _donanteController.GetAllDonantes();
            _todosLosDonantes = donantesList;
            ListaDonantes = new ObservableCollection<Donante>(donantesList);
            IsBusy = false;
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
                ListaDonantes = new ObservableCollection<Donante>(
                    _todosLosDonantes.Where(d =>
                        d.NombreApellido.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase) ||
                        d.Dni.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase) ||
                        d.Ciudad.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase) ||
                        d.Provincia.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase)
                    )
                );
            }
        }

    }
}
