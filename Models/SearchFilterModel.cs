using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace IndicadorChileAPI.Models
{
    public record SearchFilterModel : IValidatableObject
    {
        [
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "Parámetro para buscar el año",
                GroupName = "",
                Name = "Año",
                Order = 1,
                Prompt = "Ingrese aquí el año.",
                ShortName = "Año"
            ),
            Required(
                AllowEmptyStrings = false
            ),
            Range(
                minimum: 2013,
                maximum: int.MaxValue
            )
        ]
        public required ushort Year { get; init; }

        [
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "Parámetro para buscar el mes",
                GroupName = "",
                Name = "Mes",
                Order = 2,
                Prompt = "Ingrese aquí el mes.",
                ShortName = "Mes"
            ),
            Range(
                minimum: 1,
                maximum: 12
            )
        ]
        public byte? Month { get; init; }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            #region Variables
            DateOnly Date;
            bool[] MonthValidations;
            #endregion

            Date = DateOnly.FromDateTime(DateTime.Now);
            MonthValidations = new bool[]
            {
                Month < 1
                ||
                Month > 12,
                Month > Date.Month
                &&
                (Year == Date.Year || Year.Equals(obj: Date.Year))
            };

            ArgumentOutOfRangeException.ThrowIfGreaterThan<int>(
                value: Year,
                other: Date.Year
            );

            if (MonthValidations.Contains(value: true))
            {
                yield return new ValidationResult("Error con el mes.", new[] { nameof(Month) });
            }
        }
    }
}