using IndicadorChileAPI.Context;
using IndicadorChileAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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



        #region Values
        public override async Task<CurrencyModel[]> AnnualValuesAsync()
        {
            try
            {
                this.SetCurrencyList(
                    CurrencyList: (await this.TransformToDolarModelsAsync(
                        dolarData: await this.ExtractValuesAsync(
                            htmlContent: await this.GetHtmlContentAsync(),
                            tableId: "table_export".Trim()
                        )
                    ))
                    .AsParallel<CurrencyModel>()
                    .Where<CurrencyModel>(predicate: x => !float.IsNaN(f: x.Currency) && !float.IsInfinity(f: x.Currency))
                    .OrderBy<CurrencyModel, DateOnly>(keySelector: x => x.Date)
                    .ToArray<CurrencyModel>()
                );

                return this.GetCurrencyList();
            }
            catch (Exception ex)
            {
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());

                throw;
            }
        }

        public override async Task<CurrencyModel[]> MonthlyValuesAsync()
        {
            try
            {
                this.SetCurrencyList(
                    CurrencyList: (await this.AnnualValuesAsync())
                        .AsParallel<CurrencyModel>()
                        .Where<CurrencyModel>(predicate: x => x.Date.Year == this.GetYear() && x.Date.Month == this.GetMonth())
                        .ToArray<CurrencyModel>()
                );

                return this.GetCurrencyList();
            }
            catch (Exception ex)
            {
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());

                throw;
            }
        }

        public override async Task<CurrencyModel> DailyValueAsync(DateOnly Date)
        {
            CurrencyModel? Value;
            
            try
            {
                // Intentar obtener el valor exacto de la fecha solicitada
                Value = (await this.MonthlyValuesAsync())
                    .AsParallel<CurrencyModel>()
                    .Where<CurrencyModel>(predicate: x => x.Date == Date)
                    .FirstOrDefault<CurrencyModel>();

                // Si no hay un valor exacto, retornar el último disponible antes de la fecha
                if (Value is null
                    ||
                    Value == null
                    ||
                    Value.Equals(obj: null))
                {
                    Value = (await this.MonthlyValuesAsync())
                        .AsParallel<CurrencyModel>()
                        .Where<CurrencyModel>(predicate: x => x.Date < Date)
                        .OrderByDescending<CurrencyModel, DateOnly>(keySelector: x => x.Date)
                        .FirstOrDefault<CurrencyModel>();
                }

                // Si aún no hay valores, calcular el promedio o devolver un valor por defecto
                if (Value is null
                    ||
                    Value == null
                    ||
                    Value.Equals(obj: null))
                {
                    Value = new CurrencyModel
                    {
                        ID = 0,
                        Date = Date,
                        Currency = (await this.MonthlyValuesAsync()).Any<CurrencyModel>() ? (await this.MonthlyValuesAsync()).Average<CurrencyModel>(selector: x => x.Currency) : 0
                    };
                }

                return Value;
            }
            catch (Exception ex)
            {
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());

                throw;
            }
        }
        #endregion



        private async Task<CurrencyModel[]> TransformToDolarModelsAsync(Dictionary<byte, float[]> dolarData)
        {
            return await this.TransformToModelsAsync(Data: dolarData, modelFactory: (date, value) => new CurrencyModel
            {
                ID = uint.Parse(s: date.ToString(format: "yyyyMMdd")),
                Date = date,
                Currency = value
            });
        }
    }
}