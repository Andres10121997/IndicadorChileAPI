using IndicadorChileAPI.App.DTO;
using IndicadorChileAPI.App.DTO.Currency;
using IndicadorChileAPI.Models;
using IndicadorChileAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace IndicadorChileAPI.App.Context.Tool
{
    internal class Transform<TCurrency>
        where TCurrency : struct, IFloatingPointConstants<TCurrency>
    {
        #region Object
        private SearchFilterModel searchFilter;
        #endregion



        #region Constructor Method
        public Transform(SearchFilterModel SearchFilter)
        {
            #region Object
            this.searchFilter = SearchFilter;
            #endregion
        }
        #endregion



        #region Field
        internal SearchFilterModel SearchFilter
        {
            get => this.searchFilter;
        }
        #endregion



        #region To
        private async Task<TModel[]> ToModelsAsync<TModel>(Dictionary<byte, TCurrency[]> Data,
                                                           Func<DateOnly, TCurrency, TModel> ModelFactory)
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

            await Parallel.ForEachAsync<KeyValuePair<byte, TCurrency[]>>(
                source: Data,
                parallelOptions: VarGlobal.ParallelForEachOptions,
                body: async (Item, CancellationToken) =>
                {
                    #region Variables
                    var (day, values) = Item;
                    #endregion

                    for (byte month = 1; month <= values.Length; month++)
                    {
                        #region Objects
                        Result<bool, ResultErrorDto> dateRangeResult;
                        #endregion

                        #region Init
                        dateRangeResult = Validate.Range.Date(
                            Day: day,
                            Month: month,
                            SearchFilter: this.SearchFilter
                        );
                        #endregion

                        if (dateRangeResult.IsSuccess)
                        {
                            #region Variables
                            TCurrency value;
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

        internal async Task<Result<CurrencyDto<TCurrency>[], ResultErrorDto>> ToCurrencyModelsAsync(Dictionary<byte, TCurrency[]> CurrencyData)
        {
            #region Collections
            CurrencyDto<TCurrency>[] currencies;
            #endregion

            currencies = await this.ToModelsAsync<CurrencyDto<TCurrency>>(
                Data: CurrencyData,
                ModelFactory: (Date, Value) => new CurrencyDto<TCurrency>
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

            if (currencies == null)
            {
                return Result<CurrencyDto<TCurrency>[], ResultErrorDto>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Transform<TCurrency>),
                        MethodName = nameof(ToCurrencyModelsAsync),
                        VariableName = nameof(currencies),
                        Description = $"La variable {nameof(currencies)} no puede ser {null}."
                    }
                );
            }

            if (currencies.Length.Equals(obj: 0))
            {
                return Result<CurrencyDto<TCurrency>[], ResultErrorDto>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Transform<TCurrency>),
                        MethodName = nameof(ToCurrencyModelsAsync),
                        VariableName = nameof(currencies.Length),
                        Description = $"La cantidad de datos de la variable {nameof(currencies.Length)} no pude ser 0."
                    }
                );
            }

            return Result<CurrencyDto<TCurrency>[], ResultErrorDto>.Success(Value: currencies);
        }
        #endregion
    }
}