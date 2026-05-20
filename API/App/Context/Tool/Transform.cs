using API.App.DTO;
using API.App.DTO.Currency;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace API.App.Context.Tool
{
    internal class Transform<T>
        where T : struct, IFloatingPoint<T>
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
        internal SearchFilterModel SearchFilter
        {
            get => this.searchFilter;
        }
        #endregion



        #region To
        private async Task<TModel[]> ToModelsAsync<TModel>(Dictionary<byte, T[]> Data,
                                                           Func<DateOnly, T, TModel> ModelFactory)
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

            await Parallel.ForEachAsync<KeyValuePair<byte, T[]>>(
                source: Data,
                parallelOptions: VarGlobal<T>.ParallelForEachOptions,
                body: async (Item, CancellationToken) =>
                {
                    #region Variables
                    var (day, values) = Item;
                    #endregion

                    for (byte month = 1; month <= values.Length; month++)
                    {
                        if (Validate.DateRange(SearchFilter: this.SearchFilter, Day: day, Month: month))
                        {
                            #region Variables
                            T value;
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

        internal async Task<Result<CurrencyDto<T>[]>> ToCurrencyModelsAsync(Dictionary<byte, T[]> CurrencyData)
        {
            #region Collections
            CurrencyDto<T>[] currencies;
            #endregion

            currencies = await this.ToModelsAsync<CurrencyDto<T>>(
                Data: CurrencyData,
                ModelFactory: (Date, Value) => new CurrencyDto<T>
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

            if (currencies.Length == 0)
            {
                return Result<CurrencyDto<T>[]>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Transform<T>),
                        MethodName = nameof(ToCurrencyModelsAsync),
                        VariableName = nameof(currencies.Length),
                        Description = $"La cantidad de datos de la variable {nameof(currencies.Length)} no pude ser 0."
                    }
                );
            }

            return Result<CurrencyDto<T>[]>.Success(Value: currencies);
        }
        #endregion
    }
}