using API.App.DTO;
using API.App.DTO.Currency;
using API.App.DTO.HTML;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace API.App.Context.Tool
{
    public class Value<T>
        where T : struct, IFloatingPoint<T>
    {
        #region Objects
        private CurrencyInfoDto currencyInfo;
        private SearchFilterModel searchFilter;
        #endregion

        #region Tasks
        private Task<string> htmlContentAsync;
        #endregion



        #region Constructor Method
        public Value(CurrencyInfoDto CurrencyInfo,
                     SearchFilterModel SearchFilter,
                     Task<string> HtmlContentAsync)
        {
            #region Objects
            this.currencyInfo = CurrencyInfo;
            this.searchFilter = SearchFilter;
            #endregion

            this.htmlContentAsync = HtmlContentAsync;
        }
        #endregion



        public async Task<Result<CurrencyDto<T>[]>> AnnualAsync()
        {
            #region Collections
            Result<Dictionary<byte, T[]>> result;
            #endregion

            result = await Extract<T>.ValuesAsync(
                Html: new HtmlDto
                {
                    Content = await htmlContentAsync,
                    Table = new TableDto
                    {
                        ID = this.currencyInfo.Table.ID
                    }
                }
            );

            if (!result.IsSuccess)
            {
                return Result<CurrencyDto<T>[]>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Value<T>),
                        MethodName = nameof(AnnualAsync),
                        VariableName = nameof(result.IsSuccess),
                        Description = $"La variable {result.IsSuccess} es {false}",
                        OtherErrors = new[]
                        {
                            result.Error
                        }
                    }
                );
            }

            VarGlobal<T>.Currencies = await new Transform<T>(SearchFilter: this.searchFilter).ToCurrencyModelsAsync(CurrencyData: result.Value);

            VarGlobal<T>.Currencies = VarGlobal<T>.Currencies
                .AsParallel()
                .Where(predicate: Model => !T.IsNaN(value: Model.Currency)
                                           &&
                                           !T.IsZero(value: Model.Currency)
                                           &&
                                           !T.IsInfinity(value: Model.Currency)
                                           &&
                                           !T.IsNegative(value: Model.Currency))
                .OrderBy<CurrencyDto<T>, DateOnly>(keySelector: Model => Model.Date)
                .ToArray<CurrencyDto<T>>();

            return Result<CurrencyDto<T>[]>.Success(Value: VarGlobal<T>.Currencies);
        }

        public async Task<Result<CurrencyDto<T>[]>> MonthlyAsync()
        {
            var result = await this.AnnualAsync();

            if (!result.IsSuccess)
            {
                return Result<CurrencyDto<T>[]>.Failure(
                    new ResultErrorDto()
                    {
                        ClassName = nameof(Value<T>),
                        MethodName = nameof(MonthlyAsync),
                        VariableName = nameof(result.IsSuccess),
                        Description = $"La variable ${nameof(result.IsSuccess)} no puede ser ${false}",
                        OtherErrors = new[]
                        {
                            result.Error
                        }
                    }
                );
            }

             VarGlobal<T>.Currencies = result.Value
                .AsParallel()
                .Where(predicate: Model => Model.Date.Year == this.searchFilter.Year
                                           &&
                                           Model.Date.Month == this.searchFilter.Month)
                .ToArray<CurrencyDto<T>>();

            return Result<CurrencyDto<T>[]>.Success(Value: VarGlobal<T>.Currencies);
        }

        public async Task<Result<CurrencyDto<T>>> DailyAsync(DateOnly Date)
        {
            #region Objects
            CurrencyDto<T>? currency;
            #endregion

            var result = await this.AnnualAsync();

            if (!result.IsSuccess)
            {
                return Result<CurrencyDto<T>>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Value<T>),
                        MethodName = nameof(DailyAsync),
                        VariableName = nameof(result.IsSuccess),
                        Description = $"La variable {nameof(result.IsSuccess)} no puede ser {false}",
                        OtherErrors = new[]
                        {
                            result.Error
                        }
                    }
                );
            }

            // Buscar valor exacto
            currency = result.Value
                        .FirstOrDefault(predicate: model => model.Date == Date)
                    ??
                    // Si no se encuentra, buscar el valor más reciente antes de la fecha (mensual)
                    result.Value
                        .Where(predicate: Model => Model.Date < Date)
                        .OrderByDescending(keySelector: Model => Model.Date)
                        .FirstOrDefault();

            if (currency == null)
            {
                return Result<CurrencyDto<T>>.Failure(
                    new ResultErrorDto()
                    {
                        ClassName = nameof(Value<T>),
                        MethodName = nameof(DailyAsync),
                        VariableName = nameof(currency),
                        Description = $"La variable {nameof(currency)} no puede ser ${null}."
                    }
                );
            }

            return Result<CurrencyDto<T>>.Success(Value: currency);
        }
    }
}