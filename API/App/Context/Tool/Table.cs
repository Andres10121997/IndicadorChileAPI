using API.App.DTO;
using API.App.DTO.HTML;
using System.Text.RegularExpressions;

namespace API.App.Context.Tool
{
    internal class Table
    {
        #region Objects
        private HtmlDto html;
        #endregion



        #region Constructor Method
        internal Table(HtmlDto Html)
        {
            this.html = Html;
        }
        #endregion



        #region Get
        internal Result<MatchCollection> GetData()
        {
            #region Variables
            string rowPattern;
            #endregion

            #region Objects
            Result<string> result;
            #endregion

            #region Collection
            MatchCollection rowMatches;
            #endregion

            result = this.GetHtml();

            if (!result.IsSuccess)
            {
                return Result<MatchCollection>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Table),
                        MethodName = nameof(this.GetData),
                        VariableName = nameof(result.IsSuccess),
                        Description = $"La variable {nameof(result.IsSuccess)} no puede ser {false}.",
                        OtherErrors = new[]
                        {
                            result.Error
                        }
                    }
                );
            }

            // Regex para las filas de la tabla
            rowPattern = @"<tr>(.*?)<\/tr>";
            rowMatches = Regex.Matches(
                input: result.Value,
                pattern: rowPattern,
                options: RegexOptions.Singleline
            );

            return Result<MatchCollection>.Success(Value: rowMatches);
        }

        #region Table
        private Result<string> GetHtml()
        {
            #region Variables
            string tableHtml;
            #endregion

            #region Objects
            Result<Match> result;
            #endregion

            result = this.GetMatchResult();

            if (!result.IsSuccess)
            {
                return Result<string>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Table),
                        MethodName = nameof(this.GetHtml),
                        VariableName = nameof(result.IsSuccess),
                        Description = $"La variable {nameof(result.IsSuccess)} no puede ser {false}.",
                        OtherErrors = new[]
                        {
                            result.Error
                        }
                    }
                );
            }

            tableHtml = result.Value.Groups[1].Value;

            return Result<string>.Success(Value: tableHtml);
        }

        private Result<Match> GetMatchResult()
        {
            #region Object
            Match tableMatch;
            #endregion

            // Regex para encontrar la tabla con el ID dinámico
            tableMatch = Regex.Match(
                input: this.html.Content,
                pattern: this.html.Table.Pattern,
                options: RegexOptions.Singleline
            );
            
            if (tableMatch.Success == false)
            {
                return Result<Match>.Failure(
                    new ResultErrorDto()
                    {
                        ClassName = nameof(Table),
                        MethodName = nameof(this.GetMatchResult),
                        VariableName = nameof(tableMatch.Success),
                        Description = $"La variable {nameof(tableMatch.Success)} no puede ser {false}."
                    }
                );
            }

            return Result<Match>.Success(Value: tableMatch);
        }
        #endregion
        #endregion
    }
}