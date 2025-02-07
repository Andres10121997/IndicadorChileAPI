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



        public UFContext(ushort Year, byte? Month)
            : base(Url: $"https://www.sii.cl/valores_y_fechas/uf/uf{Year}.htm",
                   Year: Year,
                   Month: Month)
        {
            this.List = Array.Empty<UFModel>();
        }



        ~UFContext()
        {

        }



        public async Task<UFModel[]> AnnualUFValuesAsync()
        {
            string htmlContent = string.Empty;

            Dictionary<byte, List<float>> ufValues = new Dictionary<byte, List<float>>();

            try
            {
                // Descargar el HTML de la página
                htmlContent = await this.GetHtmlContentAsync();

                // Extraer los valores de la tabla
                ufValues = await this.ExtractValuesAsync(htmlContent: htmlContent, tableId: "table_export".Trim());

                this.List = await this.TransformToUFModelsAsync(ufData: ufValues);

                this.List = this.List.ToList<UFModel>().Where<UFModel>(predicate: x => !float.IsNaN(f: x.UF) && !float.IsInfinity(f: x.UF)).ToArray<UFModel>();

                Array.Sort(array: this.List, (x, y) => x.Date.CompareTo(value: y.Date));

                return List;
            }
            catch (Exception ex)
            {
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());

                throw;
            }
        }

        public async Task<UFModel[]> MonthlyUFValuesAsync(byte Month)
        {
            UFModel[] NewList = Array.Empty<UFModel>();
            
            try
            {
                this.List = await this.AnnualUFValuesAsync();

                NewList = await Task.Run<UFModel[]>(function: () => List.ToList<UFModel>().Where<UFModel>(predicate: x => x.Date.Year == this.GetYear() && x.Date.Month == Month).ToArray<UFModel>());

                return NewList;
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
                this.List = await this.MonthlyUFValuesAsync(Month: Convert.ToByte(value: Date.Month));

                Value = List.ToList().Where(predicate: x => x.Date == Date).Single();

                return Value;
            }
            catch (Exception ex)
            {
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());

                throw;
            }
        }

        private async Task<UFModel[]> TransformToUFModelsAsync(Dictionary<byte, List<float>> ufData)
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