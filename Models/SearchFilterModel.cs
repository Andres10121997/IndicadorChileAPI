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
            #endregion

            Date = DateOnly.FromDateTime(dateTime: DateTime.Now);
            
            if (Month < 1)
            {
                yield return new ValidationResult(
                    errorMessage: "El mes no puede ser inferior a 1.",
                    memberNames: new[] {
                        nameof(Month)
                    }
                );
            }
            else
            if (Month > 12)
            {
                yield return new ValidationResult(
                    errorMessage: "El mes no puede ser superior a 12.",
                    memberNames: new[] {
                        nameof(Month)
                    }
                );
            }
            else
            if (Year > Date.Year)
            {
                yield return new ValidationResult(
                    errorMessage: $"El año no puede ser superior a {Date.Year}.",
                    memberNames: new[] {
                        nameof(Year)
                    }
                );
            }
            else
            if (Month > Date.Month
                &&
                (Year == Date.Year || Year.Equals(obj: Date.Year)))
            {
                yield return new ValidationResult(
                    errorMessage: $"El mes y año no puede ser posterior a {Date.ToString("MM-yyyy")}.",
                    memberNames: new[] {
                        nameof(Month),
                        nameof(Year)
                    }
                );
            }
        }
    }
}