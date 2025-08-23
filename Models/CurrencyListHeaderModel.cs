using System;
using System.ComponentModel.DataAnnotations;

namespace IndicadorChileAPI.Models
{
    public record CurrencyListHeaderModel
    {
        [
            DataType(
                dataType: DataType.Date
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                GroupName = nameof(CurrencyListHeaderModel),
                Name = "Fecha consulta",
                Order = 1,
                Prompt = "",
                ShortName = ""
            ),
            Required(
                AllowEmptyStrings = false
            )
        ]
        public required DateOnly ConsultationDate { get; set; }

        [
            DataType(
                dataType: DataType.Time
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                GroupName = nameof(CurrencyListHeaderModel),
                Name = "Hora consulta",
                Order = 2,
                Prompt = "",
                ShortName = ""
            ),
            Required(
                AllowEmptyStrings = false
            )
        ]
        public required TimeOnly ConsultationTime { get; set; }

        [
            DataType(
                dataType: DataType.Currency
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                GroupName = nameof(CurrencyListHeaderModel),
                Name = "Año",
                Order = 3,
                Prompt = "Ingrese aquí el año.",
                ShortName = "Año"
            ),
            Required(
                AllowEmptyStrings = false
            )
        ]
        public required ushort Year { get; set; }

        [
            DataType(
                dataType: DataType.Text
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                GroupName = nameof(CurrencyListHeaderModel),
                Name = "Month name",
                Order = 4,
                Prompt = "",
                ShortName = "Mes"
            ),
            MaxLength(
                length: 10
            ),
            MinLength(
                length: 5
            ),
            StringLength(
                maximumLength: 10,
                MinimumLength = 5
            )
        ]
        public string? MonthName { get; set; }
        public required CurrencyModel[] List { get; set; }
    }
}