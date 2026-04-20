using API.App.DTO.HTML;
using System;
using System.Text.RegularExpressions;

namespace API.App.Context.Tool
{
    internal static class Table
    {
        static Table()
        {
            
        }



        internal static MatchCollection GetData(HtmlDto Html)
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
                input: GetHtml(Html: Html),
                pattern: rowPattern,
                options: RegexOptions.Singleline
            );

            return rowMatches;
        }

        private static string GetHtml(HtmlDto Html)
        {
            #region Variables
            string tableHtml;
            #endregion

            #region Object
            Match tableMatch;
            TableDto table;
            #endregion

            table = new TableDto
            {
                ID = Html.Table.ID,
                Pattern = $@"<table[^>]*id=""{Regex.Escape(str: Html.Table.ID)}""[^>]*>(.*?)<\/table>"
            };

            // Regex para encontrar la tabla con el ID dinámico
            tableMatch = Regex.Match(
                input: Html.Content,
                pattern: table.Pattern,
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
    }
}