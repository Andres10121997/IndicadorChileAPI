using API.App.DTO.HTML;
using System;
using System.Text.RegularExpressions;

namespace API.App.Context.Tool
{
    internal class Table
    {
        #region Objects
        private HtmlDto html;
        #endregion



        #region Constructor Method
        public Table(HtmlDto Html)
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

        private string GetHtml()
        {
            #region Variables
            string tableHtml;
            #endregion

            #region Object
            Match tableMatch;
            #endregion

            // Regex para encontrar la tabla con el ID dinámico
            tableMatch = Regex.Match(
                input: this.html.Content,
                pattern: this.html.Table.Pattern,
                options: RegexOptions.Singleline
            );

            #region Exception
            ArgumentOutOfRangeException.ThrowIfEqual<bool>(
                value: tableMatch.Success,
                other: false
            );
            #endregion

            tableHtml = tableMatch.Groups[1].Value;

            return tableHtml;
        }
        #endregion
    }
}