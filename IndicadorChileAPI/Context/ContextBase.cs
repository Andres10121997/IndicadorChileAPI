using IndicadorChileAPI.Context.Tool;
using IndicadorChileAPI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IndicadorChileAPI.Context
{
    public class ContextBase
    {
        #region Variables
        private float currency;
        private float currencyConversion;

        #region Readonly
        private readonly string VR_Url;
        #endregion
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
            #region Variables
            this.currency = 0;
            this.currencyConversion = 0;
            
            #region Readonly
            this.VR_Url = Url;
            #endregion
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
        #region Variables
        public float Currency
        {
            get => this.currency;
            set
            {
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

                this.currency = value;
            }
        }

        public float CurrencyConversion
        {
            get => this.currencyConversion;
            set
            {
                ArgumentOutOfRangeException.ThrowIfEqual<float>(
                    value: value,
                    other: float.NaN
                );
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero<float>(
                    value: value
                );

                this.currencyConversion = value;
            }
        }

        #region Readonly
        protected string Url
        {
            get => this.VR_Url.Trim();
        }
        #endregion
        #endregion

        #region Arrays
        public CurrencyModel[] CurrencyList
        {
            get => this.currencyList;
            set
            {
                ArgumentNullException.ThrowIfNull(argument: value);

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
            Transform transform = new(Search: this.SearchFilter);
            
            return (await transform.ToCurrencyModelsAsync(
                        CurrencyData: await this.ExtractValuesAsync(
                            htmlContent: await this.GetHtmlContentAsync(),
                            tableId: "table_export".Trim()
                        )
                   ))
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
            return (await this.AnnualValuesAsync())
                .AsParallel<CurrencyModel>()
                .Where<CurrencyModel>(predicate: Model => Model.Date.Year == this.SearchFilter.Year
                                                          &&
                                                          Model.Date.Month == this.SearchFilter.Month)
                .ToArray<CurrencyModel>();
        }

        public async Task<CurrencyModel> DailyValueAsync(DateOnly Date)
        {
            #region Arrays
            CurrencyModel[] MonthlyValues;
            #endregion

            #region Objects
            CurrencyModel? Value;
            #endregion

            MonthlyValues = await this.MonthlyValuesAsync();

            // Buscar valor exacto
            Value = MonthlyValues
                        .FirstOrDefault<CurrencyModel>(predicate: model => model.Date == Date)
                    ??
                    // Si no se encuentra, buscar el valor más reciente antes de la fecha (mensual)
                    MonthlyValues
                        .Where<CurrencyModel>(predicate: Model => Model.Date < Date)
                        .OrderByDescending<CurrencyModel, DateOnly>(keySelector: Model => Model.Date)
                        .FirstOrDefault<CurrencyModel>()
                    ??
                    // Si aún no se encuentra, buscar en valores anuales
                    (await this.AnnualValuesAsync())
                        .Where<CurrencyModel>(predicate: Model => Model.Date < Date)
                        .OrderByDescending<CurrencyModel, DateOnly>(keySelector: Model => Model.Date)
                        .FirstOrDefault<CurrencyModel>();

            ArgumentNullException.ThrowIfNull(argument: Value);

            return Value;
        }
        #endregion



        #region HTML
        protected async Task<string> GetHtmlContentAsync()
        {
            using HttpClient client = new();

            return await client.GetStringAsync(requestUri: this.Url);
        }

        protected Dictionary<byte, float[]> ExtractValues(string htmlContent,
                                                          string tableId)
        {
            #region Variables
            string tablePattern;
            string tableHtml;
            string rowPattern;
            #endregion

            #region Objects
            object lockObject = new object();

            #region Match
            Match tableMatch;
            MatchCollection rowMatches;
            #endregion
            #endregion

            #region Dictionary
            Dictionary<byte, float[]> Data = new Dictionary<byte, float[]>();
            #endregion

            #region Validations
            ArgumentNullException.ThrowIfNullOrWhiteSpace(argument: htmlContent);

            ArgumentNullException.ThrowIfNullOrWhiteSpace(argument: tableId);
            #endregion

            // Regex para encontrar la tabla con el ID dinámico
            tablePattern = $@"<table[^>]*id=""{Regex.Escape(str: tableId)}""[^>]*>(.*?)<\/table>";
            tableMatch = Regex.Match(input: htmlContent, pattern: tablePattern, options: RegexOptions.Singleline);

            if (!tableMatch.Success)
            {
                throw new Exception(message: $"No se encontró la tabla con ID '{tableId}'.");
            }

            tableHtml = tableMatch.Groups[1].Value;

            // Regex para las filas de la tabla
            rowPattern = @"<tr>(.*?)<\/tr>";
            rowMatches = Regex.Matches(
                input: tableHtml,
                pattern: rowPattern,
                options: RegexOptions.Singleline
            );

            Parallel.ForEach(source: rowMatches, body: rowMatch =>
            {
                #region Variables
                string rowHtml = string.Empty;
                string cellPattern = string.Empty;
                #endregion

                MatchCollection cellMatches;

                rowHtml = rowMatch.Groups[1].Value;

                // Regex para las celdas (<th> y <td>)
                cellPattern = @"<t[hd][^>]*>(.*?)<\/t[hd]>";
                cellMatches = Regex.Matches(
                    input: rowHtml,
                    pattern: cellPattern,
                    options: RegexOptions.Singleline
                );

                if (cellMatches.Count > 0)
                {
                    // Primera celda: el día
                    if (byte.TryParse(s: Regex.Replace(input: cellMatches[0].Groups[1].Value, pattern: @"\D", replacement: ""), result: out byte day))
                    {
                        #region Arrays
                        float[] Values = new float[12];
                        #endregion

                        for (byte i = 1; i < cellMatches.Count; i++)
                        {
                            #region Variables
                            string Value = string.Empty;
                            #endregion

                            Value = cellMatches[i].Groups[1].Value
                                .Trim()
                                // Eliminar puntos
                                .Replace(
                                    oldValue: ".",
                                    newValue: ""
                                )
                                // Cambiar comas por puntos
                                .Replace(
                                    oldValue: ",",
                                    newValue: "."
                                );

                            #region Guardar Valores
                            if (float.TryParse(s: Value, style: NumberStyles.Number, provider: CultureInfo.InvariantCulture, result: out float currencyValue))
                            {
                                Values[i - 1] = currencyValue;
                            }
                            else
                            {
                                Values[i - 1] = float.NaN;
                            }
                            #endregion
                        }

                        lock (lockObject)
                        {
                            Data[day] = Values;
                        }
                    }
                }
            });
            
            return Data;
        }

        protected async Task<Dictionary<byte, float[]>> ExtractValuesAsync(string htmlContent,
                                                                           string tableId)
        {
            return await Task.Run<Dictionary<byte, float[]>>(
                function: () => this.ExtractValues(
                    htmlContent: htmlContent,
                    tableId: tableId
                )
            );
        }
        #endregion
    }
}