using API.App.DTO.Currency;
using API.App.DTO.HTML;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.App.Context.Tool
{
    public static class Value
    {
        static Value()
        {
            
        }



        public static async Task<CurrencyDto[]> AnnualAsync(SearchFilterModel SearchFilter, CurrencyInfoDto CurrencyInfo, Task<string> HtmlContentAsync)
        {
            #region Collections
            Task<Dictionary<byte, float[]>> values;
            #endregion

            values = Extract.ValuesAsync(
                Html: new HtmlDto
                {
                    Content = await HtmlContentAsync,
                    Table = new TableDto
                    {
                        ID = CurrencyInfo.Table.ID
                    }
                }
            );

            VarGlobal.Currencies = await new Transform(SearchFilter: SearchFilter).ToCurrencyModelsAsync(CurrencyData: await values);

            return VarGlobal.Currencies
                .AsParallel<CurrencyDto>()
                .Where<CurrencyDto>(predicate: Model => !float.IsNaN(f: Model.Currency)
                                                        &&
                                                        !float.IsInfinity(f: Model.Currency)
                                                        &&
                                                        !float.IsNegative(f: Model.Currency))
                .OrderBy<CurrencyDto, DateOnly>(keySelector: Model => Model.Date)
                .ToArray<CurrencyDto>();
        }

        public static async Task<CurrencyDto[]> MonthlyAsync(SearchFilterModel SearchFilter, CurrencyInfoDto CurrencyInfo, Task<string> HtmlContentAsync)
        {
            VarGlobal.Currencies = await AnnualAsync(SearchFilter: SearchFilter, CurrencyInfo: CurrencyInfo, HtmlContentAsync: HtmlContentAsync);

            return VarGlobal.Currencies
                .AsParallel<CurrencyDto>()
                .Where<CurrencyDto>(predicate: Model => Model.Date.Year == SearchFilter.Year
                                                        &&
                                                        Model.Date.Month == SearchFilter.Month)
                .ToArray<CurrencyDto>();
        }

        public static async Task<CurrencyDto> DailyAsync(SearchFilterModel SearchFilter, CurrencyInfoDto CurrencyInfo, Task<string> HtmlContentAsync, DateOnly Date)
        {
            #region Objects
            CurrencyDto? value;
            #endregion

            VarGlobal.Currencies = await AnnualAsync(SearchFilter: SearchFilter, CurrencyInfo: CurrencyInfo, HtmlContentAsync: HtmlContentAsync);

            // Buscar valor exacto
            value = VarGlobal.Currencies
                        .FirstOrDefault<CurrencyDto>(predicate: model => model.Date == Date)
                    ??
                    // Si no se encuentra, buscar el valor más reciente antes de la fecha (mensual)
                    VarGlobal.Currencies
                        .Where<CurrencyDto>(predicate: Model => Model.Date < Date)
                        .OrderByDescending<CurrencyDto, DateOnly>(keySelector: Model => Model.Date)
                        .FirstOrDefault<CurrencyDto>();

            #region Exception
            ArgumentNullException.ThrowIfNull(argument: value);
            #endregion

            return value;
        }
    }
}