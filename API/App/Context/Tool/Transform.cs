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
                                                           Func<DateOnly, float, TModel> ModelFactory)
        {
            #region Objects
            object LockObject;
            #endregion

            #region List
            List<TModel> ModelList;
            #endregion

            #region Init
            LockObject = new object();
            ModelList = new List<TModel>();
            #endregion

            await Parallel.ForEachAsync<KeyValuePair<byte, float[]>>(
                source: Data,
                parallelOptions: Utils.ParallelForEachOptions,
                body: async (Item, CancellationToken) =>
                {
                    var (Day, Values) = Item;

                    for (byte month = 1; month <= Values.Length; month++)
                    {
                        if (Day > 0
                            &&
                            Day <= DateTime.DaysInMonth(year: this.Search.Year, month: month))
                        {
                            #region Variables
                            float value;
                            #endregion

                            #region Objects
                            TModel model;
                            #endregion

                            value = Values[month - 1];

                            model = ModelFactory(
                                new DateOnly(
                                    year: this.Search.Year,
                                    month: month,
                                    day: Day
                                ),
                                value
                            );

                            lock (LockObject)
                            {
                                ModelList.Add(item: model);
                            }
                        }
                    }
                }
            );
            
            return ModelList.ToArray<TModel>();
        }

        public async Task<CurrencyModel[]> ToCurrencyModelsAsync(Dictionary<byte, float[]> CurrencyData)
        {
            return await this.ToModelsAsync<CurrencyModel>(
                Data: CurrencyData,
                ModelFactory: (Date, Value) => new CurrencyModel
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