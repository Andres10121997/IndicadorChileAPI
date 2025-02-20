using IndicadorChileAPI.Context;
using IndicadorChileAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndicadorChileAPI.Areas.SII.Context.ForeignExchange
{
    public class UFContext : ContextBase
    {
        #region Arrays
        private CurrencyModel[] List { get; set; }
        #endregion



        #region ConstructorMethod
        public UFContext(ushort Year,
                         byte? Month)
            : base(Url: $"https://www.sii.cl/valores_y_fechas/uf/uf{Year}.htm",
                   Year: Year,
                   Month: Month)
        {
            this.List = Array.Empty<CurrencyModel>();
        }
        #endregion



        #region DeconstructorMethod
        ~UFContext()
        {

        }
        #endregion



        #region Values
        public override async Task<CurrencyModel[]> AnnualValuesAsync()
        {
            try
            {
                this.List = (await this.TransformToUFModelsAsync(
                    ufData: await this.ExtractValuesAsync(
                        htmlContent: await this.GetHtmlContentAsync(),
                        tableId: "table_export".Trim()
                    )
                ))
                .Where<CurrencyModel>(predicate: x => !float.IsNaN(f: x.Currency) && !float.IsInfinity(f: x.Currency))
                .OrderBy<CurrencyModel, DateOnly>(keySelector: x => x.Date)
                .ToArray<CurrencyModel>();

                return this.List;
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
                this.List = (await this.AnnualValuesAsync())
                    .Where<CurrencyModel>(x => x.Date.Year == this.GetYear() && x.Date.Month == this.GetMonth())
                    .ToArray<CurrencyModel>();

                return this.List;
            }
            catch (Exception ex)
            {
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());

                throw;
            }
        }
        
        public async Task<CurrencyModel> DailyValueAsync(DateOnly Date)
        {
            CurrencyModel? Value;

            try
            {
                // Intentar obtener el valor exacto de la fecha solicitada
                Value = (await this.MonthlyValuesAsync())
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



        private async Task<CurrencyModel[]> TransformToUFModelsAsync(Dictionary<byte, float[]> ufData)
        {
            return await this.TransformToModelsAsync(Data: ufData, modelFactory: (date, value) => new CurrencyModel
            {
                ID = uint.Parse(s: date.ToString(format: "yyyyMMdd")),
                Date = date,
                Currency = value
            });
        }
    }
}