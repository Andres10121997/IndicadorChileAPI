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
            #region Objects
            Result<bool> validationResult;
            Result<CurrencyDto<T>[]> getResult;
            CurrencyHeaderDto<T> currencyHeader;
            #endregion

            validationResult = this.Validation();

            if (!validationResult.IsSuccess)
            {
                return Result<CurrencyHeaderDto<T>>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Currency<T>),
                        MethodName = nameof(HeaderAsync),
                        VariableName = nameof(validationResult.IsSuccess),
                        Description = $"La variable {nameof(validationResult.IsSuccess)} no puede ser {false}",
                        OtherErrors = new[]
                        {
                            validationResult.Error
                        }
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

        private Result<bool> Validation()
        {
            #region Variables
            bool validation;
            #endregion

            #region Collections
            bool[] currencyValidation;
            #endregion

            currencyValidation = new bool[2]
            {
                this.searchFilter.Year >= this.currencyInfo.StartDate.Year,
                this.searchFilter.Year <= this.currencyInfo.EndDate.Year
            };

            validation = currencyValidation.All(value => value == true);

            if (validation == false)
            {
                return Result<bool>.Failure(
                    new ResultErrorDto()
                    {
                        ClassName = nameof(Currency<T>),
                        MethodName = nameof(Validation),
                        VariableName = nameof(this.searchFilter.Year),
                        Description = $"La variable '{nameof(this.searchFilter.Year)}' está fuera de rango."
                    }
                );
            }

            return Result<bool>.Success(Value: validation);
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