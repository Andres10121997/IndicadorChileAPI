using API.App.Context.Tool;
using API.App.DTO.Currency;
using API.App.DTO.HTML;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.App.Context
{
    public class ContextBase
    {
        #region Objects
        private CurrencyInfoDto currencyInfo;
        private SearchFilterModel searchFilter;
        #endregion



        #region Constructor Method
        public ContextBase(CurrencyInfoDto CurrencyInfo,
                           SearchFilterModel SearchFilter)
            : base()
        {
            #region Objects
            this.currencyInfo = CurrencyInfo;
            this.searchFilter = SearchFilter;
            #endregion
        }
        #endregion



        #region Field
        #region Objects
        protected CurrencyInfoDto CurrencyInfo
        {
            get => this.currencyInfo;
        }
        
        protected SearchFilterModel SearchFilter
        {
            get => this.searchFilter;
        }
        #endregion
        #endregion



        #region Values
        public async Task<CurrencyDto[]> AnnualValuesAsync()
        {
            #region Collections
            Task<Dictionary<byte, float[]>> values;
            #endregion

            values = Extract.ValuesAsync(
                Html: new HtmlDto
                {
                    Content = await this.GetHtmlContentAsync(),
                    Table = new TableDto
                    {
                        ID = this.CurrencyInfo.Table.ID
                    }
                }
            );

            VarGlobal.Currencies = await new Transform(SearchFilter: this.SearchFilter).ToCurrencyModelsAsync(CurrencyData: await values);

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

        public async Task<CurrencyDto[]> MonthlyValuesAsync()
        {
            VarGlobal.Currencies = await this.AnnualValuesAsync();
            
            return VarGlobal.Currencies
                .AsParallel<CurrencyDto>()
                .Where<CurrencyDto>(predicate: Model => Model.Date.Year == this.SearchFilter.Year
                                                        &&
                                                        Model.Date.Month == this.SearchFilter.Month)
                .ToArray<CurrencyDto>();
        }

        public async Task<CurrencyDto> DailyValueAsync(DateOnly Date)
        {
            #region Objects
            CurrencyDto? value;
            #endregion

            VarGlobal.Currencies = await this.AnnualValuesAsync();

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
        #endregion



        #region HTML
        protected async Task<string> GetHtmlContentAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                return await client.GetStringAsync(requestUri: this.currencyInfo.Url);
            }
        }
        #endregion
    }
}