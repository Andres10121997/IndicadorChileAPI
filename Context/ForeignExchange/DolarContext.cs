using IndicadorChileAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndicadorChileAPI.Context.ForeignExchange
{
    public class DolarContext : Context
    {
        public DolarContext(ushort Year)
            : base(Url: $"https://www.sii.cl/valores_y_fechas/dolar/dolar{Year}.htm",
                   Year: Year)
        {
            
        }



        ~DolarContext()
        {
        }



        public async Task<DolarModel[]> AnnualListOfDollarValuesAsync()
        {
            string htmlContent = string.Empty;

            Dictionary<byte, List<float>> ufValues = new Dictionary<byte, List<float>>();

            DolarModel[] ListOfDolar = Array.Empty<DolarModel>();

            try
            {
                // Descargar el HTML de la página
                htmlContent = await this.GetHtmlContentAsync();

                // Extraer los valores de la tabla
                ufValues = await this.ExtractValuesAsync(htmlContent, "table_export".Trim());

                // Imprimir los valores obtenidos
                /*
                foreach (KeyValuePair<byte, List<float>> uf in ufValues)
                {
                    await Utils.OutMessageAsync(
                        Message: $"Día: {uf.Key}, Valores: {string.Join(separator: " | ", values: uf.Value)}",
                        OType: this.GetType()
                    );
                }
                */

                ListOfDolar = await this.TransformToDolarModelsAsync(ufData: ufValues);

                ListOfDolar = ListOfDolar.ToList<DolarModel>().Where<DolarModel>(predicate: x => !float.IsNaN(f: x.Dolar)).ToArray<DolarModel>();

                Array.Sort(array: ListOfDolar, (x, y) => x.Date.CompareTo(value: y.Date));

                return ListOfDolar;
            }
            catch (Exception ex)
            {
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());

                throw;
            }
        }

        public async Task<DolarModel[]> MonthlyListOfDollarValuesAsync(byte Month)
        {
            DolarModel[] List = Array.Empty<DolarModel>();
            DolarModel[] NewList = Array.Empty<DolarModel>();
            
            try
            {
                List = await this.AnnualListOfDollarValuesAsync();

                NewList = await Task.Run<DolarModel[]>(function: () => List.ToList().Where<DolarModel>(predicate: x => x.Date.Year == this.GetYear() && x.Date.Month == Month).ToArray());

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
                ID = uint.Parse(date.ToString("yyyyMMdd")),
                Date = date,
                Dolar = value
            });
        }
    }
}