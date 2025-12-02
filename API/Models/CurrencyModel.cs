using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public record CurrencyModel : IValidatableObject
    {
        #region Property
        [
            DataType(
                dataType: DataType.Custom
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "https://www.youtube.com/shorts/UwcOL3ZL3go",
                GroupName = nameof(CurrencyModel),
                Name = "Identificador",
                Order = 1,
                Prompt = "",
                ShortName = "ID"
            ),
            Key,
            Required(
                AllowEmptyStrings = false
            )
        ]
        public required Guid ID { get; init; }

        [
            DataType(
                dataType: DataType.Date
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                GroupName = nameof(CurrencyModel),
                Name = "Fecha",
                Order = 2,
                Prompt = "Ingrese aquí la fecha.",
                ShortName = "Fecha"
            ),
            Required(
                AllowEmptyStrings = false
            )
        ]
        public required DateOnly Date { get; init; }

        [
            DataType(
                dataType: DataType.Text
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                GroupName = nameof(CurrencyModel),
                Name = "Nombre del día de la semana",
                Order = 3,
                Prompt = "Ingrese aquí el nombre del día de la semana.",
                ShortName = "Día de la semana"
            ),
            Required(
                AllowEmptyStrings = false
            )
        ]
        public required string WeekdayName { get; init; }

        [
            DataType(
                DataType.Currency
            ),
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "",
                GroupName = nameof(CurrencyModel),
                Name = "Valor Divisa",
                Order = 4,
                Prompt = "Ingrese aquí el valor de la divisa.",
                ShortName = "Divisa"
            ),
            Required(
                AllowEmptyStrings = false
            ),
            Range(
                minimum: 0,
                maximum: float.MaxValue
            )
        ]
        public required float Currency { get; init; }
        #endregion



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Currency == float.NaN)
            {
                yield return new ValidationResult(
                    errorMessage: $"No puede ser {nameof(float.NaN)}",
                    memberNames: new[]
                    {
                        nameof(this.Currency)
                    }
                );
            }
        }
    }
}