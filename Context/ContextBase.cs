using System.Collections.Generic;
using System.Globalization;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IndicadorChileAPI.Models;
using System.Linq;

namespace IndicadorChileAPI.Context
{
    public abstract class ContextBase
    {
        #region Variables
        #region Readonly
        private readonly string Url;
        private readonly ushort Year;
        private readonly ushort? Month;
        #endregion
        #endregion

        #region Arrays
        private CurrencyModel[] CurrencyList { get; set; }
        #endregion



        #region ConstructorMethod
        public ContextBase(string Url,
                           ushort Year,
                           ushort? Month)
            : base()
        {
            #region Validations
            if (string.IsNullOrEmpty(value: Url)
                ||
                string.IsNullOrWhiteSpace(value: Url))
            {
                throw new ArgumentNullException(paramName: nameof(Url), message: $"El parámetro {nameof(Url)} no pueder ser nulo, estar vacío o con espacios vacíos.");
            }
            
            if (string.IsNullOrEmpty(value: Year.ToString())
                ||
                string.IsNullOrWhiteSpace(value: Year.ToString()))
            {
                throw new ArgumentNullException(paramName: nameof(Year), message: $"El parámetro {nameof(Year)} no puede ser nulo, estar vacío o con espacios vacíos.");
            }
            else
            if (Year > DateTime.Now.Year)
            {
                throw new ArgumentException(message: $"The year must not be greater than {DateTime.Now.Year}.", paramName: nameof(Year));
            }
            
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
            #endregion

            this.Url = Url;
            this.Year = Year;
            this.Month = Month;
            this.CurrencyList = Array.Empty<CurrencyModel>();
        }
        #endregion



        #region DeconstructorMethod
        ~ContextBase()
        {

        }
        #endregion



        #region GettersAndSetters
        #region Readonly
        protected string GetUrl()
        {
            return this.Url;
        }

        protected ushort GetYear()
        {
            return this.Year;
        }

        protected ushort? GetMonth()
        {
            return this.Month;
        }
        #endregion

        public CurrencyModel[] GetCurrencyList()
        {
            if (this.CurrencyList.Length == 0
                ||
                this.CurrencyList.Length.Equals(obj: 0))
            {
                throw new Exception(message: $"El arreglo/matriz {nameof(this.CurrencyList)} debe tener más de un elemento.");
            }

            return this.CurrencyList;
        }

        public void SetCurrencyList(CurrencyModel[] CurrencyList)
        {
            if (CurrencyList is null
                ||
                CurrencyList == null
                ||
                CurrencyList.Equals(obj: null))
            {
                throw new ArgumentNullException(paramName: nameof(CurrencyList), message: $"El arreglo/matriz {nameof(CurrencyList)} no puede ser nulo.");
            }

            this.CurrencyList = CurrencyList;
        }
        #endregion



        #region Values
        public async Task<CurrencyModel[]> AnnualValuesAsync()
        {
            await Task.Run(function: async () => this.SetCurrencyList(
                CurrencyList: (await this.TransformToCurrencyModelsAsync(
                    CurrencyData: await this.ExtractValuesAsync(
                        htmlContent: await this.GetHtmlContentAsync(),
                        tableId: "table_export".Trim()
                    )
                ))
                .AsParallel<CurrencyModel>()
                .Where<CurrencyModel>(predicate: Model => !float.IsNaN(f: Model.Currency)
                                                          &&
                                                          !float.IsInfinity(f: Model.Currency))
                .OrderBy<CurrencyModel, DateOnly>(keySelector: Model => Model.Date)
                .ToArray<CurrencyModel>()
            ));

            return await Task.Run<CurrencyModel[]>(function: () => this.GetCurrencyList());
        }

        public async Task<CurrencyModel[]> MonthlyValuesAsync()
        {
            await Task.Run(function: async () => this.SetCurrencyList(
                CurrencyList: (await this.AnnualValuesAsync())
                    .AsParallel<CurrencyModel>()
                    .Where<CurrencyModel>(predicate: Model => Model.Date.Year == this.GetYear()
                                                              &&
                                                              Model.Date.Month == this.GetMonth())
                    .ToArray<CurrencyModel>()
            ));
            
            return await Task.Run<CurrencyModel[]>(function: () => this.GetCurrencyList());
        }

        public async Task<CurrencyModel> DailyValueAsync(DateOnly Date)
        {
            #region Objects
            CurrencyModel? Value;
            #endregion

            // Intentar obtener el valor exacto de la fecha solicitada
            Value = (await this.MonthlyValuesAsync())
                .AsParallel<CurrencyModel>()
                .Where<CurrencyModel>(predicate: Model => Model.Date == Date)
                .FirstOrDefault<CurrencyModel>();

            // Si no hay un valor exacto, retornar el último disponible antes de la fecha
            if (string.IsNullOrEmpty(Value?.ToString())
                ||
                string.IsNullOrWhiteSpace(Value?.ToString()))
            {
                Value = (await this.MonthlyValuesAsync())
                    .AsParallel<CurrencyModel>()
                    .Where<CurrencyModel>(predicate: Model => Model.Date < Date)
                    .OrderByDescending<CurrencyModel, DateOnly>(keySelector: Model => Model.Date)
                    .FirstOrDefault<CurrencyModel>();
            }

            // Si aún no hay valores, calcular el promedio o devolver un valor por defecto
            if (string.IsNullOrEmpty(Value?.ToString())
                ||
                string.IsNullOrWhiteSpace(Value?.ToString()))
            {
                Value = new CurrencyModel
                {
                    ID = 0,
                    Date = Date,
                    Currency = (await this.MonthlyValuesAsync()).Any<CurrencyModel>() ? (await this.MonthlyValuesAsync()).Average<CurrencyModel>(selector: Model => Model.Currency) : throw new Exception(message: "No es posible obtener el valor de la divisa.")
                };
            }

            return Value;
        }
        #endregion



