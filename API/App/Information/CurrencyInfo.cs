using API.App.Context;
using API.App.Record.Currency;
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



        public static async Task<CurrencyListHeaderRecord> CurrencyHeaderAsync(CurrencyInfoRecord CurrencyInfo,
                                                                               SearchFilterModel SearchFilter)
        {
            #region Variables
            string? monthName;
            DateTime now;
            #endregion

            #region Collections
            CurrencyRecord[] currencies;
            #endregion

            #region Objects
            CurrencyListHeaderRecord currencyList;
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

            currencies = await GetCurrenciesAsync(
                SearchFilter: SearchFilter,
                CurrencyInfo: CurrencyInfo
            );

            currencyList = new CurrencyListHeaderRecord()
            {
                ConsultationDate = DateOnly.FromDateTime(dateTime: now),
                ConsultationTime = TimeOnly.FromDateTime(dateTime: now),
                Year = SearchFilter.Year,
                MonthName = monthName,
                Currencies = currencies
            };

            return currencyList;
        }

        public static async Task<CurrencyRecord[]> GetCurrenciesAsync(CurrencyInfoRecord CurrencyInfo,
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
            context.CurrencyList = await (SearchFilter.Month.HasValue ? context.MonthlyValuesAsync() : context.AnnualValuesAsync());

            return context.CurrencyList;
        }
    }
}