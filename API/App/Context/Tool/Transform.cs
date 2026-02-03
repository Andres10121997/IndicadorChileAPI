using API.Models;
using API.Models.Get;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.App.Context.Tool
{
    public class Transform
    {
        #region Object
        private SearchFilterModel search;
        #endregion



        #region Constructor Method
        public Transform(SearchFilterModel Search)
        {
            this.search = Search;
        }
        #endregion



        #region Property
        public SearchFilterModel Search
        {
            get => this.search;
        }
        #endregion



        #region To
        private async Task<TModel[]> ToModelsAsync<TModel>(Dictionary<byte, float[]> Data,
                                                           Func<DateOnly, float, TModel> modelFactory)
        {
            #region Objects
            var lockObject = new object();
            #endregion

            #region List
            var ModelList = new List<TModel>();
            #endregion

            await Parallel.ForEachAsync(source: Data, body: async (item, cancellationToken) =>
            {
                var (day, values) = item;

                for (byte month = 1; month <= values.Length; month++)
                {
                    if (day > 0 && day <= DateTime.DaysInMonth(year: this.Search.Year, month: month))
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
                                year: this.Search.Year,
                                month: month,
                                day: day
                            ),
                            value
                        );

                        lock (lockObject)
                        {
                            ModelList.Add(item: model);
                        }
                    }
                }
            });
            
            return ModelList.ToArray<TModel>();
        }

        public async Task<CurrencyModel[]> ToCurrencyModelsAsync(Dictionary<byte, float[]> CurrencyData)
        {
            return await this.ToModelsAsync<CurrencyModel>(
                Data: CurrencyData,
                modelFactory: (Date, Value) => new CurrencyModel
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
                    WeekdayName = Date.ToString(format: "dddd"),
                    Currency = Value
                }
            );
        }
        #endregion
    }
}