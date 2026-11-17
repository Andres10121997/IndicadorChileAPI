using IndicadorChileAPI.App.DTO;
using IndicadorChileAPI.App.DTO.Currency;
using IndicadorChileAPI.Models;
using IndicadorChileAPI.Utils;
using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace IndicadorChileAPI.App.Context.Tool
{
    internal static class Term<TCurrency>
        where TCurrency : struct, IFloatingPointConstants<TCurrency>
    {
        #region Collections
        private static CurrencyDTO<TCurrency>[] currencies;
        #endregion

        #region Objects
        private static Result<CurrencyDTO<TCurrency>[], ResultErrorDto> currenciesResult;
        #endregion



        #region Constructor Method
        static Term()
        {
            currencies = Array.Empty<CurrencyDTO<TCurrency>>();
            
            currenciesResult = default!;
        }
        #endregion



        internal static async Task<CurrencyDTO<TCurrency>[]> GetAsync(string HtmlContent,
                                                                      CurrencyInfoDTO CurrencyInfo,
                                                                      SearchFilterModel SearchFilter)
        {
            if (SearchFilter.Month.HasValue)
            {
                currencies = await MonthlyAsync(
                    HtmlContent: HtmlContent,
                    CurrencyInfo: CurrencyInfo,
                    SearchFilter: SearchFilter
                );
            }
            else
            {
                currencies = await AnnualAsync(
                    HtmlContent: HtmlContent,
                    CurrencyInfo: CurrencyInfo,
                    SearchFilter: SearchFilter
                );
            }
            
            return currencies;
        }

        private static async Task<CurrencyDTO<TCurrency>[]> AnnualAsync(string HtmlContent,
                                                                        CurrencyInfoDTO CurrencyInfo,
                                                                        SearchFilterModel SearchFilter)
        {
            currenciesResult = await Value<TCurrency>.CurrenciesAsync(
                htmlContent: HtmlContent,
                CurrencyInfo: CurrencyInfo,
                SearchFilter: SearchFilter
            );

            if (!currenciesResult.IsSuccess)
            {
                return default!;
            }

            if (currenciesResult.Value == null)
            {
                return default!;
            }

            currencies = currenciesResult.Value;

            return currencies;
        }

        private static async Task<CurrencyDTO<TCurrency>[]> MonthlyAsync(string HtmlContent,
                                                                         CurrencyInfoDTO CurrencyInfo,
                                                                         SearchFilterModel SearchFilter)
        {
            currenciesResult = await Value<TCurrency>.CurrenciesAsync(
                htmlContent: HtmlContent,
                CurrencyInfo: CurrencyInfo,
                SearchFilter: SearchFilter
            );

            if (!currenciesResult.IsSuccess)
            {
                return default!;
            }

            if (currenciesResult.Value == null)
            {
                return default!;
            }

            if (SearchFilter.Month.Equals(null))
            {
                return default!;
            }

            currencies = currenciesResult.Value;

            return currencies
                .AsParallel()
                .Where(predicate: Model => Model.Date.Year == SearchFilter.Year
                                           &&
                                           Model.Date.Month == (int)SearchFilter.Month)
                .ToArray<CurrencyDTO<TCurrency>>();
        }
    }
}