using IndicadorChileAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndicadorChileAPI.Context.ForeignExchange
{
    public class UFContext : Context
    {
        public UFContext(ushort Year)
            : base(Url: $"https://www.sii.cl/valores_y_fechas/uf/uf{Year}.htm",
                   Year: Year)
        {

        }



        ~UFContext()
        {

        }



        public async Task<UFModel[]> AnnualUFValuesAsync()
        {
            string htmlContent = string.Empty;

            Dictionary<byte, List<float>> ufValues = new Dictionary<byte, List<float>>();

            UFModel[] ListOfUF = Array.Empty<UFModel>();

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

                ListOfUF = await this.TransformToUFModelsAsync(ufData: ufValues);

                ListOfUF = ListOfUF.ToList<UFModel>().Where<UFModel>(predicate: x => !float.IsNaN(f: x.UF)).ToArray<UFModel>();

                Array.Sort(array: ListOfUF, (x, y) => x.Date.CompareTo(value: y.Date));

                return ListOfUF;
            }
            catch (Exception ex)
            {
                await Utils.ErrorMessageAsync(ex: ex, OType: this.GetType());

                throw;
            }
        }

        public async Task<UFModel[]> MonthlyUFValuesAsync(byte Month)
        {
            UFModel[] List = Array.Empty<UFModel>();
            UFModel[] NewList = Array.Empty<UFModel>();
            
            try
            {
                List = await this.AnnualUFValuesAsync();

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
            try
            {
                var List = await this.MonthlyUFValuesAsync(Month: Convert.ToByte(Date.Month));

                var Value = List.ToList().Where(x => x.Date == Date).Single();

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
                ID = uint.Parse(date.ToString("yyyyMMdd")),
                Date = date,
                UF = value
            });
        }
    }
}