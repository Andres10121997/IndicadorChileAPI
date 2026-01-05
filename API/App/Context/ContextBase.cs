using API.App.Context.Tool;
using API.Models;
using API.Models.Get;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.App.Context
{
    public class ContextBase
    {
        #region Readonly
        private readonly string url;
        #endregion

        #region Variables
        private float currency;
        #endregion

        #region Arrays
        private CurrencyModel[] currencyList;
        #endregion

        #region Objects
        private SearchFilterModel searchFilter;
        #endregion



        #region Constructor Method
        public ContextBase(string Url,
                           SearchFilterModel SearchFilter)
            : base()
        {
            #region Readonly
            this.url = Url;
            #endregion

            #region Variables
            this.currency = 0;
            #endregion

            #region Arrays
            this.currencyList = Array.Empty<CurrencyModel>();
            #endregion

            #region Objects
            this.searchFilter = SearchFilter;
            #endregion
        }
        #endregion



        #region Property
        #region Readonly
        protected string Url
        {
            get => this.url.Trim();
        }
        #endregion

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

        #region Arrays
        public CurrencyModel[] CurrencyList
        {
            get => this.currencyList;
            set
            {
                #region Exception
                ArgumentNullException.ThrowIfNull(argument: value);
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value: value.Length);
                #endregion

                this.currencyList = value;
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
        public async Task<CurrencyModel[]> AnnualValuesAsync()
        {
            this.CurrencyList = await new Transform(Search: this.SearchFilter).ToCurrencyModelsAsync(
                CurrencyData: await Extract.ValuesAsync(
                    htmlContent: await this.GetHtmlContentAsync(),
                    tableId: "table_export".Trim()
                )
            );


            return this.CurrencyList
                       .AsParallel<CurrencyModel>()
                       .Where<CurrencyModel>(predicate: Model => !float.IsNaN(f: Model.Currency)
                                                        &&
                                                        !float.IsInfinity(f: Model.Currency)
                                                        &&
                                                        !float.IsNegative(f: Model.Currency))
                       .OrderBy<CurrencyModel, DateOnly>(keySelector: Model => Model.Date)
                       .ToArray<CurrencyModel>();
        }

        public async Task<CurrencyModel[]> MonthlyValuesAsync()
        {
            this.CurrencyList = await this.AnnualValuesAsync();
            
            return this.CurrencyList
                       .AsParallel<CurrencyModel>()
                       .Where<CurrencyModel>(predicate: Model => Model.Date.Year == this.SearchFilter.Year
                                                                 &&
                                                                 Model.Date.Month == this.SearchFilter.Month)
                       .ToArray<CurrencyModel>();
        }

        public async Task<CurrencyModel> DailyValueAsync(DateOnly Date)
        {
            #region Objects
            CurrencyModel? Value;
            #endregion

            this.CurrencyList = await this.MonthlyValuesAsync();

            // Buscar valor exacto
            Value = this.CurrencyList
                        .FirstOrDefault<CurrencyModel>(predicate: model => model.Date == Date)
                    ??
                    // Si no se encuentra, buscar el valor más reciente antes de la fecha (mensual)
                    this.CurrencyList
                        .Where<CurrencyModel>(predicate: Model => Model.Date < Date)
                        .OrderByDescending<CurrencyModel, DateOnly>(keySelector: Model => Model.Date)
                        .FirstOrDefault<CurrencyModel>()
                    ??
                    // Si aún no se encuentra, buscar en valores anuales
                    (await this.AnnualValuesAsync())
                        .Where<CurrencyModel>(predicate: Model => Model.Date < Date)
                        .OrderByDescending<CurrencyModel, DateOnly>(keySelector: Model => Model.Date)
                        .FirstOrDefault<CurrencyModel>();

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
                return await client.GetStringAsync(requestUri: this.Url);
            }
        }
        #endregion
    }
}