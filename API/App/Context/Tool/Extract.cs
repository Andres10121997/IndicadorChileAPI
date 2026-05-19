using API.App.DTO;
using API.App.DTO.HTML;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.App.Context.Tool
{
    public static class Extract<T>
        where T : struct, IFloatingPoint<T>
    {
        #region Objects
        private static object lockObject;
        #endregion

        #region Collections
        private static Dictionary<byte, T[]> data;
        #endregion



        #region Constructor Method
        static Extract()
        {
            #region Objects
            lockObject = new object();
            #endregion

            #region Collections
            data = new Dictionary<byte, T[]>();
            #endregion
        }
        #endregion



        #region Field
        public static Dictionary<byte, T[]> Data
        {
            get => data;
            set => data = value;
        }
        #endregion



        public static async Task<Result<Dictionary<byte, T[]>>> ValuesAsync(HtmlDto Html)
        {
            #region Objects
            Table table;
            Result<Dictionary<byte, T[]>> result;
            Result<MatchCollection> rowsResult;
            #endregion

            table = new Table(Html: Html);

            rowsResult = table.GetData();

            if (!rowsResult.IsSuccess)
            {
                return Result<Dictionary<byte, T[]>>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Extract<T>),
                        MethodName = nameof(ValuesAsync),
                        VariableName = nameof(rowsResult.IsSuccess),
                        Description = $"La variable {nameof(rowsResult.IsSuccess)} no puede ser {false}",
                        OtherErrors = new[]
                        {
                            rowsResult.Error
                        }
                    }
                );
            }

            result = await OrganizeTheDataObtainedAsync(RowMatches: rowsResult.Value);

            if (!result.IsSuccess)
            {
                return Result<Dictionary<byte, T[]>>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Extract<T>),
                        MethodName = nameof(ValuesAsync),
                        VariableName = nameof(result.IsSuccess),
                        Description = $"La variable {result.IsSuccess} no puede ser {false}.",
                        OtherErrors = new[]
                        {
                            result.Error
                        }
                    }
                );
            }

            return Result<Dictionary<byte, T[]>>.Success(Value: result.Value);
        }



        private static async Task<Result<Dictionary<byte, T[]>>> OrganizeTheDataObtainedAsync(MatchCollection RowMatches)
        {
            #region Collection
            Dictionary<byte, T[]> data;
            #endregion

            await Parallel.ForEachAsync<Match>(
                source: RowMatches,
                parallelOptions: Utils.ParallelForEachOptions,
                body: async (RowMatch, CancellationToken) =>
                {
                    Result<MatchCollection> cellResult;

                    cellResult = Cell(RowMatch: RowMatch);

                    if (cellResult.IsSuccess)
                    {
                        ParseData(CellMatches: cellResult.Value);
                    }
                }
            );

            data = Data;

            if (data.Count == 0)
            {
                return Result<Dictionary<byte, T[]>>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Extract<T>),
                        MethodName = nameof(OrganizeTheDataObtainedAsync),
                        VariableName = nameof(data.Count),
                        Description = $"La cantidad de datos de la variable {data.Count} no puede ser 0."
                    }
                );
            }

            return Result<Dictionary<byte, T[]>>.Success(Value: data);
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
                Data[Day] = values;
            }
        }
    }
}