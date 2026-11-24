using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace IndicadorChileAPI.App.DTO.Currency
{
    public sealed record CurrencyHeaderDTO<TCurrency>
        where TCurrency : struct, IFloatingPointConstants<TCurrency>
    {
        #region Field
        [
            DataType(
                dataType: DataType.Date
            ),
            Required
        ]
        public required DateOnly ConsultationDate { get; init; }

        [
            DataType(
                dataType: DataType.Time
            ),
            Required
        ]
        public required TimeOnly ConsultationTime { get; init; }

        [
            DataType(
                dataType: DataType.Url
            ),
            Required
        ]
        public required Uri Url { get; init; }

        [
            Required
        ]
        public required ushort Year { get; init; }

        [
            DataType(
                dataType: DataType.Text
            ),
            Required
        ]
        public required string MonthName { get; init; }

        [
            DataType(
                dataType: DataType.Text
            ),
            Required
        ]
        public required string CurrencyName { get; init; }

        [
            Required
        ]
        public required CurrencyDTO<TCurrency>[] Currencies { get; init; }
        #endregion
    }
}