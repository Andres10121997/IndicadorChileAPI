using IndicadorChileAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndicadorChileAPI.Context.ForeignExchange
{
    public class UFContext : Context
    {
        #region Arrays
        private UFModel[] List { get; set; }
        #endregion



        #region ConstructorMethod
        public UFContext(ushort Year, byte? Month)
            : base(Url: $"https://www.sii.cl/valores_y_fechas/uf/uf{Year}.htm",
                   Year: Year,
                   Month: Month)
        {
            this.List = Array.Empty<UFModel>();
        }
        #endregion



        #region DeconstructorMethod
        ~UFContext()
        {

        }
        #endregion



        public async Task<UFModel[]> AnnualValuesAsync()
        {
            try
            {
                this.List = (await this.TransformToUFModelsAsync(
                    ufData: await this.ExtractValuesAsync(
                        htmlContent: await this.GetHtmlContentAsync(),
                        tableId: "table_export".Trim()
                    )
                ))
                .Where<UFModel>(predicate: x => !float.IsNaN(f: x.UF) && !float.IsInfinity(f: x.UF))
                .OrderBy<UFModel, DateOnly>(keySelector: x => x.Date)
                .ToArray<UFModel>();

                return this.List;
            }
            catch (Exception ex)
            {
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());

                throw;
            }
        }

        public async Task<UFModel[]> MonthlyValuesAsync()
        {
            try
            {
                this.List = (await this.AnnualValuesAsync())
                    .Where<UFModel>(x => x.Date.Year == this.GetYear() && x.Date.Month == this.GetMonth())
                    .ToArray<UFModel>();

                return this.List;
            }
            catch (Exception ex)
            {
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());

                throw;
            }
        }

        public async Task<UFModel> DailyValueAsync(DateOnly Date)
        {
            UFModel Value;

            try
            {
                Value = (await this.MonthlyValuesAsync())
                    .Where<UFModel>(x => x.Date == Date)
                    .Single<UFModel>();

                return Value;
            }
            catch (Exception ex)
            {
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());
                
                throw;
            }
        }

        private async Task<UFModel[]> TransformToUFModelsAsync(Dictionary<byte, float[]> ufData)
        {
            return await this.TransformToModelsAsync(Data: ufData, modelFactory: (date, value) => new UFModel
            {
                ID = uint.Parse(s: date.ToString("yyyyMMdd")),
                Date = date,
                UF = value
            });
        }
    }
}