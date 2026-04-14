using API.App.Context;
using API.App.DTO.Currency;
using API.Models;
using System;
using System.Threading.Tasks;

namespace API.App.Information
{
    public static class CurrencyInfo
    {
        #region Constructor Method
        static CurrencyInfo()
        {
            
        }
        #endregion



        public static async Task<CurrencyListHeaderDto> CurrencyHeaderAsync(CurrencyInfoDto CurrencyInfo,
                                                                            SearchFilterModel SearchFilter)
        {
            #region Variables
            string? monthName;
            DateTime now;
            #endregion

            #region Objects
            CurrencyListHeaderDto currencyList;
            #endregion

            monthName = SearchFilter.Month.HasValue
                        ?
                        new DateOnly(
                            year: SearchFilter.Year,
                            month: Convert.ToInt32(value: SearchFilter.Month),
                            day: 1
                        ).ToString(format: "MMMM")
                        :
                        null;

            now = DateTime.Now;

            VarGlobal.Currencies = await GetCurrenciesAsync(
                SearchFilter: SearchFilter,
                CurrencyInfo: CurrencyInfo
            );

            currencyList = new CurrencyListHeaderDto()
            {
                ConsultationDate = DateOnly.FromDateTime(dateTime: now),
                ConsultationTime = TimeOnly.FromDateTime(dateTime: now),
                Year = SearchFilter.Year,
                MonthName = monthName,
                Currencies = VarGlobal.Currencies
            };

            return currencyList;
        }

        public static async Task<CurrencyDto[]> GetCurrenciesAsync(CurrencyInfoDto CurrencyInfo,
                                                                   SearchFilterModel SearchFilter)
        {
            #region Objects
            ContextBase context;
            #endregion

            context = new ContextBase(
                CurrencyInfo: CurrencyInfo,
                SearchFilter: SearchFilter
            );

            // Ternaria para obtener datos.
            VarGlobal.Currencies = await (SearchFilter.Month.HasValue ? context.MonthlyValuesAsync() : context.AnnualValuesAsync());

            return VarGlobal.Currencies;
        }
    }
}