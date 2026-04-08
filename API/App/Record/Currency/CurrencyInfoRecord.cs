using System;

namespace API.App.Record.Currency
{
    public sealed record CurrencyInfoRecord
    {
        public required string Url { get; init; }
        public required string TableId { get; init; }
        public required DateOnly StartDate { get; init; }
        public required DateOnly EndDate { get; init; }
    }
}