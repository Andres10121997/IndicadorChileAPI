using IndicadorChileAPI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace IndicadorChileAPI.Context.Tool
{
    public class Transform
    {
        #region Variables
        private ushort Year;
        #endregion



        #region Constructor Method
        public Transform(ushort Year)
            : base()
        {
            this.Year = Year;
        }
        #endregion



        #region To
        protected TModel[] ToModels<TModel>(Dictionary<byte, float[]> Data,
                                            Func<DateOnly, float, TModel> modelFactory)
        {
            #region List
            List<TModel> ModelList = new List<TModel>();
            #endregion

            foreach (var (day, values) in Data)
            {
                for (byte month = 1; month <= values.Length; month++)
                {
                    if (day > 0 && day <= DateTime.DaysInMonth(year: this.Year, month: month))
                    {
                        #region Variables
                        float value;
                        #endregion

                        #region Objects
                        TModel model;
                        #endregion

                        value = values[month - 1];

                        model = modelFactory(
                            new DateOnly(
                                year: this.Year,
                                month: month,
                                day: day
                            ),
                            value
                        );

                        ModelList.Add(item: model);
                    }
                }
            }

            return ModelList.ToArray<TModel>();
        }

        protected async Task<TModel[]> ToModelsAsync<TModel>(Dictionary<byte, float[]> Data,
                                                             Func<DateOnly, float, TModel> modelFactory)
        {
            return await Task.Run<TModel[]>(
                function: () => this.ToModels<TModel>(Data: Data, modelFactory)
            );
        }

        public async Task<CurrencyModel[]> ToCurrencyModelsAsync(Dictionary<byte, float[]> CurrencyData)
        {
            return await this.ToModelsAsync<CurrencyModel>(Data: CurrencyData, modelFactory: (Date, Value) => new CurrencyModel
            {
                // https://www.youtube.com/shorts/UwcOL3ZL3go
                ID = Guid.CreateVersion7(
                    timestamp: new DateTimeOffset(
                        year: Date.Year,
                        month: Date.Month,
                        day: Date.Day,
                        hour: 0,
                        minute: 0,
                        second: 0,
                        offset: TimeSpan.Zero
                    )
                ),
                Date = Date,
                WeekdayName = Date.ToString(format: "dddd", provider: CultureInfo.CreateSpecificCulture(name: "es")),
                Currency = Value
            });
        }
        #endregion
    }
}