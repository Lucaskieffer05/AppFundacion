using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppFundacion.Models;
using AppFundacion.ExcelServices;
using Microsoft.EntityFrameworkCore;
using AppFundacion.Controllers;
using System.Text;
using System.Diagnostics;

namespace AppFundacion.ViewModels
{
    public partial class ReportesViewModel : ObservableObject
    {
        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        private readonly DonanteController _donanteController;
        private readonly ZonaController _zonaController;
        private readonly ExcelControler _excelControler;

        public Dictionary<string, List<Donante>> DonantesPorCobrador;

        [ObservableProperty]
        private int opcionSeleccionada;

        [ObservableProperty]
        private bool visibleDropdown;

        [ObservableProperty]
        private List<Zona> listaZonas = [];

        [ObservableProperty]
        private List<Donante> listaDonantes = [];

        [ObservableProperty]
        private Zona zonaSeleccionada = new();

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public ReportesViewModel()
        {
            _zonaController = new ZonaController(new FundacionContext());
            _donanteController = new DonanteController(new FundacionContext());
            _excelControler = new ExcelControler();
            DonantesPorCobrador = new Dictionary<string, List<Donante>>();
            OpcionSeleccionada = 0;
            VisibleDropdown = false;

        }

        // Método que se ejecuta cuando cambia OpcionSeleccionada
        partial void OnOpcionSeleccionadaChanged(int value)
        {
            Debug.WriteLine($"OpcionSeleccionada cambió a: {value}");
            VisibleDropdown = value == 2 ? true : false;
            OnPropertyChanged(nameof(VisibleDropdown));

        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public async Task CargarListaDeComponentes()
        {
            ListaDonantes = await _donanteController.GetAllDonantes();
            ListaZonas = await _zonaController.GetAllZonas();

            if (ListaZonas.Any())
            {
                ZonaSeleccionada = ListaZonas.First(); // Asignar la primera zona como predeterminada
            }

        }

        // -------------------------------------------------------------------------
        // ----------------------- Crear pagina de Donantes Por Cobrador General ---
        // -------------------------------------------------------------------------

        public string SeleccionReporteHtml()
        {
            if (ZonaSeleccionada == null || ZonaSeleccionada == new Zona() || OpcionSeleccionada == 0 )
            {
                return "";
            }

            if (OpcionSeleccionada == 1)
            {
                // Agrupar por IdCobrador en lugar de CodigoNombre
                DonantesPorCobrador = ListaDonantes
                    .Where(d => d.IdCobradorNavigation != null)
                    .GroupBy(d => d.IdCobradorNavigation!.Id) // Agrupación por IdCobrador
                    .OrderBy(group => group.First().IdCobradorNavigation!.Codigo) // Ordenar los grupos por Codigo
                    .ToDictionary(
                        group => group.First().IdCobradorNavigation!.CodigoNombre, // Usar CodigoNombre como clave
                        group => group.OrderBy(d => d.IdCobradorNavigation!.Codigo).ToList() // Ordenar cada grupo internamente
                    );

                return GenerateHtmlReport(DonantesPorCobrador, zona: "General");
            }
            else if (OpcionSeleccionada == 2)
            {
                // Filtrar primero por ZonaSeleccionada
                var donantesFiltrados = ListaDonantes
                    .Where(d => d.IdCobradorNavigation != null && d.IdCobradorNavigation.IdZona == ZonaSeleccionada.Id);

                // Agrupar por IdCobrador y ordenar por Codigo del cobrador
                DonantesPorCobrador = donantesFiltrados
                    .GroupBy(d => d.IdCobradorNavigation!.Id)
                    .OrderBy(group => group.First().IdCobradorNavigation!.Codigo) // Ordenar grupos por Codigo del cobrador
                    .ToDictionary(
                        group => group.First().IdCobradorNavigation!.CodigoNombre, // Usar CodigoNombre como clave
                        group => group.OrderBy(d => d.IdCobradorNavigation!.Codigo).ToList() // Ordenar donantes dentro de cada grupo
                    );
                return GenerateHtmlReport(DonantesPorCobrador, zona: ZonaSeleccionada.Nombre!);
            }
            else
            {
                return "";
            }
        }



        public string GenerateHtmlReport(Dictionary<string, List<Donante>> donantesPorCobrador, string zona)
        {
            var html = new StringBuilder();
            html.Append(
                "<html>" +
                "<head>" +
                    "<title>Reporte de Donantes por Cobrador</title>" +
                    "<style>" +
                        "body { font-family: Arial, sans-serif; font-size: 12px; margin: 0; padding: 0; }" +
                        ".header { font-family: Times, serif; text-align: left; font-size: 14px; font-weight: bold; margin-bottom: 20px; }" +
                        "table { width: 100%; border-collapse: collapse; font-size: 8px; margin-bottom: 20px; }" +
                        "th, td { border: 1px solid black; padding: 5px; text-align: center; }" +
                        "thead { display: table-header-group; }" +
                        "@media print {" +
                            "@page { margin: 2cm; }" +
                            "body { margin: 0; padding: 0; }" +
                            ".header { position: relative; page-break-after: avoid; }" +
                        "}" +
                    "</style>" +
                "</head>" +
                "<body>" +
                "<div class='header'>FUNDACION SANTAFESINA VIRGEN DE LUJAN -- Reporte de donantes por cobrador</div>" +
                $"<div class='header'>Zona: {zona}</div>"
                );

            foreach (var entry in donantesPorCobrador)
            {
                html.Append($"<h3>Cobrador: {entry.Key}</h3>");
                html.Append(
                    "<table>" +
                        "<thead>" +
                            "<tr>" +
                                "<th>Codigo</th>" +
                                "<th>Nombre y Apellido</th>" +
                                "<th>Domicilio</th>" +
                                "<th>Localidad</th>" +
                                "<th>DNI</th>" +
                                "<th>F. Ingreso</th>" +
                                "<th>Importe</th>" +
                            "</tr>" +
                        "</thead>" +
                        "<tbody>");

                var montoTotal = 0;
                foreach (var donante in entry.Value)
                {
                    html.Append(
                        $"<tr>" +
                            $"<td>{donante.Id}</td>" +
                            $"<td>{donante.NombreApellido}</td>" +
                            $"<td>{donante.Domicilio}</td>" +
                            $"<td>{donante.Ciudad}</td>" +
                            $"<td>{donante.Dni}</td>" +
                            $"<td>{donante.FechaIngreso?.ToString("dd/MM/yyyy")}</td>" +
                            $"<td>{donante.Monto}</td>" +
                        $"</tr>");
                    montoTotal = montoTotal + donante.Monto;
                }
                html.Append("</tbody></table>");
                html.Append($"<h4 style='text-align: right; margin-right: 55px;'>Monto total de {entry.Key} es: ${montoTotal}</h4>");
            }

            html.Append(
                "</body>" +
                "</html>");

            return html.ToString();
        }

    }
}
