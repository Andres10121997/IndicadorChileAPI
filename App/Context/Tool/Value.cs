using IndicadorChileAPI.App.DTO;
using IndicadorChileAPI.App.DTO.Currency;
using IndicadorChileAPI.App.DTO.HTML;
using IndicadorChileAPI.Models;
using IndicadorChileAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace IndicadorChileAPI.App.Context.Tool
{
    internal static class Value<TCurrency>
        where TCurrency : struct, IFloatingPointConstants<TCurrency>
    {
        internal static async Task<Result<CurrencyDTO<TCurrency>[], ResultErrorDto>> CurrenciesAsync(string htmlContent,
                                                                                                     CurrencyInfoDTO CurrencyInfo,
                                                                                                     SearchFilterModel SearchFilter)
        {
            #region Objects
            Result<CurrencyDTO<TCurrency>[], ResultErrorDto> currenciesResult;
            Result<Dictionary<byte, TCurrency[]>, ResultErrorDto> valuesResult;
            #endregion

            valuesResult = await Extract<TCurrency>.ValuesAsync(
                Html: new HtmlDto
                {
                    Content = htmlContent,
                    Table = new TableDto
                    {
                        ID = CurrencyInfo.Table.ID
                    }
                }
            );

            if (!valuesResult.IsSuccess)
            {
                return Result<CurrencyDTO<TCurrency>[], ResultErrorDto>.Failure(
                    Error: new ResultErrorDto
                    {
                        ClassName = nameof(Value<TCurrency>),
                        MethodName = nameof(CurrenciesAsync),
                        VariableName = nameof(valuesResult.IsSuccess),
                        Description = $"La variable {valuesResult.IsSuccess} es {false}."
                    }
                );
            }

            if (valuesResult.Value == null)
            {
                return Result<CurrencyDTO<TCurrency>[], ResultErrorDto>.Failure(
                    Error: new ResultErrorDto
                    {
                        ClassName = nameof(Value<TCurrency>),
                        MethodName = nameof(CurrenciesAsync),
                        VariableName = nameof(valuesResult.Value),
                        Description = $"La variable {valuesResult.Value} es {null}."
                    }
                );
            }

            currenciesResult = await new Transform<TCurrency>(SearchFilter: SearchFilter).ToCurrencyModelsAsync(CurrencyData: valuesResult.Value);

            if (!currenciesResult.IsSuccess)
            {
                return Result<CurrencyDTO<TCurrency>[], ResultErrorDto>.Failure(
                    Error: new ResultErrorDto
                    {
                        ClassName = nameof(Value<TCurrency>),
                        MethodName = nameof(CurrenciesAsync),
                        VariableName = nameof(currenciesResult.IsSuccess),
                        Description = $"La variable {currenciesResult.IsSuccess} es {false}."
                    }
                );
            }

            if (currenciesResult.Value == null)
            {
                return Result<CurrencyDTO<TCurrency>[], ResultErrorDto>.Failure(
                    Error: new ResultErrorDto
                    {
                        ClassName = nameof(Value<TCurrency>),
                        MethodName = nameof(CurrenciesAsync),
                        VariableName = nameof(currenciesResult.Value),
                        Description = $"La variable {currenciesResult.Value} es {null}."
                    }
                );
            }

            return Result<CurrencyDTO<TCurrency>[], ResultErrorDto>.Success(
                Value: currenciesResult.Value
                    .AsParallel()
                    .Where(predicate: Model => !TCurrency.IsNaN(value: Model.Currency)
                                               &&
                                               !TCurrency.IsZero(value: Model.Currency)
                                               &&
                                               !TCurrency.IsInfinity(value: Model.Currency)
                                               &&
                                               !TCurrency.IsNegative(value: Model.Currency))
                    .OrderBy<CurrencyDTO<TCurrency>, DateOnly>(keySelector: Model => Model.Date)
                    .ToArray<CurrencyDTO<TCurrency>>()
            );
        }
    }
}