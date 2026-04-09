using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.App.Context.Tool
{
    public static class Extract
    {
        #region Objects
        private static object lockObject;
        #endregion

        #region Collections
        private static Dictionary<byte, float[]> data;
        #endregion



        #region Constructor Method
        static Extract()
        {
            lockObject = new object();
            data = new Dictionary<byte, float[]>();
        }
        #endregion



        #region Field
        public static object LockObject
        {
            get => lockObject;
            set => lockObject = value;
        }

        public static Dictionary<byte, float[]> Data
        {
            get => data;
            set => data = value;
        }
        #endregion



        public static async Task<Dictionary<byte, float[]>> ValuesAsync(string HtmlContent,
                                                                        string TableId)
        {
            #region Collection
            MatchCollection Rows;
            #endregion

            Rows = GetTableData(
                HtmlContent: HtmlContent,
                TableId: TableId
            );

            Data = await OrganizeTheDataObtainedAsync(RowMatches: Rows);

            return Data;
        }



        private static MatchCollection GetTableData(string HtmlContent,
                                                    string TableId)
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
            tablePattern = $@"<table[^>]*id=""{Regex.Escape(str: TableId)}""[^>]*>(.*?)<\/table>";
            tableMatch = Regex.Match(
                input: HtmlContent,
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
            await Parallel.ForEachAsync<Match>(
                source: RowMatches,
                parallelOptions: Utils.ParallelForEachOptions,
                body: async (RowMatch, CancellationToken) =>
                {
                    #region Variables
                    string RowHtml;
                    string CellPattern;
                    #endregion

                    #region Collection
                    MatchCollection cellMatches;
                    #endregion

                    RowHtml = RowMatch.Groups[1].Value;

                    // Regex para las celdas (<th> y <td>)
                    CellPattern = @"<t[hd][^>]*>(.*?)<\/t[hd]>";
                    cellMatches = Regex.Matches(
                        input: RowHtml,
                        pattern: CellPattern,
                        options: RegexOptions.Singleline
                    );

                    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(
                        value: cellMatches.Count,
                        other: 0
                    );

                    ParseAndSetData(CellMatches: cellMatches);
                }
            );

            return Data;
        }

        private static void ParseAndSetData(MatchCollection CellMatches)
        {
            #region Collections
            float[] Values;
            #endregion

            // Primera celda: el día
            if (byte.TryParse(s: Regex.Replace(input: CellMatches[0].Groups[1].Value,
                                               pattern: @"\D",
                                               replacement: ""),
                              result: out byte day))
            {
                Values = new float[12];

                for (byte i = 1; i < CellMatches.Count; i++)
                {
                    #region Variables
                    string Value;
                    #endregion

                    Value = CellMatches[i].Groups[1].Value
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
                    switch (float.TryParse(s: Value,
                                           style: NumberStyles.Number,
                                           provider: CultureInfo.InvariantCulture,
                                           result: out float currencyValue))
                    {
                        case true:
                            Values[i - 1] = currencyValue;
                            break;
                        case false:
                            Values[i - 1] = float.NaN;
                            break;
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
}