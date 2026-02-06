using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.App.Context.Tool
{
    public static class Extract
    {
        #region Constructor Method
        static Extract()
        {

        }
        #endregion



        public static async Task<Dictionary<byte, float[]>> ValuesAsync(string htmlContent,
                                                                        string tableId)
        {
            #region Dictionary
            Dictionary<byte, float[]> Data;
            #endregion

            #region Exception
            ArgumentNullException.ThrowIfNullOrWhiteSpace(argument: htmlContent);

            ArgumentNullException.ThrowIfNullOrWhiteSpace(argument: tableId);
            #endregion

            Data = await OrganizeTheDataObtainedAsync(
                RowMatches: GetTableData(
                    htmlContent: htmlContent,
                    tableId: tableId
                )
            );

            return Data;
        }

        private static MatchCollection GetTableData(string htmlContent,
                                                    string tableId)
        {
            #region Variables
            string tablePattern;
            string tableHtml;
            string rowPattern;
            #endregion

            #region Match
            Match tableMatch;
            MatchCollection rowMatches;
            #endregion

            // Regex para encontrar la tabla con el ID dinámico
            tablePattern = $@"<table[^>]*id=""{Regex.Escape(str: tableId)}""[^>]*>(.*?)<\/table>";
            tableMatch = Regex.Match(
                input: htmlContent,
                pattern: tablePattern,
                options: RegexOptions.Singleline
            );

            #region Exception
            ArgumentOutOfRangeException.ThrowIfEqual<bool>(
                value: tableMatch.Success,
                other: false
            );
            #endregion

            tableHtml = tableMatch.Groups[1].Value;

            // Regex para las filas de la tabla
            rowPattern = @"<tr>(.*?)<\/tr>";
            rowMatches = Regex.Matches(
                input: tableHtml,
                pattern: rowPattern,
                options: RegexOptions.Singleline
            );

            return rowMatches;
        }

        private static async Task<Dictionary<byte, float[]>> OrganizeTheDataObtainedAsync(MatchCollection RowMatches)
        {
            #region Objects
            object LockObject;
            Dictionary<byte, float[]> Data;
            #endregion

            #region Init
            LockObject = new object();
            Data = new Dictionary<byte, float[]>();
            #endregion

            await Parallel.ForEachAsync<Match>(
                source: RowMatches,
                parallelOptions: new ParallelOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount,
                    TaskScheduler = TaskScheduler.Current
                },
                body: async (rowMatch, cancellationToken) =>
                {
                    #region Variables
                string rowHtml;
                string cellPattern;
                    #endregion

                    #region Collection
                    MatchCollection cellMatches;
                    #endregion

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

                            lock (LockObject)
                            {
                                Data[day] = Values;
                            }
                        }
                    }
                }
            );

            return Data;
        }
    }
}