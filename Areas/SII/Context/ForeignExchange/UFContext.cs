using IndicadorChileAPI.Context;
using System;
using System.Threading.Tasks;

namespace IndicadorChileAPI.Areas.SII.Context.ForeignExchange
{
    public class UFContext : ContextBase
    {
        #region ConstructorMethod
        public UFContext(ushort Year,
                         byte? Month)
            : base(Url: $"https://www.sii.cl/valores_y_fechas/uf/uf{Year}.htm",
                   Year: Year,
                   Month: Month)
        {
            
        }
        #endregion



        #region DeconstructorMethod
        ~UFContext()
        {

        }
        #endregion



        public async Task<uint> ConversionInChileanPesosAsync(DateOnly Date, float UF)
        {
            #region Variables
            float Currency;
            double CurrencyConversion;
            uint Pesos;
            #endregion

            Currency = (await this.DailyValueAsync(Date: Date)).Currency;

            CurrencyConversion = await Task.Run<double>(function: () => Math.Truncate(d: UF * Currency));

            Pesos = await Task.Run<uint>(function: () => Convert.ToUInt32(value: CurrencyConversion));

            return Pesos;
        }
    }
}