        #region Conversion
        public async Task<uint> ConversionInChileanPesosAsync(DateOnly Date,
                                                              float AmountOfCurrency)
        {
            #region Variables
            float Currency;
            double CurrencyConversion;
            uint Pesos;
            #endregion

            Currency = (await this.DailyValueAsync(Date: Date)).Currency;

            CurrencyConversion = await Task.Run<double>(function: () => Math.Truncate(d: AmountOfCurrency * Currency));

            Pesos = await Task.Run<uint>(function: () => Convert.ToUInt32(value: CurrencyConversion));

            return Pesos;
        }
        #endregion



        #region Mathematics
        public async Task<StatisticsModel> GetStatisticsAsync()
        {
            return await Task.Run<StatisticsModel>(function: async () => new StatisticsModel()
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
                Variance = await Statistics.VarianceAsync(Values: this.CurrencyList.Select<CurrencyModel, float>(selector: Variance => Variance.Currency).ToArray<float>())
            });
        }
        #endregion



        #region HTML
        protected async Task<string> GetHtmlContentAsync()
        {
            using HttpClient client = new HttpClient();

            return await client.GetStringAsync(requestUri: this.Url);
        }

        protected async Task<Dictionary<byte, float[]>> ExtractValuesAsync(string htmlContent,
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
            if (string.IsNullOrEmpty(value: htmlContent)
                ||
                string.IsNullOrWhiteSpace(value: htmlContent))
            {
                throw new ArgumentNullException(message: $"El parámetro {nameof(htmlContent)} no puede ser nulo o estar vacío.",
                                                paramName: nameof(htmlContent));
            }
            
            if (string.IsNullOrEmpty(value: tableId)
                ||
                string.IsNullOrWhiteSpace(value: tableId))
            {
                throw new ArgumentException(message: "El ID de la tabla no puede estar vacío.",
                                            paramName: nameof(tableId));
            }
            #endregion

            // Regex para encontrar la tabla con el ID dinámico
            tablePattern = $@"<table[^>]*id=""{Regex.Escape(str: tableId)}""[^>]*>(.*?)<\/table>";
            tableMatch = await Task.Run<Match>(function: () => Regex.Match(input: htmlContent, pattern: tablePattern, options: RegexOptions.Singleline));

            if (!tableMatch.Success)
            {
                throw new Exception(message: $"No se encontró la tabla con ID '{tableId}'.");
            }

            tableHtml = tableMatch.Groups[1].Value;

            // Regex para las filas de la tabla
            rowPattern = @"<tr>(.*?)<\/tr>";
            rowMatches = await Task.Run<MatchCollection>(function: () => Regex.Matches(input: tableHtml, pattern: rowPattern, options: RegexOptions.Singleline));

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
                cellMatches = await Task.Run<MatchCollection>(function: () => Regex.Matches(input: rowHtml, pattern: cellPattern, options: RegexOptions.Singleline));

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
                                .Replace(oldValue: ".", newValue: "")   // Eliminar puntos
                                .Replace(oldValue: ",", newValue: "."); // Cambiar comas por puntos
                            
                            #region GuardarValores
                            if (float.TryParse(s: Value, style: NumberStyles.Float, provider: CultureInfo.InvariantCulture, result: out float ufValue))
                            {
                                await Task.Run<float>(function: () => Values[i - 1] = ufValue);
                            }
                            else
                            {
                                await Task.Run<float>(function: () => Values[i - 1] = float.NaN);
                            }
                            #endregion
                        }

                        await Task.Run<float[]>(function: () => Data[day] = Values);
                    }
                }
            }

            return Data;
        }
        #endregion

        #region Transform
        protected async Task<TModel[]> TransformToModelsAsync<TModel>(Dictionary<byte, float[]> Data,
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

                        model = await Task.Run(function: () => modelFactory(
                            new DateOnly(year: this.Year, month: month, day: day),
                            value
                        ));

                        await Task.Run(action: () => ModelList.Add(item: model));
                    }
                }
            }

            return await Task.Run<TModel[]>(function: () => ModelList.ToArray());
        }

        protected async Task<CurrencyModel[]> TransformToCurrencyModelsAsync(Dictionary<byte, float[]> CurrencyData)
        {
            return await this.TransformToModelsAsync(Data: CurrencyData, modelFactory: (date, value) => new CurrencyModel
            {
                ID = uint.Parse(s: date.ToString(format: "yyyyMMdd")),
                Date = date,
                Currency = value
            });
        }
        #endregion
    }
}