using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace IndicadorChileAPI.App.DTO.Currency
{
    public sealed record CurrencyDTO<TCurrency>
        where TCurrency : struct, IFloatingPointConstants<TCurrency>
    {
        #region Field
        [
            DataType(
                dataType: DataType.Custom
            ),
            Key,
            Required
        ]
        public required Guid ID { get; init; }

        [
            DataType(
                dataType: DataType.Date
            ),
            Required
        ]
        public required DateOnly Date { get; init; }

        [
            DataType(
                dataType: DataType.Text
            ),
            Required
        ]
        public required string WeekdayName { get; init; }

        [
            DataType(
                dataType: DataType.Currency
            ),
            Required
        ]
        public required TCurrency Currency { get; init; }
        #endregion
    }
}