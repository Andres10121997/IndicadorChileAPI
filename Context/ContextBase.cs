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
        private float V_Currency;
        private double V_CurrencyConversion;

        #region Readonly
        private readonly string VR_Url;
        private readonly ushort VR_Year;
        private readonly ushort? VR_Month;
        #endregion
        #endregion

        #region Arrays
        private CurrencyModel[] A_CurrencyList;
        #endregion



        #region ConstructorMethod
        public ContextBase(string Url,
                           ushort Year,
                           ushort? Month)
            : base()
        {
            DateOnly Date;
            bool[] MonthValidations;

            Date = DateOnly.FromDateTime(DateTime.Now);
            MonthValidations = new bool[]
            {
                Month < 1
                ||
                Month > 12,
                Month > Date.Month
                &&
                (Year == Date.Year || Year.Equals(obj: Date.Year))
            };
            
            #region Validations
            ArgumentNullException.ThrowIfNullOrEmpty(argument: Url);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(argument: Url);
            ArgumentOutOfRangeException.ThrowIfGreaterThan<int>(
                value: Year,
                other: Date.Year
            );

            /*
            if (Month < 1
                ||
                Month > 12)
            {
                throw new ArgumentException(message: "The month number must be between 1 and 12.", paramName: nameof(Month));
            }
            else
            if (Month > DateTime.Now.Month
                &&
                (Year == DateTime.Now.Year || Year.Equals(obj: DateTime.Now.Year)))
            {
                throw new Exception(message: "The query cannot be performed.");
            }
            */
            if (MonthValidations.Contains(true))
            {
                throw new ArgumentException(message: "", paramName: nameof(Month));
            }
            #endregion

            #region Variables
            #region Readonly
            this.VR_Url = Url;
            this.VR_Year = Year;
            this.VR_Month = Month;
            #endregion
            #endregion

            #region Arrays
            this.A_CurrencyList = Array.Empty<CurrencyModel>();
            #endregion
        }
        #endregion



        #region Getters And Setters
        #region Variables
        public float Currency
        {
            get => this.V_Currency;
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
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero<float>(value: value);

                this.V_Currency = value;
            }
        }

        public double CurrencyConversion
        {
            get => this.V_CurrencyConversion;
            set
            {
                ArgumentOutOfRangeException.ThrowIfEqual<double>(
                    value: value,
                    other: double.NaN
                );
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero<double>(value: value);

                this.V_CurrencyConversion = value;
            }
        }

        #region Readonly
        protected string Url
        {
            get => this.VR_Url.Trim();
        }

        protected ushort Year
        {
            get => this.VR_Year;
        }

        protected ushort? Month
        {
            get => this.VR_Month;
        }
        #endregion
        #endregion

        #region Arrays
        public CurrencyModel[] CurrencyList
        {
            get => this.A_CurrencyList;
            set
            {
                ArgumentNullException.ThrowIfNull(argument: value);

                this.A_CurrencyList = value;
            }
        }
        #endregion
        #endregion



        #region Values
        public async Task<CurrencyModel[]> AnnualValuesAsync()
        {
            return (
                await this.TransformToCurrencyModelsAsync(
                        CurrencyData: await this.ExtractValuesAsync(
                            htmlContent: await this.GetHtmlContentAsync(),
                            tableId: "table_export".Trim()
                        )
                    )
                )
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
                .Where<CurrencyModel>(predicate: Model => Model.Date.Year == this.Year
                                                          &&
                                                          Model.Date.Month == this.Month)
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
                        .FirstOrDefault<CurrencyModel>();

            // Si aún no se encuentra, buscar en valores anuales
            Value ??= (await this.AnnualValuesAsync())
                    .Where<CurrencyModel>(predicate: Model => Model.Date < Date)
                    .OrderByDescending<CurrencyModel, DateOnly>(keySelector: Model => Model.Date)
                    .FirstOrDefault<CurrencyModel>();

            ArgumentNullException.ThrowIfNull(argument: Value);

            return Value;
        }
        #endregion



        #region Conversion
        public async Task<uint> ConversionInChileanPesosAsync(DateOnly Date,
                                                              float AmountOfCurrency)
        {
            #region Variables
            uint Pesos;
            #endregion

            this.Currency = (await this.DailyValueAsync(Date: Date)).Currency;

            this.CurrencyConversion = Math.Truncate(d: AmountOfCurrency * this.Currency);

            Pesos = Convert.ToUInt32(value: this.CurrencyConversion);

            return Pesos;
        }

        public async Task<float> ConversionIntoAnotherCurrencyAsync(DateOnly Date,
                                                                    ulong Pesos)
        {
            #region Variables
            float AnotherCurrency;
            #endregion

            this.Currency = (await this.DailyValueAsync(Date: Date)).Currency;

            this.CurrencyConversion = Math.Round(value: Pesos / this.Currency, digits: 2);

            AnotherCurrency = Convert.ToSingle(value: this.CurrencyConversion);

            return AnotherCurrency;
        }
        #endregion



        #region Mathematics
        public async Task<StatisticsModel> GetStatisticsAsync() => new StatisticsModel()
        {
            StartDate = this.CurrencyList.Min<CurrencyModel, DateOnly>(selector: Minimum => Minimum.Date),
            EndDate = this.CurrencyList.Max<CurrencyModel, DateOnly>(selector: Maximum => Maximum.Date),
            AmountOfData = Convert.ToUInt16(value: this.CurrencyList.Length),
            Minimum = this.CurrencyList.Min<CurrencyModel>(selector: Minimum => Minimum.Currency),
            Maximum = this.CurrencyList.Max<CurrencyModel>(selector: Maximum => Maximum.Currency),
            Summation = this.CurrencyList.Sum<CurrencyModel>(selector: x => x.Currency),
            SumOfSquares = this.CurrencyList.Sum<CurrencyModel>(x => Math.Pow(x: x.Currency, y: 2)),
            Average = this.CurrencyList.Average<CurrencyModel>(selector: Average => Average.Currency),
            StandardDeviation = await Statistics.StandardDeviationAsync(Values: this.CurrencyList.Select<CurrencyModel, float>(selector: StandardDeviation => StandardDeviation.Currency).ToArray<float>()),
            Variance = this.CurrencyList.Select(value => value.Currency).Variance()
        };
        #endregion



        #region HTML
        protected async Task<string> GetHtmlContentAsync()
        {
            using HttpClient client = new HttpClient();

            return await client.GetStringAsync(requestUri: this.Url);
        }

        protected Dictionary<byte, float[]> ExtractValues(string htmlContent,
                                                          string tableId)
        {
            #region Variables
            string tablePattern = string.Empty;
            string tableHtml = string.Empty;
            string rowPattern = string.Empty;
            #endregion

            #region Dictionary
            Dictionary<byte, float[]> Data = new Dictionary<byte, float[]>();
            #endregion

            #region Match
            Match tableMatch;
            MatchCollection rowMatches;
            #endregion

            #region Validations
            ArgumentNullException.ThrowIfNullOrEmpty(argument: htmlContent);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(argument: htmlContent);
            ArgumentNullException.ThrowIfNullOrEmpty(argument: tableId);
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
            rowMatches = Regex.Matches(input: tableHtml, pattern: rowPattern, options: RegexOptions.Singleline);

            foreach (Match rowMatch in rowMatches)
            {
                #region Variables
                string rowHtml = string.Empty;
                string cellPattern = string.Empty;
                #endregion

                MatchCollection cellMatches;

                rowHtml = rowMatch.Groups[1].Value;

                // Regex para las celdas (<th> y <td>)
                cellPattern = @"<t[hd][^>]*>(.*?)<\/t[hd]>";
                cellMatches = Regex.Matches(input: rowHtml, pattern: cellPattern, options: RegexOptions.Singleline);

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

                            #region GuardarValores
                            if (float.TryParse(s: Value, style: NumberStyles.Float, provider: CultureInfo.InvariantCulture, result: out float currencyValue))
                            {
                                Values[i - 1] = currencyValue;
                            }
                            else
                            {
                                Values[i - 1] = float.NaN;
                            }
                            #endregion
                        }

                        Data[day] = Values;
                    }
                }
            }

            return Data;
        }

        protected async Task<Dictionary<byte, float[]>> ExtractValuesAsync(string htmlContent,
                                                                           string tableId)
        {
            return await Task.Run<Dictionary<byte, float[]>>(
                function: () => this.ExtractValues(htmlContent: htmlContent, tableId: tableId)
            );
        }
        #endregion



        #region Transform
        protected TModel[] TransformToModels<TModel>(Dictionary<byte, float[]> Data,
                                                     Func<DateOnly, float, TModel> modelFactory)
        {
            #region List
            List<TModel> ModelList = new List<TModel>();
            #endregion

            foreach (var (day, values) in Data)
            {
                for (byte month = 1; month <= values.Length; month++)
                {
                    if (day > 0 && day <= DateTime.DaysInMonth(year: this.Year, month: month))
                    {
                        #region Variables
                        float value;
                        #endregion

                        #region Objects
                        TModel model;
                        #endregion

                        value = values[month - 1];

                        model = modelFactory(
                            new DateOnly(
                                year: this.Year,
                                month: month,
                                day: day
                            ),
                            value
                        );

                        ModelList.Add(item: model);
                    }
                }
            }

            return ModelList.ToArray<TModel>();
        }

        protected async Task<TModel[]> TransformToModelsAsync<TModel>(Dictionary<byte, float[]> Data,
                                                                      Func<DateOnly, float, TModel> modelFactory)
        {
            return await Task.Run<TModel[]>(
                function: () => this.TransformToModels<TModel>(Data: Data, modelFactory)
            );
        }

        protected async Task<CurrencyModel[]> TransformToCurrencyModelsAsync(Dictionary<byte, float[]> CurrencyData)
        {
            return await this.TransformToModelsAsync<CurrencyModel>(Data: CurrencyData, modelFactory: (Date, Value) => new CurrencyModel
            {
                ID = uint.Parse(s: Date.ToString(format: "yyyyMMdd")),
                Date = Date,
                WeekdayName = Date.ToString(format: "dddd", provider: CultureInfo.CreateSpecificCulture(name: "es")),
                Currency = Value
            });
        }
        #endregion
    }
}