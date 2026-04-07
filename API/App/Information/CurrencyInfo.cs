using API.App.Context;
using API.App.Record;
using API.Models;
using API.Models.Get;
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



        public static async Task<CurrencyListHeaderModel> CurrencyHeaderAsync(CurrencyInfoRecord CurrencyInfo,
                                                                              SearchFilterModel SearchFilter)
        {
            #region Variables
            string? MonthName;
            DateTime Now;
            #endregion

            #region Objects
            CurrencyListHeaderModel CurrencyList;
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

            CurrencyList = new CurrencyListHeaderModel()
            {
                ConsultationDate = DateOnly.FromDateTime(dateTime: Now),
                ConsultationTime = TimeOnly.FromDateTime(dateTime: Now),
                Year = SearchFilter.Year,
                MonthName = MonthName,
                Currencies = await GetCurrenciesAsync(SearchFilter: SearchFilter,
                                                      CurrencyInfo: CurrencyInfo)
            };

            return CurrencyList;
        }

        public static async Task<CurrencyModel[]> GetCurrenciesAsync(CurrencyInfoRecord CurrencyInfo,
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