using API.App.Context.Tool;
using API.App.Record.Currency;
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
        #region Variables
        private float currency;
        #endregion

        #region Collections
        private CurrencyRecord[] currencies;
        #endregion

        #region Objects
        private CurrencyInfoRecord currencyInfo;
        private SearchFilterModel searchFilter;
        #endregion



        #region Constructor Method
        public ContextBase(CurrencyInfoRecord CurrencyInfo,
                           SearchFilterModel SearchFilter)
            : base()
        {
            #region Variables
            this.currency = 0;
            #endregion

            #region Collections
            this.currencies = Array.Empty<CurrencyRecord>();
            #endregion

            #region Objects
            this.currencyInfo = CurrencyInfo;
            this.searchFilter = SearchFilter;
            #endregion
        }
        #endregion



        #region Field
        #region Variables
        public float Currency
        {
            get => this.currency;
            set
            {
                #region Exception
                ArgumentOutOfRangeException.ThrowIfEqual<float>(
                    value: value,
                    other: float.NaN
                );
                ArgumentOutOfRangeException.ThrowIfEqual<float>(
                    value: value,
                    other: float.PositiveInfinity
                );
                ArgumentOutOfRangeException.ThrowIfEqual<float>(
                    value: value,
                    other: float.NegativeInfinity
                );
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero<float>(
                    value: value
                );
                #endregion

                this.currency = value;
            }
        }
        #endregion

        #region Collections
        public CurrencyRecord[] Currencies
        {
            get => this.currencies;
            set
            {
                #region Exception
                ArgumentNullException.ThrowIfNull(argument: value);
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value: value.Length);
                #endregion

                this.currencies = value;
            }
        }
        #endregion

        #region Objects
        protected SearchFilterModel SearchFilter
        {
            get => this.searchFilter;
        }
        #endregion
        #endregion



        #region Values
        public async Task<CurrencyRecord[]> AnnualValuesAsync()
        {
            #region Variables
            Task<string> htmlContent;
            #endregion

            #region Collections
            Task<Dictionary<byte, float[]>> values;
            #endregion

            htmlContent = this.GetHtmlContentAsync();

            values = Extract.ValuesAsync(
                HtmlContent: await htmlContent,
                TableId: this.currencyInfo.TableId
            );

            this.Currencies = await new Transform(Search: this.SearchFilter).ToCurrencyModelsAsync(CurrencyData: await values);

            return this.Currencies
                .AsParallel<CurrencyRecord>()
                .Where<CurrencyRecord>(predicate: Model => !float.IsNaN(f: Model.Currency)
                                                           &&
                                                           !float.IsInfinity(f: Model.Currency)
                                                           &&
                                                           !float.IsNegative(f: Model.Currency))
                .OrderBy<CurrencyRecord, DateOnly>(keySelector: Model => Model.Date)
                .ToArray<CurrencyRecord>();
        }

        public async Task<CurrencyRecord[]> MonthlyValuesAsync()
        {
            this.Currencies = await this.AnnualValuesAsync();
            
            return this.Currencies
                .AsParallel<CurrencyRecord>()
                .Where<CurrencyRecord>(predicate: Model => Model.Date.Year == this.SearchFilter.Year
                                                           &&
                                                           Model.Date.Month == this.SearchFilter.Month)
                .ToArray<CurrencyRecord>();
        }

        public async Task<CurrencyRecord> DailyValueAsync(DateOnly Date)
        {
            #region Objects
            CurrencyRecord? Value;
            #endregion

            this.Currencies = await this.MonthlyValuesAsync();

            // Buscar valor exacto
            Value = this.Currencies
                        .FirstOrDefault<CurrencyRecord>(predicate: model => model.Date == Date)
                    ??
                    // Si no se encuentra, buscar el valor más reciente antes de la fecha (mensual)
                    this.Currencies
                        .Where<CurrencyRecord>(predicate: Model => Model.Date < Date)
                        .OrderByDescending<CurrencyRecord, DateOnly>(keySelector: Model => Model.Date)
                        .FirstOrDefault<CurrencyRecord>()
                    ??
                    // Si aún no se encuentra, buscar en valores anuales
                    (await this.AnnualValuesAsync())
                        .Where<CurrencyRecord>(predicate: Model => Model.Date < Date)
                        .OrderByDescending<CurrencyRecord, DateOnly>(keySelector: Model => Model.Date)
                        .FirstOrDefault<CurrencyRecord>();

            #region Exception
            ArgumentNullException.ThrowIfNull(argument: Value);
            #endregion

            return Value;
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