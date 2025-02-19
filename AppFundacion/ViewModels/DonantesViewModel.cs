using AppFundacion.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using AppFundacion.Models;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using AppFundacion.Views;
using CommunityToolkit.Mvvm.Messaging;
using AppFundacion.Mensajes;
using System.Diagnostics;

namespace AppFundacion.ViewModels
{
    public partial class DonantesViewModel : ObservableObject
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        private DonanteController _donanteController;
        private CobradorController _cobradorController;
        private ZonaController _zonaController;

        public List<Donante> _todosLosDonantes = [];

        [ObservableProperty]
        private string textoBusqueda = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private ObservableCollection<Donante> listaDonantes = [];

        [ObservableProperty]
        private ObservableCollection<Cobrador> listaCobradores = [];

        [ObservableProperty]
        private ObservableCollection<Zona> listaZonas = [];

        [ObservableProperty]
        private Cobrador cobradorSeleccionado = new();

        [ObservableProperty]
        private Zona zonaSeleccionada = new();

        [ObservableProperty]
        private Donante donanteSeleccionado = null!;

        private readonly object _donanteAgregadoToken = new();


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------


        public DonantesViewModel()
        {
            _donanteController = new DonanteController(new FundacionContext());
            _cobradorController = new CobradorController(new FundacionContext());
            _zonaController = new ZonaController(new FundacionContext());

            WeakReferenceMessenger.Default.Register<DonanteAgregadoMessage>(this, async (r, m) =>
            {
                if (m.Value) { 
                    await CargarListasAsync();
                    FiltrarDonantes();
                }
            });

            WeakReferenceMessenger.Default.Register<DonanteModificadoMessage>(this, async (r, m) =>
            {
                if (m.Value)
                {
                    await CargarListasAsync();
                    FiltrarDonantes();
                }
            });



        }
        partial void OnCobradorSeleccionadoChanged(Cobrador value)
        {
            FiltrarCobradorZona();
        }

        partial void OnZonaSeleccionadaChanged(Zona value)
        {
            FiltrarCobradorZona();
        }

        public void FiltrarCobradorZona()
        {
            if (CobradorSeleccionado.Id == -1 && ZonaSeleccionada.Id == -1)
            {
                FiltrarDonantes();
            }
            else if (CobradorSeleccionado.Id != -1 && ZonaSeleccionada.Id == -1)
            {
                ListaDonantes = new ObservableCollection<Donante>(_todosLosDonantes.Where(d => d.IdCobrador == CobradorSeleccionado.Id));
                ZonaSeleccionada = CobradorSeleccionado.IdZonaNavigation;
                FiltrarDonantes(new List<Donante>(ListaDonantes));
            }
            else if (CobradorSeleccionado.Id == -1 && ZonaSeleccionada.Id != -1)
            {
                ListaDonantes = new ObservableCollection<Donante>(_todosLosDonantes.Where(d => d.IdCobradorNavigation!.IdZona == ZonaSeleccionada.Id));
                FiltrarDonantes(new List<Donante>(ListaDonantes));
            }
            else
            {
                ListaDonantes = new ObservableCollection<Donante>(_todosLosDonantes.Where(d => d.IdCobrador == CobradorSeleccionado.Id && d.IdCobradorNavigation!.IdZona == ZonaSeleccionada.Id));
                FiltrarDonantes(new List<Donante>(ListaDonantes));
            }
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
                    await CargarListasAsync();
                    FiltrarDonantes();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error!", "No se ha podido eliminar el donante.", "OK");
                }
            }
        }

        [RelayCommand]
        public async Task RecargarTabla()
        {
            _donanteController = new DonanteController(new FundacionContext());
            _cobradorController = new CobradorController(new FundacionContext());
            _zonaController = new ZonaController(new FundacionContext());
            await CargarListasAsync();
            FiltrarDonantes();
        }

        [RelayCommand]
        public void FiltrarDonantes(List<Donante>? auxListaDonantes = null)
        {
            auxListaDonantes ??= _todosLosDonantes;
            if (string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                ListaDonantes = new ObservableCollection<Donante>(auxListaDonantes);
            }
            else
            {
                // Separamos las palabras del texto de búsqueda
                var palabrasBusqueda = TextoBusqueda.Split([' '], StringSplitOptions.RemoveEmptyEntries)
                                                     .Select(p => p.ToLower()) // Convertimos todo a minúsculas para una búsqueda insensible a mayúsculas
                                                     .ToList();

                ListaDonantes = new ObservableCollection<Donante>(
                    auxListaDonantes.Where(d =>
                        // Comprobamos si todas las palabras de búsqueda están presentes en los campos de los donantes
                        palabrasBusqueda.All(palabra =>
                            (d.Id.ToString() ?? "").Contains(palabra) ||
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
                Debug.WriteLine("dasd");
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



        public async Task CargarListasAsync()
        {
            IsBusy = true;
            var donantesList = await _donanteController.GetAllDonantes();
            _todosLosDonantes = donantesList;
            ListaDonantes = new ObservableCollection<Donante>(donantesList);

            CobradorSeleccionado = new Cobrador { Id = -1, Codigo = -1, Nombre = "Ninguno" };
            var cobradorList = await _cobradorController.GetAllCobradores();
            cobradorList.Insert(0, CobradorSeleccionado);
            ListaCobradores = new ObservableCollection<Cobrador>(cobradorList);

            ZonaSeleccionada = new Zona { Id = -1, Nombre = "Ninguna" };
            var zonaList = await _zonaController.GetAllZonas();
            zonaList.Insert(0, ZonaSeleccionada);
            ListaZonas = new ObservableCollection<Zona>(zonaList);

            IsBusy = false;
        }
    }
}
