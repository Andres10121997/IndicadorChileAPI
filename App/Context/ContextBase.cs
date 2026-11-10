using IndicadorChileAPI.App.Context.Tool;
using IndicadorChileAPI.App.DTO.Currency;
using IndicadorChileAPI.Models;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;

namespace IndicadorChileAPI.App.Context
{
    public class ContextBase<TCurrency>
        where TCurrency : struct, IFloatingPointConstants<TCurrency>
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
        public async Task<CurrencyDto<TCurrency>[]> Values()
        {
            #region Collections
            CurrencyDto<TCurrency>[] currencies;
            #endregion

            currencies = await Term<TCurrency>.GetAsync(
                HtmlContent: await GetHtmlContentAsync(),
                CurrencyInfo: CurrencyInfo,
                SearchFilter: SearchFilter
            );

            return currencies;
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