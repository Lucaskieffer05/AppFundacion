using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppFundacion.Models;
using AppFundacion.ExcelServices;
using Microsoft.EntityFrameworkCore;
using AppFundacion.Controllers;
using System.Text;
using System.Diagnostics;
using Microsoft.Extensions.Primitives;
using DocumentFormat.OpenXml.Drawing.Charts;

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
        private int opcionSeleccionadaTarjeta;

        [ObservableProperty]
        private bool visibleDropdown;

        [ObservableProperty]
        private bool visibleDropdownTarjeta;

        [ObservableProperty]
        private List<Zona> listaZonas = [];

        [ObservableProperty]
        private List<Donante> listaDonantes = [];

        [ObservableProperty]
        private Zona? zonaSeleccionada = null;

        [ObservableProperty]
        private Zona? zonaSeleccionadaTarjeta = null;

        [ObservableProperty]
        private int mesPerido;

        [ObservableProperty]
        private int añoPerido;




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
            OpcionSeleccionadaTarjeta = 0;
            VisibleDropdown = false;
            VisibleDropdownTarjeta = false;
            MesPerido = DateTime.Now.Month;
            AñoPerido = DateTime.Now.Year;
        }

        // Método que se ejecuta cuando cambia OpcionSeleccionada
        partial void OnOpcionSeleccionadaChanged(int value)
        {
            Debug.WriteLine($"OpcionSeleccionada cambió a: {value}");
            VisibleDropdown = value == 2 ? true : false;
            OnPropertyChanged(nameof(VisibleDropdown));

        }

        // Método que se ejecuta cuando cambia OpcionSeleccionada
        partial void OnOpcionSeleccionadaTarjetaChanged(int value)
        {
            Debug.WriteLine($"OpcionSeleccionada cambió a: {value}");
            VisibleDropdownTarjeta = value == 2 ? true : false;
            OnPropertyChanged(nameof(VisibleDropdownTarjeta));

        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        public async Task CargarListaDeComponentes()
        {
            ListaDonantes = await _donanteController.GetAllDonantes();
            ListaZonas = await _zonaController.GetAllZonas();
        }

        // -------------------------------------------------------------------------
        // ----------------------- Crear pagina de Donantes Por Cobrador General ---
        // -------------------------------------------------------------------------

        public string SeleccionReporteHtml()
        {
            if (OpcionSeleccionada == 0 )
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

                return GenerateHtmlReport(DonantesPorCobrador, zona: "General", periodo: $"{MesPerido}/{AñoPerido}");
            }
            else if (OpcionSeleccionada == 2)
            {
                if (ZonaSeleccionada == null || ZonaSeleccionada == new Zona())
                {
                    return "";
                }
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
                return GenerateHtmlReport(DonantesPorCobrador, zona: ZonaSeleccionada.Nombre!, periodo: $"{MesPerido}/{AñoPerido}");
            }
            else
            {
                return "";
            }
        }

        public string SeleccionTarjetasHtml()
        {
            if (OpcionSeleccionadaTarjeta == 0)
            {
                return "";
            }

            if (OpcionSeleccionadaTarjeta == 1)
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

                return GenerateHtmlTarjetas(DonantesPorCobrador, zona: "General", periodo: $"{MesPerido}/{AñoPerido}");
            }
            else if (OpcionSeleccionadaTarjeta == 2)
            {
                if (ZonaSeleccionadaTarjeta == null || ZonaSeleccionadaTarjeta == new Zona())
                {
                    return "";
                }

                // Filtrar primero por ZonaSeleccionadaTarjeta
                var donantesFiltrados = ListaDonantes
                    .Where(d => d.IdCobradorNavigation != null && d.IdCobradorNavigation.IdZona == ZonaSeleccionadaTarjeta.Id);

                // Agrupar por IdCobrador y ordenar por Codigo del cobrador
                DonantesPorCobrador = donantesFiltrados
                    .GroupBy(d => d.IdCobradorNavigation!.Id)
                    .OrderBy(group => group.First().IdCobradorNavigation!.Codigo) // Ordenar grupos por Codigo del cobrador
                    .ToDictionary(
                        group => group.First().IdCobradorNavigation!.CodigoNombre, // Usar CodigoNombre como clave
                        group => group.OrderBy(d => d.IdCobradorNavigation!.Codigo).ToList() // Ordenar donantes dentro de cada grupo
                    );
                return GenerateHtmlTarjetas(DonantesPorCobrador, zona: ZonaSeleccionadaTarjeta.Nombre!, periodo: $"{MesPerido}/{AñoPerido}");
            }
            else
            {
                return "";
            }
        }





        public string GenerateHtmlReport(Dictionary<string, List<Donante>> donantesPorCobrador, string zona, string periodo)
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
                        "th, td { border: 1px solid black; padding: 2px 5px; text-align: center; line-height: 1.2; }" + // Ajuste de espacio
                        "thead { display: table-header-group; background-color: #d3d3d3; -webkit-print-color-adjust: exact; print-color-adjust: exact; }" +
                        "@media print {" +
                            "@page { margin: 2cm; }" +
                            "body { margin: 0; padding: 0; }" +
                            ".header { position: relative; page-break-after: avoid; }" +
                            "thead th { background-color: #d3d3d3 !important; -webkit-print-color-adjust: exact; print-color-adjust: exact; }" +
                        "}" +
                    "</style>" +
                "</head>" +
                "<body>" +
                "<div class='header'>FUNDACION SANTAFESINA VIRGEN DE LUJAN -- Reporte de donantes por cobrador</div>" +
                $"<div class='header'>Zona: {zona}</div>" +
                $"<div class='header'>Periodo: {periodo}</div>"
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


        public string GenerateHtmlTarjetas(Dictionary<string, List<Donante>> donantesPorCobrador, string zona, string periodo)
        {
            var html = new StringBuilder();
            html.Append(
                "<html>" +
                "<head>" +
                    "<title>Reporte de Donantes por Cobrador</title>" +
                    "<style>" +
                        "body { font-family: Arial, sans-serif; font-size: 12px; margin: 0; padding: 0; }" +
                        ".header { font-family: Times, serif; text-align: left; font-size: 14px; font-weight: bold; margin-bottom: 20px; }" +
                        ".donantes-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 0px; }" +
                        ".donante-title { text-align: center; font-weight: bold; font-size: 13px; margin-bottom: 0px; }" +
                        ".donante-card { border: 1px dotted black; padding: 10px; font-size: 8px; page-break-inside: avoid; break-inside: avoid; }" +
                        ".donante-info { margin-bottom: 5px;}" +
                        "table { width: 100%; font-size: 13px; border-collapse: collapse;}" +
                        "tr { margin: 0; line-height: 1; padding: 0 }" + 
                        "td.label { text-align: left; width: 30%; }" + 
                        "td.value { width: 70%; font-weight: bold; }" + 
                        "strong { font-family: 'Courier New', Courier, monospace; }" +
                        "@media print {" +
                            "@page { margin: 0cm; }" +
                            "body { margin: 0; padding: 0; }" +
                            ".header { position: relative; page-break-after: avoid; }" +
                            ".donantes-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); }" + 
                            ".donante-card { page-break-inside: avoid; break-inside: avoid; }" + 
                        "}" +
                    "</style>" +
                "</head>" +
                "<body>" +
                "<div class='header'>FUNDACION SANTAFESINA VIRGEN DE LUJAN -- Reporte de donantes por cobrador</div>" +
                $"<div class='header'>Zona: {zona}</div>" +
                $"<div class='header'>Periodo: {periodo}</div>"
            );

            foreach (var entry in donantesPorCobrador)
            {
                html.Append($"<h3>Cobrador: {entry.Key}</h3>");
                html.Append("<div class='donantes-grid'>"); // Inicia la grilla

                foreach (var donante in entry.Value)
                {
                    var cobradorNombre = donante.IdCobradorNavigation?.CodigoNombre ?? "N/A";
                    html.Append(
                        "<div class='donante-card'>" +
                            $"<div class='donante-title'>FUNDACION SANTAFESINA VIRGEN DE LUJAN</div>" + // Título añadido
                            $"<div style='text-align: center; margin-bottom: 10px; font-size: 11px'>Para enfermos Oncológicos sin recursos</div>" +
                            "<table>" +
                                $"<tr><td class='label'>Dte. Volunt:</td><td class='value' colspan='3'><strong>{donante.Id} {donante.NombreApellido}</strong></td></tr>" +
                                $"<tr><td class='label'>Domicilio:</td><td class='value' colspan='3'><strong>{donante.Domicilio}</strong></td></tr>" +
                                $"<tr><td class='label'>Localidad:</td><td class='value' colspan='3'><strong>{donante.Ciudad}</strong></td></tr>" +
                                $"<tr><td class='label'>F.Ingreso:</td><td class='value' colspan='3'><strong>{donante.FechaIngreso?.ToString("dd/MM/yyyy")}</strong></td></tr>" +
                                $"<tr><td class='label'>Periodo:</td><td style='width: 30%'><strong>{periodo}</strong></td><td>Importe:</td><td class='value' ><strong>${donante.Monto}</strong></td></tr>" +
                                $"<tr><td class='label'>Cobrador:</td><td class='value' colspan='3'><strong>{cobradorNombre}</strong></td></tr>" +
                            "</table>" +
                        "</div>"
                    );
                }

                html.Append("</div>"); // Cierra la grilla
            }

            html.Append(
                "</body>" +
                "</html>");

            return html.ToString();
        }





    }
}
