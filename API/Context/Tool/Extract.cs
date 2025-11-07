using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.Context.Tool
{
    public static class Extract
    {
        #region Constructor Method
        static Extract()
        {

        }
        #endregion



        #region Values
        public static Dictionary<byte, float[]> Values(string htmlContent,
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
                            string Value;
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

        public static async Task<Dictionary<byte, float[]>> ValuesAsync(string htmlContent,
                                                                        string tableId)
        {
            return await Task.Run<Dictionary<byte, float[]>>(
                function: () => Values(
                    htmlContent: htmlContent,
                    tableId: tableId
                )
            );
        }
        #endregion
    }
}