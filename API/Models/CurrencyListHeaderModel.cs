using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace API.Models
{
    public sealed record CurrencyListHeaderModel
    {
        #region Property
        [
            DataType(
                dataType: DataType.Date
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                GroupName = nameof(CurrencyListHeaderModel),
                Name = "Fecha de la consulta",
                Order = 1,
                Prompt = "",
                ShortName = "Fecha consulta"
            ),
            Editable(
                allowEdit: false
            ),
            Required(
                AllowEmptyStrings = false
            )
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
                GroupName = nameof(CurrencyListHeaderModel),
                Name = "Hora de la consulta consulta",
                Order = 2,
                Prompt = "",
                ShortName = "Hora consulta"
            ),
            Editable(
                allowEdit: false
            ),
            Required(
                AllowEmptyStrings = false
            )
        ]
        public required TimeOnly ConsultationTime { get; init; }

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
            Editable(
                allowEdit: false
            ),
            Required(
                AllowEmptyStrings = false
            )
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
                GroupName = nameof(CurrencyListHeaderModel),
                Name = "Nombre del mes",
                Order = 4,
                Prompt = "",
                ShortName = "Mes"
            ),
            Editable(
                allowEdit: false
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
        public string? MonthName
        {
            get => field;
            init
            {
                string[] Months = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;

                if (value is not null
                    &&
                    !Months.Contains(value: value, StringComparer.OrdinalIgnoreCase))
                {
                    new ValidationResult(
                        errorMessage: "Error",
                        memberNames: new[]
                        {
                            nameof(MonthName)
                        }
                    );
                }

                field = value;
            }
        }

        [
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                GroupName = nameof(CurrencyListHeaderModel),
                Name = "Lista de divisas",
                Order = 5,
                Prompt = "",
                ShortName = "Divisas"
            ),
            Editable(
                allowEdit: false
            ),
            MinLength(
                length: 1
            ),
            Required
        ]
        public required CurrencyModel[] Currencies { get; init; }
        #endregion
    }
}