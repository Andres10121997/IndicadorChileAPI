using API.App.DTO;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.App.Context.Tool
{
    internal static class Data<T>
        where T : struct, IFloatingPoint<T>
    {
        #region Objects
        private static object lockObject;
        #endregion

        #region Collections
        private static Dictionary<byte, T[]> data;
        #endregion



        #region Constructor Method
        static Data()
        {
            #region Objects
            lockObject = new object();
            #endregion

            #region Collections
            data = new Dictionary<byte, T[]>();
            #endregion
        }
        #endregion



        internal static async Task<Dictionary<byte, T[]>> GetAsync(MatchCollection RowMatches)
        {
            await Parallel.ForEachAsync<Match>(
                source: RowMatches,
                parallelOptions: VarGlobal<T>.ParallelForEachOptions,
                body: async (RowMatch, CancellationToken) =>
                {
                    #region Objects
                    Result<MatchCollection> cellResult;
                    #endregion

                    cellResult = Cell(RowMatch: RowMatch);

                    if (cellResult.IsSuccess)
                    {
                        Parse(CellMatches: cellResult.Value);
                    }
                }
            );

            return data;
        }

        private static Result<MatchCollection> Cell(Match RowMatch)
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

            if (cellMatches.Count < 0)
            {
                return Result<MatchCollection>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Extract<T>),
                        MethodName = nameof(Cell),
                        VariableName = nameof(cellMatches.Count),
                        Description = $"La cantidad de datos de la lista {nameof(cellMatches.Count)} no puede ser inferior a 0."
                    }
                );
            }

            if (cellMatches.Count == 0)
            {
                return Result<MatchCollection>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Extract<T>),
                        MethodName = nameof(Cell),
                        VariableName = nameof(cellMatches.Count),
                        Description = $"La cantidad de datos de la lista {nameof(cellMatches.Count)} no puede ser 0."
                    }
                );
            }

            return Result<MatchCollection>.Success(Value: cellMatches);
        }

        private static void Parse(MatchCollection CellMatches)
        {
            // Primera celda: el día
            if (byte.TryParse(s: Regex.Replace(input: CellMatches[0].Groups[1].Value,
                                               pattern: @"\D",
                                               replacement: ""),
                              result: out byte day))
            {
                Set(CellMatches: CellMatches, Day: day);
            }
        }

        private static void Set(MatchCollection CellMatches,
                                    byte Day)
        {
            #region Collections
            T[] values;
            #endregion

            values = new T[12];

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

                switch (T.TryParse(s: value,
                                      style: NumberStyles.Number,
                                      provider: CultureInfo.InvariantCulture,
                                      result: out var currencyValue))
                {
                    case true:
                        values[i - 1] = currencyValue;
                        break;
                    case false:
                        values[i - 1] = T.Zero;
                        break;
                }
            }

            lock (lockObject)
            {
                data[Day] = values;
            }
        }
    }
}