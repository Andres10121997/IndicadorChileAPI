using IndicadorChileAPI.Context;
using IndicadorChileAPI.Models;
using System.Threading.Tasks;

namespace IndicadorChileAPI.Info
{
    public static class CurrencyInfo
    {
        static CurrencyInfo()
        {
            
        }



        public static async Task<CurrencyModel[]> GetValuesAsync(SearchFilterModel SearchFilter, string Url)
        {
            ContextBase Context;

            Context = new ContextBase(
                Url: Url.Replace(
                    oldValue: "{Year}",
                    newValue: SearchFilter.Year.ToString()
                ),
                SearchFilter: SearchFilter
            );

            Context.CurrencyList = await (SearchFilter.Month.HasValue ? Context.MonthlyValuesAsync() : Context.AnnualValuesAsync()); // Ternaria para obtener datos.

            return Context.CurrencyList;
        }
    }
}