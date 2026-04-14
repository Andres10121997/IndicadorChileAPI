using API.App.Record.Currency;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.App.Context.Tool
{
    public class Transform
    {
        #region Object
        private SearchFilterModel searchFilter;
        #endregion



        #region Constructor Method
        public Transform(SearchFilterModel SearchFilter)
        {
            this.searchFilter = SearchFilter;
        }
        #endregion



        #region Field
        public SearchFilterModel SearchFilter
        {
            get => this.searchFilter;
        }
        #endregion



        #region To
        private async Task<TModel[]> ToModelsAsync<TModel>(Dictionary<byte, float[]> Data,
                                                           Func<DateOnly, float, TModel> ModelFactory)
        {
            #region Objects
            object lockObject;
            #endregion

            #region Collections
            List<TModel> modelList;
            #endregion

            #region Init
            lockObject = new object();
            modelList = new List<TModel>();
            #endregion

            await Parallel.ForEachAsync<KeyValuePair<byte, float[]>>(
                source: Data,
                parallelOptions: Utils.ParallelForEachOptions,
                body: async (Item, CancellationToken) =>
                {
                    #region Variables
                    var (day, values) = Item;
                    #endregion

                    #region Collections
                    bool[] dateRange;
                    #endregion

                    for (byte month = 1; month <= values.Length; month++)
                    {
                        dateRange = new bool[2]
                        {
                            day > 0,
                            day <= DateTime.DaysInMonth(year: this.SearchFilter.Year, month: month)
                        };

                        if (dateRange.All(predicate: value => value == true))
                        {
                            #region Variables
                            float value;
                            #endregion

                            #region Objects
                            TModel model;
                            #endregion

                            value = values[month - 1];

                            model = ModelFactory(
                                new DateOnly(
                                    year: this.SearchFilter.Year,
                                    month: month,
                                    day: day
                                ),
                                value
                            );

                            lock (lockObject)
                            {
                                modelList.Add(item: model);
                            }
                        }
                    }
                }
            );
            
            return modelList.ToArray<TModel>();
        }

        public async Task<CurrencyDto[]> ToCurrencyModelsAsync(Dictionary<byte, float[]> CurrencyData)
        {
            return await this.ToModelsAsync<CurrencyDto>(
                Data: CurrencyData,
                ModelFactory: (Date, Value) => new CurrencyDto
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
                            millisecond: 0,
                            microsecond: 0,
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