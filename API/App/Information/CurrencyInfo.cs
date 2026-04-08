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
            string? MonthName;
            DateTime Now;
            #endregion

            #region Collections
            CurrencyRecord[] Currencies;
            #endregion

            #region Objects
            CurrencyListHeaderRecord CurrencyList;
            #endregion

            MonthName = SearchFilter.Month.HasValue
                        ?
                        new DateOnly(
                            year: SearchFilter.Year,
                            month: Convert.ToInt32(value: SearchFilter.Month),
                            day: 1
                        ).ToString(format: "MMMM")
                        :
                        null;
            
            Now = DateTime.Now;

            Currencies = await GetCurrenciesAsync(
                SearchFilter: SearchFilter,
                CurrencyInfo: CurrencyInfo
            );

            CurrencyList = new CurrencyListHeaderRecord()
            {
                ConsultationDate = DateOnly.FromDateTime(dateTime: Now),
                ConsultationTime = TimeOnly.FromDateTime(dateTime: Now),
                Year = SearchFilter.Year,
                MonthName = MonthName,
                Currencies = Currencies
            };

            return CurrencyList;
        }

        public static async Task<CurrencyRecord[]> GetCurrenciesAsync(CurrencyInfoRecord CurrencyInfo,
                                                                      SearchFilterModel SearchFilter)
        {
            #region Objects
            ContextBase Context;
            #endregion

            Context = new ContextBase(
                CurrencyInfo: CurrencyInfo,
                SearchFilter: SearchFilter
            );

            // Ternaria para obtener datos.
            Context.CurrencyList = await (SearchFilter.Month.HasValue ? Context.MonthlyValuesAsync() : Context.AnnualValuesAsync());

            return Context.CurrencyList;
        }
    }
}