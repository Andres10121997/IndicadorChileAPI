using API.App.DTO.Currency;
using API.App.DTO.HTML;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.App.Context.Tool
{
    public class Value
    {
        #region Objects
        private CurrencyInfoDto currencyInfo;
        private SearchFilterModel searchFilter;
        #endregion

        private Task<string> htmlContentAsync;



        public Value(CurrencyInfoDto CurrencyInfo,
                     SearchFilterModel SearchFilter,
                     Task<string> HtmlContentAsync)
        {
            #region Objects
            this.currencyInfo = CurrencyInfo;
            this.searchFilter = SearchFilter;
            #endregion

            this.htmlContentAsync = HtmlContentAsync;
        }



        public async Task<CurrencyDto[]> AnnualAsync()
        {
            #region Collections
            Task<Dictionary<byte, float[]>> values;
            #endregion

            values = Extract.ValuesAsync(
                Html: new HtmlDto
                {
                    Content = await htmlContentAsync,
                    Table = new TableDto
                    {
                        ID = this.currencyInfo.Table.ID
                    }
                }
            );

            VarGlobal.Currencies = await new Transform(SearchFilter: this.searchFilter).ToCurrencyModelsAsync(CurrencyData: await values);

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

        public async Task<CurrencyDto[]> MonthlyAsync()
        {
            VarGlobal.Currencies = await AnnualAsync();

            return VarGlobal.Currencies
                .AsParallel<CurrencyDto>()
                .Where<CurrencyDto>(predicate: Model => Model.Date.Year == this.searchFilter.Year
                                                        &&
                                                        Model.Date.Month == this.searchFilter.Month)
                .ToArray<CurrencyDto>();
        }

        public async Task<CurrencyDto> DailyAsync(DateOnly Date)
        {
            #region Objects
            CurrencyDto? value;
            #endregion

            VarGlobal.Currencies = await AnnualAsync();

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