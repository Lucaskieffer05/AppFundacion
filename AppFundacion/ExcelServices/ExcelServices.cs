using AppFundacion.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Cell = DocumentFormat.OpenXml.Spreadsheet.Cell;
using Font = DocumentFormat.OpenXml.Spreadsheet.Font;
using Color = DocumentFormat.OpenXml.Spreadsheet.Color;
using Border = DocumentFormat.OpenXml.Spreadsheet.Border;
namespace AppFundacion.ExcelServices
{
    public class ExcelControler
    {
        private void LlenarHojaConEncabezado(uint rowIndex, SheetData sheetData, uint styleIndex) { 
            
            List<string> encabezado = ["Cobrador", "Nombre Donante", "Ciudad", "DNI"];
            Row row = new Row() { RowIndex = rowIndex };
            foreach (var titulo in encabezado)
            {
                Cell cell = new Cell()
                {
                    CellValue = new CellValue(titulo),
                    DataType = CellValues.String,
                    StyleIndex = styleIndex
                };
                row.Append(cell);
            }
            sheetData.Append(row);

            

        }
       


        private void LlenarHojaConDatos(uint rowIndex, SheetData sheetData, Dictionary<string, List<Donante>> datos, uint styleIndex) {
            foreach (var entry in datos)
            {
                // Crear una fila para el cobrador
                Row row = new Row() { RowIndex = rowIndex };
                Cell cobradorCell = new Cell()
                {
                    CellValue = new CellValue(entry.Key),  // Cobrador (clave del diccionario)
                    DataType = CellValues.String,
                    StyleIndex = styleIndex
                };
                row.Append(cobradorCell);

                // Si hay donadores, agregar el primero en la misma fila
                if (entry.Value.Count > 0)
                {
                    var donador = entry.Value[0];
                    row.Append(new Cell() { CellValue = new CellValue(donador.NombreApellido), DataType = CellValues.String, StyleIndex=styleIndex });
                    row.Append(new Cell() { CellValue = new CellValue(donador.Ciudad ?? "-"), DataType = CellValues.String, StyleIndex = styleIndex });
                    row.Append(new Cell() { CellValue = new CellValue(donador.Dni ?? "-"), DataType = CellValues.String, StyleIndex = styleIndex });
                }

                sheetData.Append(row);
                rowIndex++;

                // Agregar los donadores restantes en filas nuevas, debajo del cobrador
                for (int i = 1; i < entry.Value.Count; i++)
                {
                    row = new Row() { RowIndex = rowIndex };
                    row.Append(new Cell()); // Celda vacía en la columna A para el cobrador

                    var donador = entry.Value[i]; // Donador
                    row.Append(new Cell() { CellValue = new CellValue(donador.NombreApellido), DataType = CellValues.String, StyleIndex = styleIndex });
                    row.Append(new Cell() { CellValue = new CellValue(donador.Ciudad ?? "-"), DataType = CellValues.String, StyleIndex = styleIndex });
                    row.Append(new Cell() { CellValue = new CellValue(donador.Dni ?? "-"), DataType = CellValues.String, StyleIndex = styleIndex });

                    sheetData.Append(row);
                    rowIndex++;
                }
            }
        }

        
        
        public void CrearExcel(string filepath, Dictionary<string, List<Donante>> datos)
        {
            // Asegurarse de que la carpeta existe
            string directory = Path.GetDirectoryName(filepath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (SpreadsheetDocument document = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                // Agregar estilos al libro
                WorkbookStylesPart stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                stylesPart.Stylesheet = CrearStylesheet();
                stylesPart.Stylesheet.Save();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                SheetData sheetData = new SheetData();

                // Crear y configurar las columnas
                Columns columns = new Columns();
                columns.Append(new Column() { Min = 1, Max = 1, Width = 30, CustomWidth = true }); // Columna A (Cobrador)
                columns.Append(new Column() { Min = 2, Max = 2, Width = 30, CustomWidth = true }); // Columna B (Nombre Donante)
                columns.Append(new Column() { Min = 3, Max = 3, Width = 25, CustomWidth = true }); // Columna C (Ciudad)
                columns.Append(new Column() { Min = 4, Max = 4, Width = 20, CustomWidth = true }); // Columna D (DNI)

                // Crear la hoja de trabajo y agregar las columnas
                Worksheet worksheet = new Worksheet();
                worksheet.Append(columns);
                worksheet.Append(sheetData);
                worksheetPart.Worksheet = worksheet;

                // Agregar hojas al libro
                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Hoja1" };
                sheets.Append(sheet);

                uint rowIndex = 1;
                LlenarHojaConEncabezado(rowIndex, sheetData, 0);
                rowIndex++;
                LlenarHojaConDatos(rowIndex, sheetData, datos, 1);

                worksheetPart.Worksheet.Save();
                workbookPart.Workbook.Save();
            }
        }


        private Stylesheet CrearStylesheet()
        {
            return new Stylesheet(
                new Fonts(
                    // Fuente para títulos (índice 0)
                    new Font(
                        new FontSize() { Val = 14 }, // Tamaño de la letra
                        new Bold(), // Negrita
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } } // Color del texto (negro)
                    ),

                    // Fuente normal (índice 1)
                    new Font(
                        new FontSize() { Val = 11 } // Tamaño normal
                    )
                ),
                new Fills(

                    // Relleno gris para títulos (índice 0)
                    new Fill(
                        new PatternFill() { PatternType = PatternValues.Solid, ForegroundColor =  new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "D9D9D9" } } }
                    )
                    /*// Relleno por defecto (índice 1)
                    new Fill(
                        new PatternFill() { PatternType = PatternValues.None } // Relleno transparente
                    )*/

                ),
                new Borders(
                    // Borde por defecto (índice 0)
                    new Border() { VerticalBorder = new VerticalBorder() { Color = new Color() { Rgb = "#00000" } } }
                ),
                new CellFormats(
                    // Formato normal (índice 0)
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 0, ApplyFont = true, ApplyFill = true },

                    // Formato para títulos (índice 1)
                    new CellFormat() { FontId = 1, BorderId = 0, ApplyFont = false, ApplyFill = false, ApplyBorder=true }
                )
            );
        }


        public ExcelControler()
        {
        }
    }
}