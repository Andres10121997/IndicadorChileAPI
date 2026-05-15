using API.App.Context;
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




        public async Task<CurrencyHeaderDto<T>> HeaderAsync()
        {
            #region Objects
            Result<bool, string> validationResult = this.Validation();
            CurrencyHeaderDto<T> currencyHeader;
            #endregion

            if (!validationResult.IsSuccess)
            {
                return default!;
            }

            VarGlobal<T>.Currencies = await GetAsync();

            currencyHeader = new CurrencyHeaderDto<T>
            {
                ConsultationDateTime = DateTime.Now,
                Year = this.searchFilter.Year,
                MonthName = this.MonthName(),
                Currencies = VarGlobal<T>.Currencies
            };

            return currencyHeader;
        }

        private Result<bool, string> Validation()
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
                return Result<bool, string>.Failure(Error: $"La variable '{nameof(this.searchFilter.Year)}' está fuera de rango.");
            }

            return Result<bool, string>.Success(Value: validation);
        }

        private async Task<CurrencyDto<T>[]> GetAsync()
        {
            #region Objects
            ContextBase<T> context;
            #endregion

            context = new ContextBase<T>(
                CurrencyInfo: this.currencyInfo,
                SearchFilter: this.searchFilter
            );

            VarGlobal<T>.Currencies = await context.Values();

            return VarGlobal<T>.Currencies;
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