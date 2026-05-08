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
        internal MatchCollection GetData()
        {
            #region Variables
            string rowPattern;
            #endregion

            #region Match
            MatchCollection rowMatches;
            #endregion

            // Regex para las filas de la tabla
            rowPattern = @"<tr>(.*?)<\/tr>";
            rowMatches = Regex.Matches(
                input: this.GetHtml(),
                pattern: rowPattern,
                options: RegexOptions.Singleline
            );

            return rowMatches;
        }

        #region Table
        private string GetHtml()
        {
            #region Objects
            Result<Match> result = GetMatch();
            #endregion

            result = GetMatch();

            if (result.IsSuccess)
            {
                #region Variables
                string tableHtml;
                #endregion

                tableHtml = result.Value.Groups[1].Value;

                return tableHtml;
            }

            return result.Error;
        }

        private Result<Match> GetMatch()
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
                return Result<Match>.Failure($"La variable {nameof(tableMatch.Success)} no puede ser falso.");
            }

            return Result<Match>.Success(Value: tableMatch);
        }
        #endregion
        #endregion
    }
}