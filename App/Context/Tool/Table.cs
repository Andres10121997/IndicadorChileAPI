using IndicadorChileAPI.App.DTO;
using IndicadorChileAPI.App.DTO.HTML;
using IndicadorChileAPI.Utils;
using System.Text.RegularExpressions;

namespace IndicadorChileAPI.App.Context.Tool
{
    internal static class Table
    {
        #region Get
        internal static Result<MatchCollection, ResultErrorDto> GetRows(HtmlDto Html)
        {
            #region Objects
            Result<string, ResultErrorDto> htmlResult;
            #endregion

            htmlResult = GetHtml(Html: Html);

            if (!htmlResult.IsSuccess)
            {
                return Result<MatchCollection, ResultErrorDto>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Table),
                        MethodName = nameof(GetRows),
                        VariableName = nameof(htmlResult.IsSuccess),
                        Description = $"La variable {nameof(htmlResult.IsSuccess)} no puede ser {false}."
                    }
                );
            }

            if (htmlResult.Value == null)
            {
                return Result<MatchCollection, ResultErrorDto>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Table),
                        MethodName = nameof(GetRows),
                        VariableName = nameof(htmlResult.Value),
                        Description = $"La variable {nameof(htmlResult.Value)} no puede ser {null}."
                    }
                );
            }

            return Result<MatchCollection, ResultErrorDto>.Success(Value: RowMatches(Input: htmlResult.Value));
        }

        private static MatchCollection RowMatches(string Input)
        {
            #region Variables
            string rowPattern;
            #endregion

            #region Collection
            MatchCollection rowMatches;
            #endregion

            // Regex para las filas de la tabla
            rowPattern = @"<tr>(.*?)<\/tr>";
            rowMatches = Regex.Matches(
                input: Input,
                pattern: rowPattern,
                options: RegexOptions.Singleline
            );

            return rowMatches;
        }

        #region Table
        private static Result<string, ResultErrorDto> GetHtml(HtmlDto Html)
        {
            #region Variables
            string tableHtml;
            #endregion

            #region Objects
            Result<Match, ResultErrorDto> tableMatchResult;
            #endregion

            tableMatchResult = GetMatchResult(Html: Html);

            if (!tableMatchResult.IsSuccess)
            {
                return Result<string, ResultErrorDto>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Table),
                        MethodName = nameof(GetHtml),
                        VariableName = nameof(tableMatchResult.IsSuccess),
                        Description = $"La variable {nameof(tableMatchResult.IsSuccess)} no puede ser {false}."
                    }
                );
            }

            if (tableMatchResult.Value == null)
            {
                return Result<string, ResultErrorDto>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Table),
                        MethodName = nameof(GetHtml),
                        VariableName = nameof(tableMatchResult.Value),
                        Description = $"La variable {nameof(tableMatchResult.Value)} no puede ser {null}."
                    }
                );
            }

            tableHtml = tableMatchResult.Value.Groups[1].Value;

            if (string.IsNullOrWhiteSpace(value: tableHtml))
            {
                return Result<string, ResultErrorDto>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Table),
                        MethodName = nameof(GetHtml),
                        VariableName = nameof(tableHtml),
                        Description = $"La variable {nameof(tableHtml)} no puede ser {null}, {nameof(string.Empty)} o 'White Space'."
                    }
                );
            }

            return Result<string, ResultErrorDto>.Success(Value: tableHtml);
        }

        private static Result<Match, ResultErrorDto> GetMatchResult(HtmlDto Html)
        {
            #region Object
            Match tableMatch;
            #endregion

            // Regex para encontrar la tabla con el ID dinámico
            tableMatch = Regex.Match(
                input: Html.Content,
                pattern: Html.Table.Pattern,
                options: RegexOptions.Singleline
            );

            if (tableMatch.Equals(obj: null))
            {
                return Result<Match, ResultErrorDto>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Table),
                        MethodName = nameof(GetHtml),
                        VariableName = nameof(tableMatch),
                        Description = $"La variable {nameof(tableMatch)} no puede ser {false}."
                    }
                );
            }

            if (tableMatch.Success.Equals(obj: false))
            {
                return Result<Match, ResultErrorDto>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Table),
                        MethodName = nameof(GetMatchResult),
                        VariableName = nameof(tableMatch.Success),
                        Description = $"La variable {nameof(tableMatch.Success)} no puede ser {false}."
                    }
                );
            }

            return Result<Match, ResultErrorDto>.Success(Value: tableMatch);
        }
        #endregion
        #endregion
    }
}