using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IndicadorChileAPI.Models
{
    public sealed record CurrencyListHeaderModel : IValidatableObject
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
        public string? MonthName { get; init; }

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
            MinLength(
                length: 1
            ),
            Required
        ]
        public required CurrencyModel[] Currencies { get; init; }
        #endregion



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Currencies == null
                ||
                this.Currencies.Equals(null))
            {
                yield return new ValidationResult(
                    errorMessage: "No puede ser nulo.",
                    memberNames: new[]
                    {
                        nameof(this.Currencies)
                    }
                );
            }
            else
            if (this.Currencies.Length == 0
                ||
                this.Currencies.Length.Equals(obj: 0))
            {
                yield return new ValidationResult(
                    errorMessage: "Debe de haber al menos un registro.",
                    memberNames: new[]
                    {
                        nameof(this.Currencies)
                    }
                );
            }
        }
    }
}