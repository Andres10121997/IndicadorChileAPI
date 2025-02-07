using IndicadorChileAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndicadorChileAPI.Context.ForeignExchange
{
    public class DolarContext : Context
    {
        private DolarModel[] List { get; set; }



        public DolarContext(ushort Year)
            : base(Url: $"https://www.sii.cl/valores_y_fechas/dolar/dolar{Year}.htm",
                   Year: Year)
        {
            this.List = Array.Empty<DolarModel>();
        }



        ~DolarContext()
        {

        }



        public async Task<DolarModel[]> AnnualListOfDollarValuesAsync()
        {
            string htmlContent = string.Empty;

            Dictionary<byte, List<float>> ufValues = new Dictionary<byte, List<float>>();

            try
            {
                // Descargar el HTML de la página
                htmlContent = await this.GetHtmlContentAsync();

                // Extraer los valores de la tabla
                ufValues = await this.ExtractValuesAsync(htmlContent: htmlContent, tableId: "table_export".Trim());

                this.List = await this.TransformToDolarModelsAsync(ufData: ufValues);

                this.List = this.List.ToList<DolarModel>().Where<DolarModel>(predicate: x => !float.IsNaN(f: x.Dolar)).ToArray<DolarModel>();

                Array.Sort(array: this.List, (x, y) => x.Date.CompareTo(value: y.Date));

                return this.List;
            }
            catch (Exception ex)
            {
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());

                throw;
            }
        }

        public async Task<DolarModel[]> MonthlyListOfDollarValuesAsync(byte Month)
        {
            DolarModel[] NewList = Array.Empty<DolarModel>();
            
            try
            {
                this.List = await this.AnnualListOfDollarValuesAsync();

                NewList = await Task.Run<DolarModel[]>(function: () => this.List.ToList().Where<DolarModel>(predicate: x => x.Date.Year == this.GetYear() && x.Date.Month == Month).ToArray());

                return NewList;
            }
            catch (Exception ex)
            {
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());

                throw;
            }
        }

        private async Task<DolarModel[]> TransformToDolarModelsAsync(Dictionary<byte, List<float>> ufData)
        {
            return await this.TransformToModelsAsync(Data: ufData, modelFactory: (date, value) => new DolarModel
            {
                ID = uint.Parse(s: date.ToString("yyyyMMdd")),
                Date = date,
                Dolar = value
            });
        }
    }
}