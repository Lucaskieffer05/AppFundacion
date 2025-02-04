using AppFundacion.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using AppFundacion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace AppFundacion.ViewModels
{
    public partial class DonantesViewModel : ObservableObject
    {
        private readonly DonanteController _donanteController;


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
            ListaDonantes = new ObservableCollection<Donante>(donantesList);
            IsBusy = false;
        }
    }
}
