using API.App.DTO;
using API.App.DTO.HTML;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.App.Context.Tool
{
    public static class Extract<T>
        where T : struct, IFloatingPoint<T>
    {
        #region Constructor Method
        static Extract()
        {
            
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

            data = await Data<T>.GetAsync(RowMatches: RowMatches);

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

        public static Result<MatchCollection> Cell(Match RowMatch)
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
    }
}