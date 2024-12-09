using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacturaGat.Models
{
    public class Factura
    {
        public DateTime? Fecha { get; set; } // Fecha de la factura
        public string Folio { get; set; } // Folio de la factura
        public string Nombre { get; set; } // Nombre del cliente o emisor
        public string RFC { get; set; } // RFC del cliente o emisor
        public string FormaPago { get; set; } // Forma de pago (efectivo, transferencia, etc.)
        public string MetodoPago { get; set; } // Método de pago (PUE, PPD, etc.)
        public string TipoDeComprobante { get; set; } // Tipo de comprobante (Ingreso, Egreso, etc.)
        public decimal Subtotal0 { get; set; } // Subtotal gravado al 0%
        public decimal? Subtotal16 { get; set; } // Subtotal gravado al 16%
        public decimal? IVA { get; set; } // IVA calculado
        public decimal? Total { get; set; } // Total de la factura
        public string FolioFiscal { get; set; } // Folio fiscal (UUID)
        public string Concepto { get; set; } // Concepto general de la factura
        public List<string> FacturaRelacionada { get; set; } // Factura relacionada (si aplica)    }
    }
}
