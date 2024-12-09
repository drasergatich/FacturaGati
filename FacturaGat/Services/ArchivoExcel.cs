using FacturaGat.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using ClosedXML.Excel;

namespace FacturaGat.Services
{
    public static class ArchivoExcel
    {
        public static IXLWorksheet GenerarHoja(XLWorkbook xLWorkbook, string nombre)
        {
            return xLWorkbook.Worksheets.Add("AUXILIAR");
        }
        public static void GuardarArchivo(XLWorkbook workbook, string mes, string year, string nombre)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string defaultDirectory = Path.Combine(documentsPath, "ArchivosExcel");

            // Crear la carpeta si no existe
            if (!Directory.Exists(defaultDirectory))
            {
                Directory.CreateDirectory(defaultDirectory);
            }

            // Configurar el cuadro de diálogo
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Guardar archivo Excel",
                InitialDirectory = defaultDirectory, // Carpeta predeterminada
                FileName = $"{mes} {year} {nombre}.xlsx" // Nombre sugerido
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                workbook.SaveAs(filePath); // Guardar el archivo
                MessageBox.Show("Archivo Excel guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public static void AgregarRegistros(List<Factura> facturas, IXLWorksheet worksheet, int rowInicio)
        {
            int j = 0;
            for (int i = rowInicio; i < facturas.Count + rowInicio; i++)
            {
                //fila, columna
                worksheet.Cell(i, 1).Value = j + 1; // Número de fila
                worksheet.Cell(i, 2).Value = facturas[j].Fecha?.ToString("dd/MM/yyyy"); // Fecha
                worksheet.Cell(i, 3).Value = facturas[j].Folio; // Folio
                worksheet.Cell(i, 4).Value = facturas[j].Nombre; // Nombre
                worksheet.Cell(i, 5).Value = facturas[j].RFC; // RFC
                worksheet.Cell(i, 6).Value = facturas[j].FormaPago; // Forma de pago
                worksheet.Cell(i, 7).Value = facturas[j].MetodoPago; // Método de pago
                worksheet.Cell(i, 8).Value = facturas[j].TipoDeComprobante; // Tipo de comprobante
                worksheet.Cell(i, 9).Value = facturas[j].Subtotal0; // Subtotal 0%
                worksheet.Cell(i, 10).Value = facturas[j].Subtotal16; // Subtotal 16%
                worksheet.Cell(i, 11).Value = facturas[j].IVA; // IVA
                worksheet.Cell(i, 12).Value = facturas[j].Total; // Total
                worksheet.Cell(i, 13).Value = facturas[j].FolioFiscal; // Folio fiscal
                worksheet.Cell(i, 14).Value = facturas[j].Concepto; // Concepto
                if (facturas[j].FacturaRelacionada != null)
                {
                    int k = 15;
                    foreach (var fact in facturas[j].FacturaRelacionada)
                    {
                        worksheet.Cell(i, k).Value = fact;
                        k++;
                    }
                }
                j++;
            }
        }

        public static void GenerarEncabezadosTabla(IXLWorksheet worksheet, int RowInicio, int color, string tituloTabla)
        {
            // 4. Agregar negrita a encabezados
            for (int i = 1; i <= 15; i++)
            {
                worksheet.Cell(RowInicio, i).Style.Font.Bold = true;
                worksheet.Cell(RowInicio + 1, i).Style.Font.Bold = true;
            }

            //Color a encabezado
            worksheet.Range(RowInicio, 1, RowInicio + 1, 15).Style.Fill.BackgroundColor = XLColor.FromArgb(color);

            //// Combinar las celdas de la fila 1, columnas 1 a 15
            //worksheet.Range(1, RowInicio, 1, columnFin).Merge();

            //Titulo
            worksheet.Cell(RowInicio, 2).Value = tituloTabla;

            //Columnas
            worksheet.Cell(RowInicio + 1, 1).Value = "";
            worksheet.Cell(RowInicio + 1, 2).Value = "Fecha";
            worksheet.Cell(RowInicio + 1, 3).Value = "Folio";
            worksheet.Cell(RowInicio + 1, 4).Value = "Nombre";
            worksheet.Cell(RowInicio + 1, 5).Value = "RFC";
            worksheet.Cell(RowInicio + 1, 6).Value = "Forma de pago";
            worksheet.Cell(RowInicio + 1, 7).Value = "Método de pago";
            worksheet.Cell(RowInicio + 1, 8).Value = "Tipo de comprobante";
            worksheet.Cell(RowInicio + 1, 9).Value = "Subtotal 0%";
            worksheet.Cell(RowInicio + 1, 10).Value = "Subtotal 16%";
            worksheet.Cell(RowInicio + 1, 11).Value = "IVA";
            worksheet.Cell(RowInicio + 1, 12).Value = "Total";
            worksheet.Cell(RowInicio + 1, 13).Value = "Folio fiscal";
            worksheet.Cell(RowInicio + 1, 14).Value = "Concepto";
            worksheet.Cell(RowInicio + 1, 15).Value = "Factura relacionada";

            //Formato moneda a columnas
            worksheet.Column(9).Style.NumberFormat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"_);_(@_)";
            worksheet.Column(10).Style.NumberFormat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"_);_(@_)";
            worksheet.Column(11).Style.NumberFormat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"_);_(@_)";
            worksheet.Column(12).Style.NumberFormat.Format = "_($* #,##0.00_);_($* (#,##0.00);_($* \"-\"_);_(@_)";
        }
    }
}
