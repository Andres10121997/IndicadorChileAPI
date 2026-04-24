using API.App.Context;
using API.App.DTO.Currency;
using API.Models;
using System;
using System.Threading.Tasks;

namespace API.App.Information
{
    public class Currency
    {
        #region Objects
        private CurrencyInfoDto currencyInfo;
        private SearchFilterModel searchFilter;
        #endregion



        #region Constructor Method
        public Currency(CurrencyInfoDto CurrencyInfo, SearchFilterModel SearchFilter)
        {
            this.currencyInfo = CurrencyInfo;
            this.searchFilter = SearchFilter;
        }
        #endregion



        public async Task<CurrencyHeaderDto> HeaderAsync()
        {
            #region Objects
            CurrencyHeaderDto currencyHeader;
            #endregion

            VarGlobal.Currencies = await GetAsync();

            currencyHeader = new CurrencyHeaderDto
            {
                ConsultationDateTime = VarGlobal.Now,
                Year = this.searchFilter.Year,
                MonthName = this.MonthName(),
                Currencies = VarGlobal.Currencies
            };

            return currencyHeader;
        }

        private async Task<CurrencyDto[]> GetAsync()
        {
            #region Objects
            ContextBase context;
            #endregion

            context = new ContextBase(
                CurrencyInfo: this.currencyInfo,
                SearchFilter: this.searchFilter
            );

            VarGlobal.Currencies = await context.Values();

            return VarGlobal.Currencies;
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