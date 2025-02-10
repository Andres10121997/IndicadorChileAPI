using IndicadorChileAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndicadorChileAPI.Context.ForeignExchange
{
    public class DolarContext : Context
    {
        #region Arrays
        private DolarModel[] List { get; set; }
        #endregion



        #region ConstructorMethod
        public DolarContext(ushort Year, byte? Month)
            : base(Url: $"https://www.sii.cl/valores_y_fechas/dolar/dolar{Year}.htm",
                   Year: Year,
                   Month: Month)
        {
            this.List = Array.Empty<DolarModel>();
        }
        #endregion



        ~DolarContext()
        {

        }



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

        public async Task<DolarModel[]> MonthlyListOfDollarValuesAsync()
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

        private async Task<DolarModel[]> TransformToDolarModelsAsync(Dictionary<byte, float[]> dolarData)
        {
            return await this.TransformToModelsAsync(Data: dolarData, modelFactory: (date, value) => new DolarModel
            {
                ID = uint.Parse(s: date.ToString("yyyyMMdd")),
                Date = date,
                Dolar = value
            });
        }
    }
}