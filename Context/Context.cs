using System.Collections.Generic;
using System.Globalization;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IndicadorChileAPI.Context
{
    public abstract class Context
    {
        #region Variables
        private readonly string Url;
        private readonly ushort Year;
        private readonly ushort? Month;
        #endregion



        #region ConstructorMethod
        public Context(string Url,
                       ushort Year,
                       ushort? Month)
            : base()
        {
            if (Year > DateTime.Now.Year)
            {
                throw new ArgumentException(message: $"The year must not be greater than {DateTime.Now.Year}.", paramName: nameof(Year));
            }
            else
            if (Month < 1
                ||
                Month > 12)
            {
                throw new ArgumentException(message: "The month number must be between 1 and 12.", paramName: nameof(Month));
            }
            else
            if (Year == DateTime.Now.Year
                &&
                Month > DateTime.Now.Month)
            {
                throw new Exception(message: "The query cannot be performed.");
            }

            this.Url = Url;
            this.Year = Year;
            this.Month = Month;
        }
        #endregion



        #region DeconstructorMethod
        ~Context()
        {

        }
        #endregion



        #region Getters
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

            Dictionary<byte, float[]> Data = new Dictionary<byte, float[]>();

            #region Match
            Match tableMatch;
            MatchCollection rowMatches;
            #endregion

            if (string.IsNullOrWhiteSpace(value: tableId)
                ||
                string.IsNullOrEmpty(value: tableId))
            {
                throw new ArgumentException(message: "El ID de la tabla no puede estar vacío.", paramName: nameof(tableId));
            }

            // Regex para encontrar la tabla con el ID dinámico
            tablePattern = $@"<table[^>]*id=""{Regex.Escape(tableId)}""[^>]*>(.*?)<\/table>";
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
                        float[] Values = new float[12];

                        for (byte i = 1; i < cellMatches.Count; i++)
                        {
                            string Value = string.Empty;
                            
                            Value = cellMatches[i].Groups[1].Value
                                .Trim()
                                .Replace(".", "")   // Eliminar puntos
                                .Replace(",", "."); // Cambiar comas por puntos
                            
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

        protected async Task<TModel[]> TransformToModelsAsync<TModel>(Dictionary<byte, float[]> Data, Func<DateOnly, float, TModel> modelFactory)
        {
            List<TModel> models = new List<TModel>();

            foreach (var (day, values) in Data)
            {
                for (byte month = 1; month <= values.Length; month++)
                {
                    if (day > 0 && day <= DateTime.DaysInMonth(year: this.Year, month: month))
                    {
                        float value = values[month - 1];

                        TModel model = await Task.Run(function: () => modelFactory(
                            new DateOnly(year: this.Year, month: month, day: day),
                            value
                        ));

                        await Task.Run(action: () => models.Add(item: model));
                    }
                }
            }

            return models.ToArray();
        }
    }
}