using API.App.DTO.HTML;
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
            #region Objects
            lockObject = new object();
            #endregion

            #region Collections
            data = new Dictionary<byte, float[]>();
            #endregion
        }
        #endregion



        #region Field
        public static Dictionary<byte, float[]> Data
        {
            get => data;
            set => data = value;
        }
        #endregion



        public static async Task<Dictionary<byte, float[]>> ValuesAsync(HtmlDto Html)
        {
            #region Objects
            Table table;
            #endregion

            #region Collection
            MatchCollection rows;
            #endregion

            table = new Table(Html: Html);

            rows = table.GetData();

            Data = await OrganizeTheDataObtainedAsync(RowMatches: rows);

            return Data;
        }



        private static async Task<Dictionary<byte, float[]>> OrganizeTheDataObtainedAsync(MatchCollection RowMatches)
        {
            await Parallel.ForEachAsync<Match>(
                source: RowMatches,
                parallelOptions: Utils.ParallelForEachOptions,
                body: async (RowMatch, CancellationToken) => ParseData(CellMatches: Cell(RowMatch))
            );

            return Data;
        }

        private static MatchCollection Cell(Match RowMatch)
        {
            #region Variables
            string rowHtml;
            string cellPattern;
            #endregion

            #region Collection
            MatchCollection cellMatches;
            #endregion

            rowHtml = RowMatch.Groups[1].Value;

            // Regex para las celdas (<th> y <td>)
            cellPattern = @"<t[hd][^>]*>(.*?)<\/t[hd]>";
            cellMatches = Regex.Matches(
                input: rowHtml,
                pattern: cellPattern,
                options: RegexOptions.Singleline
            );

            #region Exception
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(
                value: cellMatches.Count,
                other: 0
            );
            #endregion

            return cellMatches;
        }

        private static void ParseData(MatchCollection CellMatches)
        {
            // Primera celda: el día
            if (byte.TryParse(s: Regex.Replace(input: CellMatches[0].Groups[1].Value,
                                               pattern: @"\D",
                                               replacement: ""),
                              result: out byte day))
            {
                SetData(CellMatches: CellMatches, Day: day);
            }
        }

        private static void SetData(MatchCollection CellMatches,
                                    byte Day)
        {
            #region Collections
            float[] values;
            #endregion

            values = new float[12];

            for (byte i = 1; i < CellMatches.Count; i++)
            {
                #region Variables
                string value;
                #endregion

                value = CellMatches[i].Groups[1].Value
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

                switch (float.TryParse(s: value,
                                       style: NumberStyles.Number,
                                       provider: CultureInfo.InvariantCulture,
                                       result: out float currencyValue))
                {
                    case true:
                        values[i - 1] = currencyValue;
                        break;
                    case false:
                        values[i - 1] = float.NaN;
                        break;
                }
            }

            lock (lockObject)
            {
                Data[Day] = values;
            }
        }
    }
}