using IndicadorChileAPI.App.DTO;
using IndicadorChileAPI.Utils;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IndicadorChileAPI.App.Context.Tool
{
    internal static class Data<TCurrency>
        where TCurrency : struct, IFloatingPointConstants<TCurrency>
    {
        #region Objects
        private static object lockObject;
        #endregion

        #region Collections
        private static Dictionary<byte, TCurrency[]> data;
        #endregion



        #region Constructor Method
        static Data()
        {
            #region Objects
            lockObject = new object();
            #endregion

            #region Collections
            data = new Dictionary<byte, TCurrency[]>();
            #endregion
        }
        #endregion



        internal static async Task<Dictionary<byte, TCurrency[]>> GetAsync(MatchCollection RowMatches)
        {
            await Parallel.ForEachAsync<Match>(
                source: RowMatches,
                parallelOptions: VarGlobal.ParallelForEachOptions,
                body: async (RowMatch, CancellationToken) =>
                {
                    #region Objects
                    Result<MatchCollection, ResultErrorDto> cellResult;
                    #endregion

                    cellResult = Cell(RowMatch: RowMatch);

                    if (cellResult.IsSuccess)
                    {
                        if (cellResult.Value != null)
                        {
                            Parse(CellMatches: cellResult.Value);
                        }
                    }
                }
            );

            return data;
        }

        private static Result<MatchCollection, ResultErrorDto> Cell(Match RowMatch)
        {
            #region Variables
            string cellPattern;
            #endregion

            #region Collection
            MatchCollection cellMatches;
            #endregion

            // Regex para las celdas (<th> y <td>)
            cellPattern = @"<t[hd][^>]*>(.*?)<\/t[hd]>";
            cellMatches = Regex.Matches(
                input: RowHtml(RowMatch: RowMatch),
                pattern: cellPattern,
                options: RegexOptions.Singleline
            );

            if (cellMatches.Equals(obj: null))
            {
                return Result<MatchCollection, ResultErrorDto>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Data<TCurrency>),
                        MethodName = nameof(Cell),
                        VariableName = nameof(cellMatches),
                        Description = $"La variable {nameof(cellMatches)} no puede ser {null}."
                    }
                );
            }

            if (cellMatches.Count < 0)
            {
                return Result<MatchCollection, ResultErrorDto>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Data<TCurrency>),
                        MethodName = nameof(Cell),
                        VariableName = nameof(cellMatches.Count),
                        Description = $"La cantidad de datos de la lista {nameof(cellMatches.Count)} no puede ser inferior a 0."
                    }
                );
            }

            if (cellMatches.Count.Equals(obj: 0))
            {
                return Result<MatchCollection, ResultErrorDto>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Data<TCurrency>),
                        MethodName = nameof(Cell),
                        VariableName = nameof(cellMatches.Count),
                        Description = $"La cantidad de datos de la lista {nameof(cellMatches.Count)} no puede ser 0."
                    }
                );
            }

            return Result<MatchCollection, ResultErrorDto>.Success(Value: cellMatches);
        }

        private static string RowHtml(Match RowMatch)
        {
            #region Variables
            string rowHtml;
            #endregion

            rowHtml = RowMatch.Groups[1].Value;

            return rowHtml;
        }

        private static void Parse(MatchCollection CellMatches)
        {
            // Primera celda: el día
            if (byte.TryParse(s: Regex.Replace(input: CellMatches[0].Groups[1].Value,
                                               pattern: @"\D",
                                               replacement: ""),
                              result: out byte day))
            {
                Set(
                    CellMatches: CellMatches,
                    Day: day
                );
            }
        }

        private static void Set(byte Day,
                                MatchCollection CellMatches)
        {
            #region Collections
            TCurrency[] values;
            #endregion

            values = new TCurrency[12];

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

                if (TCurrency.TryParse(s: value,
                                       style: NumberStyles.Number,
                                       provider: CultureInfo.InvariantCulture,
                                       result: out var currencyValue))
                {
                    values[i - 1] = currencyValue;
                }
                else
                {
                    values[i - 1] = TCurrency.Zero;
                }
            }

            lock (lockObject)
            {
                data[Day] = values;
            }
        }
    }
}