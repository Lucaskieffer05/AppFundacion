using AppFundacion.Controllers;
using AppFundacion.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace AppFundacion.ViewModels
{
    public partial class FuncionalidadesExtrasViewModel : ObservableObject
    {
        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------


        private readonly DonanteController _donanteController;
        private readonly CobradorController _cobradorController;
        private readonly ZonaController _zonaController;

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
        private ObservableCollection<Cobrador> listaCobradoresOrigen = [];

        [ObservableProperty]
        private ObservableCollection<Zona> listaZonas = [];

        [ObservableProperty]
        private Cobrador cobradorSeleccionado = new();

        [ObservableProperty]
        private Cobrador? cobradorSeleccionadoDestino = null;

        [ObservableProperty]
        private Zona zonaSeleccionada = new();

        [ObservableProperty]
        private int cantidadDonantes = 0;

        [ObservableProperty]
        private int? viejoMonto = null;

        [ObservableProperty]
        private bool menorIgual = false;


        [ObservableProperty]
        private int? nuevoMonto = null;

        private readonly object _donanteAgregadoToken = new();

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public FuncionalidadesExtrasViewModel()
        {
            _donanteController = new DonanteController(new FundacionContext());
            _cobradorController = new CobradorController(new FundacionContext());
            _zonaController = new ZonaController(new FundacionContext());

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
                if (CobradorSeleccionado.IdZonaNavigation != null)
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

            CantidadDonantes = ListaDonantes.Count;

        }

        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

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
        public async Task AsignarDonantesACobrador()
        {
            if( CobradorSeleccionadoDestino== null || CobradorSeleccionadoDestino.Id == -1)
            {
                await Shell.Current.DisplayAlert("Atención", "Debe seleccionar un cobrador destino", "Aceptar");
                return;
            }

            if (ListaDonantes.Count == 0)
            {
                await Shell.Current.DisplayAlert("Atención", "Debe seleccionar al menos un donante", "Aceptar");
                return;
            }

            var confirmacion1 = await Shell.Current.DisplayAlert("Transferir donante a un nuevo cobrador", 
                                                                $"¿Está seguro que desea transferir los {CantidadDonantes} dontantes seleccionados " +
                                                                $"al nuevo cobrador destino {CobradorSeleccionadoDestino.CodigoNombre} ?", "Si", "No");

            if (!confirmacion1) return;

            var confirmacion2 = await Shell.Current.DisplayAlert("Atención!!", "Esta operación realiza cambios importantes en el sistema. ¿Quiere proceder con esta tarea?", "Proceder", "Cancelar");

            if (!confirmacion2) return;

            var listaDonantes = new List<Donante>(ListaDonantes);
            var resultado = await _donanteController.TransferirDonantes(listaDonantes, CobradorSeleccionadoDestino);

            if (resultado)
            {
                await Shell.Current.DisplayAlert("Transferencia de donantes", "La transferencia de donantes se realizó con éxito. Recuerda actualizar la tabla del menú 'Donantes'.", "Aceptar");
                await CargarListasAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert("Transferencia de donantes", "Ocurrió un error al transferir los donantes", "Aceptar");
            }

        }


        [RelayCommand]
        public async Task ActualizarMontoDonantes()
        {
            if (ViejoMonto == null || ViejoMonto <= 0)
            {
                await Shell.Current.DisplayAlert("Atención", "Debes escribir un monto a reemplazar adecuado", "Aceptar");
                return;
            }

            if (NuevoMonto == null || NuevoMonto <= 0)
            {
                await Shell.Current.DisplayAlert("Atención", "Debes escribir un nuevo monto adecuado", "Aceptar");
                return;
            }

            var confirmacion1 = await Shell.Current.DisplayAlert("Actualizar el monto de los donantes seleciconados",
                                                                $"¿Está seguro que desea actualizar el monto de ${ViejoMonto} de los {CantidadDonantes} dontantes seleccionados " +
                                                                $"al nuevo monto de ${NuevoMonto}?", "Si", "No");

            if (!confirmacion1) return;

            var confirmacion2 = await Shell.Current.DisplayAlert("Atención!!", "Esta operación realiza cambios importantes en el sistema. ¿Quiere proceder con esta tarea?", "Proceder", "Cancelar");

            if (!confirmacion2) return;

            var listaDonantes = new List<Donante>(ListaDonantes);
            var (resultado, cantidad) = await _donanteController.ActualizarMontos(listaDonantes, ViejoMonto, NuevoMonto, MenorIgual);
                

            if (resultado)
            {
                if (cantidad == 0)
                {
                    await Shell.Current.DisplayAlert("Actualizar montos", "No se encontraron donantes con el monto a reemplazar", "Aceptar");
                    return;
                }
                await Shell.Current.DisplayAlert("Actualizar montos", $"Se actualizaron los montos de {cantidad} donantes. Recuerda actualizar la tabla del menú 'Donantes'.", "Aceptar");
                await CargarListasAsync();
                FiltrarDonantes();
                MenorIgual = false;
            }
            else
            {
                await Shell.Current.DisplayAlert("Transferencia de donantes", "Ocurrió un error al transferir los donantes", "Aceptar");
            }


        }

        public async Task CargarListasAsync()
        {
            IsBusy = true;
            var donantesList = await _donanteController.GetAllDonantes();
            _todosLosDonantes = donantesList;
            ListaDonantes = new ObservableCollection<Donante>(donantesList);

            CobradorSeleccionado = new Cobrador { Id = -1, Codigo = -1, Nombre = "Todos" };
            var cobradorList = await _cobradorController.GetAllCobradores();
            ListaCobradoresOrigen = new ObservableCollection<Cobrador>(cobradorList);
            cobradorList.Insert(0, CobradorSeleccionado);
            ListaCobradores = new ObservableCollection<Cobrador>(cobradorList);

            ZonaSeleccionada = new Zona { Id = -1, Nombre = "Todos" };
            var zonaList = await _zonaController.GetAllZonas();
            zonaList.Insert(0, ZonaSeleccionada);
            ListaZonas = new ObservableCollection<Zona>(zonaList);

            IsBusy = false;
        }

    }
}
