using FacturaGat.Helpers;
using FacturaGat.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace FacturaGat.Services
{
    public static class ArchivoXMLService
    {
        public static List<string> ImportarArchivos()
        {
            try
            {
                // Crear un diálogo para seleccionar archivos
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Seleccionar Archivos",
                    Multiselect = true, // Permite seleccionar múltiples archivos
                    Filter = "Archivos XML (*.xml)|*.xml"
                };

                // Mostrar el diálogo y verificar si el usuario seleccionó un archivo
                if (openFileDialog.ShowDialog() == true)
                {
                    List<string> archivosSeleccionados = new List<string>();

                    return archivosSeleccionados = openFileDialog.FileNames.ToList();

                }

                else
                {
                    RetornarError("Selecciona al menos un archivo");
                    return null;
                }
            }

            catch (Exception ex)
            {
                RetornarError(ex.Message);
                return null;
            }
        }

        public static (List<Factura>, List<Factura>, List<Factura>) LeerArchivo(List<string> archivosSeleccionados)
        {
            List<Factura> facts = new List<Factura>();
            List<Factura> factsDevoluciones = new List<Factura>();
            List<Factura> factsPendientesDePago = new List<Factura>();

            foreach (var item in archivosSeleccionados)
            {
                XDocument xDocument = XDocument.Load(item);

                // Definir el namespace
                XNamespace cfdi = "http://www.sat.gob.mx/cfd/4";
                XNamespace tfd = "http://www.sat.gob.mx/TimbreFiscalDigital"; // Namespace del Timbre Fiscal Digital (ajústalo según el XML)
                XNamespace pago20 = "http://www.sat.gob.mx/Pagos20";

                Factura factura = new Factura();

                string formaPago = null;
                // Fecha
                string fecha = xDocument.Descendants(cfdi + "Comprobante")
                                      .FirstOrDefault()?
                                      .Attribute("Fecha")?.Value;
                factura.Fecha = DateTime.TryParse(fecha, out DateTime date) ? date : new DateTime();

                // Folio
                factura.Folio = xDocument.Descendants(cfdi + "Comprobante")
                                      .FirstOrDefault()?
                                      .Attribute("Folio")?.Value;

                // Nombre de Emisor
                factura.Nombre = xDocument.Descendants(cfdi + "Emisor")
                                      .FirstOrDefault()?
                                      .Attribute("Nombre")?.Value;

                // RFC
                factura.RFC = xDocument.Descendants(cfdi + "Emisor")
                                      .FirstOrDefault()?
                                      .Attribute("Rfc")?.Value;

                // Método de pago
                factura.MetodoPago = xDocument.Descendants(cfdi + "Comprobante")
                                  .FirstOrDefault()?
                                  .Attribute("MetodoPago")?.Value;
                // Tipo de comprobante
                factura.TipoDeComprobante = xDocument.Descendants(cfdi + "Comprobante")
                                      .FirstOrDefault()?
                                      .Attribute("TipoDeComprobante")?.Value;

                if (factura.TipoDeComprobante != null && factura.TipoDeComprobante == "P")
                {
                    // Obtener el nodo Totales
                    var totales = xDocument.Descendants(pago20 + "Totales").FirstOrDefault();
                    // Subtotal relacionado al IVA 16%
                    string subtotalBaseIVA16 = totales?.Attribute("TotalTrasladosBaseIVA16")?.Value;
                    factura.Subtotal16 = decimal.TryParse(subtotalBaseIVA16, out decimal sub) ? sub : 0;

                    // IVA relacionado al 16%
                    string totalIVA16 = totales?.Attribute("TotalTrasladosImpuestoIVA16")?.Value;
                    factura.IVA = decimal.TryParse(totalIVA16, out decimal iva) ? iva : 0;

                    // Acceder al nodo <pago20:Pago> y obtener el atributo FormaDePagoP
                    var pago = xDocument.Descendants(pago20 + "Pago").FirstOrDefault();
                    formaPago = pago?.Attribute("FormaDePagoP")?.Value;

                    //Facturas relacionadas
                    factura.FacturaRelacionada = xDocument.Descendants(pago20 + "DoctoRelacionado")
                                                       .Select(doc => doc.Attribute("IdDocumento")?.Value)
                                                       .Where(id => !string.IsNullOrEmpty(id)) // Filtra valores no nulos
                                                       .ToList();

                }
                else
                {
                    // SubTotal 16%
                    string subTotal16 = xDocument.Descendants(cfdi + "Comprobante")
                                      .FirstOrDefault()?
                                      .Attribute("SubTotal")?.Value;

                    factura.Subtotal16 = decimal.TryParse(subTotal16, out decimal subTot16) ? subTot16 : 0;

                    //IVA
                    var impuestosElement = xDocument.Descendants(cfdi + "Impuestos").FirstOrDefault();

                    var listaImpuestosSeccion = xDocument.Descendants(cfdi + "Impuestos")
                      .ToList();

                    // Iterar sobre la lista de nodos <cfdi:Impuestos> para encontrar el atributo TotalImpuestosTrasladados
                    foreach (var impuesto in listaImpuestosSeccion)
                    {
                        // Buscar el atributo TotalImpuestosTrasladados
                        var totalImpuestosTrasladados = impuesto.Attribute("TotalImpuestosTrasladados")?.Value;

                        if (!string.IsNullOrEmpty(totalImpuestosTrasladados))
                        {
                            // Si se encuentra, convertirlo a decimal
                            factura.IVA = decimal.TryParse(totalImpuestosTrasladados, out decimal iv) ? iv : 0;
                        }
                    }

                    // Forma de pago
                    formaPago = xDocument.Descendants(cfdi + "Comprobante")
                                          .FirstOrDefault()?
                                          .Attribute("FormaPago")?.Value;
                }

                factura.Total = factura.Subtotal16 + factura.IVA;

                //Folio fiscal
                factura.FolioFiscal = xDocument.Descendants(cfdi + "Complemento")
                  .Descendants(tfd + "TimbreFiscalDigital")
                  .FirstOrDefault()?
                  .Attribute("UUID")?.Value;

                //Concepto
                string concepto = xDocument.Descendants(cfdi + "Receptor")
                  .FirstOrDefault()?
                  .Attribute("UsoCFDI")?.Value;

                //Buscar Forma de pago
                if (formaPago != null)
                {
                    factura.FormaPago = FormaPagoDictionary.FormasDePago.TryGetValue(formaPago, out string descripcion) ? descripcion : null;
                }

                //Buscar concepto
                if (concepto != null)
                {
                    factura.Concepto = UsoCFDIDictionary.UsosCFDI.TryGetValue(concepto, out string concep) ? concep : null;
                }
                if (formaPago != null && formaPago == "99")
                {
                    factsPendientesDePago.Add(factura);
                }
                else if (concepto != null && concepto == "G02")
                {
                    factsDevoluciones.Add(factura);
                }
                else
                {
                    facts.Add(factura);
                }
            }
            return (facts, factsDevoluciones, factsPendientesDePago);
        }
        public static void RetornarError(string msg)
        {
            MessageBox.Show("Error al leer el archivo: " + msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }
}
