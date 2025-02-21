using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppFundacion.Controllers;
using System.Collections.ObjectModel;
using AppFundacion.Models;

namespace AppFundacion.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly DonanteController _donanteController;
        private readonly ZonaController _zonaController;

        [ObservableProperty]
        private int totalDonantes;

        [ObservableProperty]
        private ObservableCollection<Cobrador> cobradoresSinDonantes;

        [ObservableProperty]
        private ObservableCollection<KeyValuePair<string, int>> donantesPorZona;

        public HomeViewModel()
        {
            _donanteController = new DonanteController(new FundacionContext());
            _zonaController = new ZonaController(new FundacionContext());
            CobradoresSinDonantes = [];
            DonantesPorZona = [];
        }

        [RelayCommand]
        public async Task CargarEstadisticas()
        {
            TotalDonantes = await _donanteController.GetTotalDonantes();
            CobradoresSinDonantes = new ObservableCollection<Cobrador>(await _donanteController.GetCobradoresSinDonantes());
            var donantesPorZonaDict = await _zonaController.GetDonantesPorZona();
            DonantesPorZona = new ObservableCollection<KeyValuePair<string, int>>(donantesPorZonaDict);
        }
    }
}
