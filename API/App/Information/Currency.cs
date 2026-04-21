using API.App.Context;
using API.App.DTO.Currency;
using API.Models;
using System;
using System.Threading.Tasks;

namespace API.App.Information
{
    public static class Currency
    {
        #region Constructor Method
        static Currency()
        {
            
        }
        #endregion



        public static async Task<CurrencyHeaderDto> HeaderAsync(CurrencyInfoDto CurrencyInfo,
                                                                SearchFilterModel SearchFilter)
        {
            #region Variables
            string? monthName;
            #endregion

            #region Objects
            CurrencyHeaderDto currencyHeader;
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

            VarGlobal.Currencies = await GetAsync(
                SearchFilter: SearchFilter,
                CurrencyInfo: CurrencyInfo
            );

            currencyHeader = new CurrencyHeaderDto
            {
                ConsultationDateTime = VarGlobal.Now,
                Year = SearchFilter.Year,
                MonthName = monthName,
                Currencies = VarGlobal.Currencies
            };

            return currencyHeader;
        }

        private static async Task<CurrencyDto[]> GetAsync(CurrencyInfoDto CurrencyInfo,
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
            VarGlobal.Currencies = await context.Values();

            return VarGlobal.Currencies;
        }
    }
}