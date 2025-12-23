using API.Context;
using API.Models;
using System;
using System.Threading.Tasks;

namespace API.Information
{
    public static class CurrencyInfo
    {
        #region Constructor Method
        static CurrencyInfo()
        {
            
        }
        #endregion



        public static async Task<CurrencyListHeaderModel> DataListAsync(SearchFilterModel SearchFilter,
                                                                        string Url)
        {
            #region Variables
            DateTime Now;
            #endregion

            #region Objects
            CurrencyListHeaderModel CurrencyList;
            #endregion

            Now = DateTime.Now;

            CurrencyList = new CurrencyListHeaderModel()
            {
                ConsultationDate = DateOnly.FromDateTime(dateTime: Now),
                ConsultationTime = TimeOnly.FromDateTime(dateTime: Now),
                Year = SearchFilter.Year,
                MonthName = SearchFilter.Month.HasValue ? new DateOnly(year: SearchFilter.Year, month: Convert.ToInt32(value: SearchFilter.Month), day: 1).ToString(format: "MMMM") : null,
                Currencies = await GetValuesAsync(SearchFilter: SearchFilter, Url: Url)
            };

            return CurrencyList;
        }

        public static async Task<CurrencyModel[]> GetValuesAsync(SearchFilterModel SearchFilter,
                                                                 string Url)
        {
            ContextBase Context;

            Context = new ContextBase(
                Url: Url.Replace(
                    oldValue: "{Year}",
                    newValue: SearchFilter.Year.ToString()
                ),
                SearchFilter: SearchFilter
            );

            // Ternaria para obtener datos.
            Context.CurrencyList = await (SearchFilter.Month.HasValue ? Context.MonthlyValuesAsync() : Context.AnnualValuesAsync());

            return Context.CurrencyList;
        }
    }
}