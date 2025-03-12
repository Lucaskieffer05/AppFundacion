using AppFundacion.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using AppFundacion.Models;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using AppFundacion.Views;
using CommunityToolkit.Mvvm.Messaging;
using AppFundacion.Mensajes;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace AppFundacion.ViewModels
{
    public partial class DonantesViewModel : ObservableObject
    {

        // --------------------------------------------------------------------------
        // ----------------------- Definiciones de DonanteAgregarViewModel ----------
        // --------------------------------------------------------------------------

        [ObservableProperty]
        private Donante? donanteAgregar = null;

        // --------------------------------------------------------------------------
        // ----------------------- Definiciones de DonanteModificarViewModel --------
        // --------------------------------------------------------------------------

        [ObservableProperty]
        private Donante? donanteModificarAgregar = new();

        [ObservableProperty]
        private ObservableCollection<Cobrador> listaCobradoresModificar = [];

        // --------------------------------------------------------------------------
        // ----------------------- Definiciones de DonanteViewModel -----------------
        // --------------------------------------------------------------------------

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

        [ObservableProperty]
        private bool isEnableDropdownZona = true;

        private readonly object _donanteAgregadoToken = new();

        private readonly FundacionContext _context = new();


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------


        public DonantesViewModel()
        {
            _donanteController = new DonanteController(new FundacionContext());
            _cobradorController = new CobradorController(new FundacionContext());
            _zonaController = new ZonaController(new FundacionContext());
            DonanteModificarAgregar = new()
            {
                FechaIngreso = DateTime.Now,
                Pais = "Argentina"
            };

            WeakReferenceMessenger.Default.Register<DonanteAgregadoMessage>(this, async (r, m) =>
            {
                if (m.Value) { 
                    await CargarListasAsync();
                    FiltrarCobradorZona();
                }
            });

            WeakReferenceMessenger.Default.Register<DonanteModificadoMessage>(this, async (r, m) =>
            {
                if (m.Value)
                {
                    await CargarListasAsync();
                    FiltrarCobradorZona();
                }
            });



        }
        partial void OnCobradorSeleccionadoChanged(Cobrador value)
        {
            if (CobradorSeleccionado.Id == -1 && ZonaSeleccionada.Id != -1)
            {
                ZonaSeleccionada = new Zona { Id = -1, Nombre = "Ninguna" };
            }
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
                IsEnableDropdownZona = true;
                FiltrarDonantes();
            }
            else if (CobradorSeleccionado.Id != -1 && ZonaSeleccionada.Id == -1)
            {
                ListaDonantes = new ObservableCollection<Donante>(_todosLosDonantes.Where(d => d.IdCobrador == CobradorSeleccionado.Id));
                ZonaSeleccionada = CobradorSeleccionado.IdZonaNavigation;
                IsEnableDropdownZona = false;
                FiltrarDonantes(new List<Donante>(ListaDonantes));
            }
            else if (CobradorSeleccionado.Id == -1 && ZonaSeleccionada.Id != -1)
            {
                IsEnableDropdownZona = true;
                ListaDonantes = new ObservableCollection<Donante>(_todosLosDonantes.Where(d => d.IdCobradorNavigation!.IdZona == ZonaSeleccionada.Id));
                FiltrarDonantes(new List<Donante>(ListaDonantes));
            }
            else
            {
                ListaDonantes = new ObservableCollection<Donante>(_todosLosDonantes.Where(d => d.IdCobrador == CobradorSeleccionado.Id && d.IdCobradorNavigation!.IdZona == ZonaSeleccionada.Id));
                ZonaSeleccionada = CobradorSeleccionado.IdZonaNavigation;
                IsEnableDropdownZona = false;
                FiltrarDonantes(new List<Donante>(ListaDonantes));
            }
        }

        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB de DonanteViewModel -----------------
        // -------------------------------------------------------------------

        [RelayCommand]
        public async Task EliminarDonanteAsync()
        {
            if (DonanteModificarAgregar == null)
            {
                await Shell.Current.DisplayAlert("Error!", "No se ha seleccionado ningun donante.", "OK");
                return;
            }

            var confirmacion = await Shell.Current.DisplayAlert("Eliminar Donante", "¿Está seguro que desea eliminar el donante?", "Si", "No");

            if (confirmacion)
            {
                DonanteModificarAgregar.IdCobradorNavigation = null;
                var resultado = await _donanteController.DeleteDonante(DonanteModificarAgregar.Id);

                if (resultado)
                {
                    await Shell.Current.DisplayAlert("Donante Eliminado", "El donante ha sido eliminado correctamente.", "OK");
                    await CargarListasAsync();
                    FiltrarCobradorZona();
                    DonanteModificarAgregar = new()
                    {
                        FechaIngreso = DateTime.Now,
                        Pais = "Argentina"
                    };
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
            await CargarListasAsync(true);
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
            }
            else
            {
                await Shell.Current.DisplayAlert("Error!", "No se ha seleccionado ningun donante.", "OK");
            }
        }



        public async Task CargarListasAsync(bool actualizarTodo = false)
        {
            IsBusy = true;

            var donantesList = await _donanteController.GetAllDonantes();
            _todosLosDonantes = donantesList;
            ListaDonantes = new ObservableCollection<Donante>(donantesList);

            if (actualizarTodo)
            {
                CobradorSeleccionado = new Cobrador { Id = -1, Codigo = -1, Nombre = "Ninguno" };
                var cobradorList = await _cobradorController.GetAllCobradores();
                cobradorList.Insert(0, CobradorSeleccionado);
                ListaCobradores = new ObservableCollection<Cobrador>(cobradorList);
                ListaCobradoresModificar = new ObservableCollection<Cobrador>(cobradorList);

                ZonaSeleccionada = new Zona { Id = -1, Nombre = "Ninguna" };
                var zonaList = await _zonaController.GetAllZonas();
                zonaList.Insert(0, ZonaSeleccionada);
                ListaZonas = new ObservableCollection<Zona>(zonaList);
            }

            DonanteModificarAgregar = new()
            {
                FechaIngreso = DateTime.Now,
                Pais = "Argentina"
            };

            IsBusy = false;
        }


        // -----------------------------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB de DonanteModificarViewModel ----------
        // -----------------------------------------------------------------------------------------

        [RelayCommand]
        async Task ModficiarDonador()
        {
            if (DonanteModificarAgregar == null || DonanteModificarAgregar.NombreApellido == null || DonanteModificarAgregar.NombreApellido == "")
            {
                await Shell.Current.DisplayAlert("Error", "Debes seleccionar un nombre adecuado", "OK");
                return;
            }

            if (DonanteModificarAgregar.Monto <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "Debes ingresar un monto adecuado", "OK");
                return;
            }


            if (DonanteModificarAgregar == null || DonanteModificarAgregar.IdCobradorNavigation == null)
            {
                await Shell.Current.DisplayAlert("Error", "Ocurrió un error al modificar el donante", "OK");
                return;
            }
            if (DonanteModificarAgregar.IdCobrador != DonanteModificarAgregar.IdCobradorNavigation.Id)
            {
                DonanteModificarAgregar.IdCobrador = DonanteModificarAgregar.IdCobradorNavigation.Id;
            }
            var resultado = await _donanteController.UpdateDonante(DonanteModificarAgregar);
            if (!resultado)
            {
                await Shell.Current.DisplayAlert("Error", "Ocurrió un error al modificar el donante", "OK");
                return;
            }

            await Shell.Current.DisplayAlert("Modificado", "El donante se modificó correctamente", "OK");

            await CargarListasAsync();
            FiltrarCobradorZona();
        }


        // -----------------------------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB de DonanteAgregarViewModel ------------
        // -----------------------------------------------------------------------------------------


        [RelayCommand]
        async Task LimpiarAsync()
        {
            await CargarListasAsync();
            FiltrarCobradorZona();
            DonanteModificarAgregar = new Donante
            {
                FechaIngreso = DateTime.Now,
                Pais = "Argentina"
            };
        }

        [RelayCommand]
        async Task AgregarDonante()
        {
            if (DonanteModificarAgregar == null || DonanteModificarAgregar.IdCobradorNavigation == null || DonanteModificarAgregar.NombreApellido == "" || DonanteModificarAgregar.Monto < 0)
            {
                await Shell.Current.DisplayAlert("Error", "Ocurrió un error al agregar el donante. Verifica el campo de cobrador, nombre y monto", "OK");
                return;
            }

            var respuesta = await _donanteController.DonanteExists(DonanteModificarAgregar.Dni!);

            if (respuesta)
            {
                await Shell.Current.DisplayAlert("Error", "Ya existe un donante con el DNI ingresado", "OK");
                return;
            }



            // Crear un nuevo objeto Donante para agregar

            var nuevoDonante = new Donante
            {
                Dni = DonanteModificarAgregar.Dni,
                NombreApellido = DonanteModificarAgregar.NombreApellido,
                Ciudad = DonanteModificarAgregar.Ciudad,
                Provincia = DonanteModificarAgregar.Provincia,
                Pais = DonanteModificarAgregar.Pais,
                FechaIngreso = DonanteModificarAgregar.FechaIngreso,
                Monto = DonanteModificarAgregar.Monto,
                Domicilio = DonanteModificarAgregar.Domicilio,
                IdCobrador = DonanteModificarAgregar.IdCobradorNavigation.Id
            };

            ReemplazarNulosConGuion(nuevoDonante);

            // Evita que Entity Framework intente insertar un nuevo Cobrador
            nuevoDonante.IdCobradorNavigation = null;

            _donanteController = new DonanteController(new FundacionContext());
            var resultado = await _donanteController.AddDonante(nuevoDonante);
            string mensaje = resultado ? "El donante fue agregado con éxito" : "Ocurrió un error al agregar el donante";

            await Shell.Current.DisplayAlert(resultado ? "Tarea Exitosa" : "Error", mensaje, "OK");

            if (resultado)
            {
                DonanteModificarAgregar = new Donante
                {
                    FechaIngreso = DateTime.Now,
                    Pais = "Argentina"
                };
            }

            await CargarListasAsync();
            FiltrarCobradorZona();
        }



        void ReemplazarNulosConGuion(Donante donante)
        {
            foreach (var prop in typeof(Donante).GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    var valor = (string?)prop.GetValue(donante);
                    if (string.IsNullOrEmpty(valor))
                    {
                        prop.SetValue(donante, "-");
                    }
                }
            }
        }


    }
}
