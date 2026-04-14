using System;

namespace API.App.Record.Currency
{
    public sealed record CurrencyDto
    {
        #region Field
        public required Guid ID { get; init; }
        public required DateOnly Date { get; init; }
        public required string WeekdayName { get; init; }
        public required float Currency { get; init; }
        #endregion
    }
}