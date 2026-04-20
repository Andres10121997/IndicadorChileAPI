using API.App.Context.Tool;
using API.App.DTO.Currency;
using API.Models;
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
        public async Task<CurrencyDto[]> Values()
        {
            switch (this.SearchFilter.Month.HasValue)
            {
                case true:
                    return await Value.MonthlyAsync(
                        SearchFilter: this.SearchFilter,
                        CurrencyInfo: this.CurrencyInfo,
                        HtmlContentAsync: this.GetHtmlContentAsync()
                    );
                case false:
                    return await Value.AnnualAsync(
                        SearchFilter: this.SearchFilter,
                        CurrencyInfo: this.CurrencyInfo,
                        HtmlContentAsync: this.GetHtmlContentAsync()
                    );
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