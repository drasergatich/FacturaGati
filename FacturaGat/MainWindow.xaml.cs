using ClosedXML.Excel;
using FacturaGat.Models;
using FacturaGat.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FacturaGat
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string mes = "";
            var selectedMes = cmbMes.SelectedItem as ComboBoxItem;
            if (selectedMes != null)
            {
                mes = selectedMes.Content.ToString();
            }
            string year = "";
            var selectedYear = cmbYear.SelectedItem as ComboBoxItem;
            if (selectedYear != null)
            {
                year = selectedYear.Content.ToString();
            }

            string nombre = txtBxNombre.Text;

            List<Factura> facts = new List<Factura>();
            List<Factura> factsDevoluciones = new List<Factura>();
            List<Factura> factsPendientesDePago = new List<Factura>();

            List<string> archivosSeleccionados = ArchivoXMLService.ImportarArchivos();

            // Crear un libro de Excel
            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet xLWorksheet = ArchivoExcel.GenerarHoja(workbook, "AUXILIAR");

                (facts, factsDevoluciones, factsPendientesDePago) = ArchivoXMLService.LeerArchivo(archivosSeleccionados);

                //Primera tabla : Facturas
                ArchivoExcel.GenerarEncabezadosTabla(xLWorksheet, 1, 0xdbe4ed, "");

                ArchivoExcel.AgregarRegistros(facts, xLWorksheet, 3);

                //segunda tabla : devoluciones
                ArchivoExcel.GenerarEncabezadosTabla(xLWorksheet, facts.Count + 4, 0xff7676, "NOTAS DE CREDITO POR DEVOLUCIONES Y DESCUENTOS");

                ArchivoExcel.AgregarRegistros(factsDevoluciones, xLWorksheet, facts.Count + 6);

                //tercera tabla : devoluciones
                ArchivoExcel.GenerarEncabezadosTabla(xLWorksheet, factsDevoluciones.Count + 1 + facts.Count + 6, 0x94da82, "FACTURAS PENDIENTES DE PAGO");

                ArchivoExcel.AgregarRegistros(factsPendientesDePago, xLWorksheet, factsDevoluciones.Count + 3 + facts.Count + 6);

                // Ajustar automáticamente las columnas
                xLWorksheet.Columns().AdjustToContents();

                ArchivoExcel.GuardarArchivo(workbook, mes, year, nombre);
            }
        }
    }
}
