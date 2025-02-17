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
        #region Arrays
        private DolarModel[] List { get; set; }
        #endregion



        #region ConstructorMethod
        public DolarContext(ushort Year,
                            byte? Month)
            : base(Url: $"https://www.sii.cl/valores_y_fechas/dolar/dolar{Year}.htm",
                   Year: Year,
                   Month: Month)
        {
            this.List = Array.Empty<DolarModel>();
        }
        #endregion



        #region DeconstructorMethod
        ~DolarContext()
        {

        }
        #endregion



        #region Values
        public async Task<DolarModel[]> AnnualValuesAsync()
        {
            try
            {
                this.List = (await this.TransformToDolarModelsAsync(
                    dolarData: await this.ExtractValuesAsync(
                        htmlContent: await this.GetHtmlContentAsync(),
                        tableId: "table_export".Trim()
                    )
                ))
                .Where<DolarModel>(predicate: x => !float.IsNaN(f: x.Dolar) && !float.IsInfinity(f: x.Dolar))
                .OrderBy<DolarModel, DateOnly>(keySelector: x => x.Date)
                .ToArray<DolarModel>();

                return this.List;
            }
            catch (Exception ex)
            {
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());

                throw;
            }
        }

        public async Task<DolarModel[]> MonthlyValuesAsync()
        {
            try
            {
                this.List = (await this.AnnualValuesAsync())
                    .Where<DolarModel>(predicate: x => x.Date.Year == this.GetYear() && x.Date.Month == this.GetMonth())
                    .ToArray<DolarModel>();

                return this.List;
            }
            catch (Exception ex)
            {
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());

                throw;
            }
        }

        public async Task<DolarModel> DailyValueAsync(DateOnly Date)
        {
            DolarModel? Value;
            
            try
            {
                // Intentar obtener el valor exacto de la fecha solicitada
                Value = (await this.MonthlyValuesAsync())
                    .Where<DolarModel>(predicate: x => x.Date == Date)
                    .FirstOrDefault<DolarModel>();

                // Si no hay un valor exacto, retornar el último disponible antes de la fecha
                if (Value is null
                    ||
                    Value == null
                    ||
                    Value.Equals(obj: null))
                {
                    Value = (await this.MonthlyValuesAsync())
                        .Where<DolarModel>(predicate: x => x.Date < Date)
                        .OrderByDescending<DolarModel, DateOnly>(keySelector: x => x.Date)
                        .FirstOrDefault<DolarModel>();
                }

                // Si aún no hay valores, calcular el promedio o devolver un valor por defecto
                if (Value is null
                    ||
                    Value == null
                    ||
                    Value.Equals(obj: null))
                {
                    Value = new DolarModel
                    {
                        ID = 0,
                        Date = Date,
                        Dolar = (await this.MonthlyValuesAsync()).Any<DolarModel>() ? (await this.MonthlyValuesAsync()).Average<DolarModel>(selector: x => x.Dolar) : 0
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



        private async Task<DolarModel[]> TransformToDolarModelsAsync(Dictionary<byte, float[]> dolarData)
        {
            return await this.TransformToModelsAsync(Data: dolarData, modelFactory: (date, value) => new DolarModel
            {
                ID = uint.Parse(s: date.ToString(format: "yyyyMMdd")),
                Date = date,
                Dolar = value
            });
        }
    }
}