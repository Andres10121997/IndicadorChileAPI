using IndicadorChileAPI.Context;
using System;
using System.Threading.Tasks;

namespace IndicadorChileAPI.Areas.SII.Context.ForeignExchange
{
    public class DolarContext : ContextBase
    {
        #region ConstructorMethod
        public DolarContext(ushort Year,
                            byte? Month)
            : base(Url: $"https://www.sii.cl/valores_y_fechas/dolar/dolar{Year}.htm",
                   Year: Year,
                   Month: Month)
        {
            
        }
        #endregion



        #region DeconstructorMethod
        ~DolarContext()
        {

        }
        #endregion



        public async Task<uint> ConversionInChileanPesosAsync(DateOnly Date, float Dolar)
        {
            #region Variables
            float Currency;
            double CurrencyConversion;
            uint Pesos;
            #endregion

            Currency = (await this.DailyValueAsync(Date: Date)).Currency;

            CurrencyConversion = await Task.Run<double>(function: () => Math.Truncate(d: Dolar * Currency));

            Pesos = await Task.Run<uint>(function: () => Convert.ToUInt32(value: CurrencyConversion));

            return Pesos;
        }
    }
}