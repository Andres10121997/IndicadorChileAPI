using IndicadorChileAPI.Context;
using IndicadorChileAPI.Models;
using System.Threading.Tasks;

namespace IndicadorChileAPI.Information
{
    public static class CurrencyInfo
    {
        static CurrencyInfo()
        {
            
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