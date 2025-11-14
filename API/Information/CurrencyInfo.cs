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