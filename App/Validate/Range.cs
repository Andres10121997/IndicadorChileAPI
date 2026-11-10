using IndicadorChileAPI.App.DTO;
using IndicadorChileAPI.App.DTO.Currency;
using IndicadorChileAPI.Models;
using IndicadorChileAPI.Utils;
using System;
using System.Linq;

namespace IndicadorChileAPI.App.Validate
{
    internal static class Range
    {
        #region Variables
        private static bool all;
        #endregion

        #region Collections
        private static bool[] range;
        #endregion



        #region Constructor Method
        static Range()
        {
            range = Array.Empty<bool>();
        }
        #endregion



        internal static Result<bool, ResultErrorDto> Date(byte Day,
                                                          byte Month,
                                                          SearchFilterModel SearchFilter)
        {
            range = new bool[2]
            {
                Day > 0,
                Day <= DateTime.DaysInMonth(
                    year: SearchFilter.Year,
                    month: Month
                )
            };

            all = range.All(predicate: value => value == true);

            if (!all)
            {
                return Result<bool, ResultErrorDto>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Range),
                        MethodName = nameof(Date),
                        VariableName = nameof(all),
                        Description = $"La variable {nameof(all)} no puede ser {false}."
                    }
                );
            }

            return Result<bool, ResultErrorDto>.Success(Value: true);
        }

        internal static Result<bool, ResultErrorDto> Year(CurrencyInfoDto CurrencyInfo,
                                                          SearchFilterModel SearchFilter)
        {
            range = new bool[2]
            {
                SearchFilter.Year >= CurrencyInfo.StartDate.Year,
                SearchFilter.Year <= CurrencyInfo.EndDate.Year
            };

            all = range.All(predicate: value => value == true);

            if (!all)
            {
                return Result<bool, ResultErrorDto>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Range),
                        MethodName = nameof(Year),
                        VariableName = nameof(all),
                        Description = $"La variable {nameof(all)} no puede ser {false}."
                    }
                );
            }

            return Result<bool, ResultErrorDto>.Success(Value: true);
        }
    }
}