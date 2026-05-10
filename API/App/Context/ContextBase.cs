using API.App.Context.Tool;
using API.App.DTO.Currency;
using API.Models;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;

namespace API.App.Context
{
    public class ContextBase<T>
        where T : struct, IFloatingPoint<T>
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
        public async Task<CurrencyDto<T>[]> Values()
        {
            #region Objects
            Value<T> value;
            #endregion

            value = new Value<T>(
                CurrencyInfo: this.CurrencyInfo,
                SearchFilter: this.SearchFilter,
                HtmlContentAsync: this.GetHtmlContentAsync()
            );

            switch (this.SearchFilter.Month.HasValue)
            {
                case true:
                    return await value.MonthlyAsync();
                case false:
                    return await value.AnnualAsync();
            }
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