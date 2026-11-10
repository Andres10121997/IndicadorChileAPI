using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace IndicadorChileAPI.App.DTO.Currency
{
    public sealed record CurrencyHeaderDto<TCurrency>
        where TCurrency : struct, IFloatingPointConstants<TCurrency>
    {
        #region Field
        [
            DataType(
                dataType: DataType.Date
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                // GroupName = nameof(CurrencyHeaderDto<TCurrency>),
                Name = "Fecha de la consulta",
                Order = 1,
                Prompt = "",
                ShortName = "Consulta"
            ),
            Required
        ]
        public required DateOnly ConsultationDate { get; init; }

        [
            DataType(
                dataType: DataType.Time
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                // GroupName = nameof(CurrencyHeaderDto<TCurrency>),
                Name = "Hora de la consulta",
                Order = 2,
                Prompt = "",
                ShortName = "Consulta"
            ),
            Required
        ]
        public required TimeOnly ConsultationTime { get; init; }

        [
            DataType(
                dataType: DataType.Url
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                // GroupName = nameof(CurrencyHeaderDto<TCurrency>),
                Name = "URL",
                Order = 3,
                Prompt = "",
                ShortName = "URL"
            ),
            Required
        ]
        public required Uri Url { get; init; }

        [
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                // GroupName = nameof(CurrencyHeaderDto<TCurrency>),
                Name = "Año",
                Order = 4,
                Prompt = "",
                ShortName = "Año"
            ),
            Required
        ]
        public required ushort Year { get; init; }

        [
            DataType(
                dataType: DataType.Text
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                // GroupName = nameof(CurrencyHeaderDto<TCurrency>),
                Name = "Mes",
                Order = 5,
                Prompt = "",
                ShortName = "Mes"
            )
        ]
        public required string MonthName { get; init; }

        [
            DataType(
                dataType: DataType.Text
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                // GroupName = nameof(CurrencyHeaderDto<TCurrency>),
                Name = "Nombre divisa",
                Order = 6,
                Prompt = "",
                ShortName = "Divisa"
            ),
            Required
        ]
        public required string CurrencyName { get; init; }

        public required CurrencyDto<TCurrency>[] Currencies { get; init; }
        #endregion
    }
}