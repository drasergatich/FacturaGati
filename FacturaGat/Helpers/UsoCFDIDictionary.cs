using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacturaGat.Helpers
{
    public static class UsoCFDIDictionary
    {
        public static readonly Dictionary<string, string> UsosCFDI = new Dictionary<string, string>
{
    { "G01", "Adquisición de mercancías" },
    { "G02", "Devoluciones, descuentos o bonificaciones" },
    { "G03", "Gastos en general" },
    { "I01", "Construcciones" },
    { "I02", "Mobiliario y equipo de oficina para inversiones" },
    { "I03", "Equipo de transporte" },
    { "I04", "Equipo de cómputo y accesorios" },
    { "I05", "Dados, troqueles, moldes, matrices y herramental" },
    { "I06", "Comunicaciones telefónicas" },
    { "I07", "Comunicaciones satelitales" },
    { "I08", "Otra maquinaria y equipo" },
    { "D01", "Honorarios médicos, dentales y hospitalarios" },
    { "D02", "Gastos médicos por incapacidad o discapacidad" },
    { "D03", "Gastos funerales" },
    { "D04", "Donativos" },
    { "D05", "Intereses reales pagados por créditos hipotecarios" },
    { "D06", "Aportaciones voluntarias al SAR" },
    { "D07", "Primas de seguros de gastos médicos" },
    { "D08", "Gastos de transportación escolar obligatoria" },
    { "D09", "Depósitos en cuentas para el ahorro, primas de pensiones" },
    { "D10", "Pagos por servicios educativos (colegiaturas)" },
    { "S01", "Sin efectos fiscales" },
    { "CP01", "Pagos" },
    { "CN01", "Nómina" }
};
    }
}
