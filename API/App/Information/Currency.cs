using API.App.Context;
using API.App.DTO;
using API.App.DTO.Currency;
using API.Models;
using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace API.App.Information
{
    public class Currency<T>
        where T : struct, IFloatingPoint<T>
    {
        #region Objects
        private CurrencyInfoDto currencyInfo;
        private SearchFilterModel searchFilter;
        #endregion



        #region Constructor Method
        public Currency(CurrencyInfoDto CurrencyInfo,
                        SearchFilterModel SearchFilter)
        {
            this.currencyInfo = CurrencyInfo;
            this.searchFilter = SearchFilter;
        }
        #endregion




        public async Task<Result<CurrencyHeaderDto<T>>> HeaderAsync()
        {
            #region Variables
            bool validation;
            #endregion

            #region Objects
            Result<CurrencyDto<T>[]> getResult;
            CurrencyHeaderDto<T> currencyHeader;
            #endregion

            validation = Validate.YearRange(SearchFilter: this.searchFilter, CurrencyInfo: this.currencyInfo);

            if (!validation)
            {
                return Result<CurrencyHeaderDto<T>>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Currency<T>),
                        MethodName = nameof(HeaderAsync),
                        VariableName = nameof(validation),
                        Description = $"La variable {nameof(validation)} no puede ser {false}"
                    }
                );
            }

            getResult = await GetAsync();

            if (!getResult.IsSuccess)
            {
                return Result<CurrencyHeaderDto<T>>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Currency<T>),
                        MethodName = nameof(GetAsync),
                        VariableName = nameof(getResult.IsSuccess),
                        Description = $"La variable {nameof(getResult.IsSuccess)} no puede ser {false}",
                        OtherErrors = new[]
                        {
                            getResult.Error
                        }
                    }
                );
            }

            currencyHeader = new CurrencyHeaderDto<T>
            {
                ConsultationDateTime = DateTime.Now,
                Year = this.searchFilter.Year,
                MonthName = this.MonthName(),
                Currencies = VarGlobal<T>.Currencies
            };

            return Result<CurrencyHeaderDto<T>>.Success(currencyHeader);
        }

        private async Task<Result<CurrencyDto<T>[]>> GetAsync()
        {
            #region Objects
            ContextBase<T> context;
            Result<CurrencyDto<T>[]> result;
            #endregion

            context = new ContextBase<T>(
                CurrencyInfo: this.currencyInfo,
                SearchFilter: this.searchFilter
            );

            result = await context.Values();

            if (!result.IsSuccess)
            {
                return Result<CurrencyDto<T>[]>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Currency<T>),
                        MethodName = nameof(GetAsync),
                        VariableName = nameof(result.IsSuccess),
                        Description = $"La variable {nameof(result.IsSuccess)} no puede ser {false}",
                        OtherErrors = new[]
                        {
                            result.Error
                        }
                    }
                );
            }

            return Result<CurrencyDto<T>[]>.Success(Value: result.Value);
        }

        private string? MonthName()
        {
            switch (this.searchFilter.Month.HasValue)
            {
                case true:
                    return new DateOnly(
                        year: this.searchFilter.Year,
                        month: Convert.ToInt32(value: this.searchFilter.Month),
                        day: 1
                    ).ToString(format: "MMMM");
                case false:
                    return null;
            }
        }
    }
}