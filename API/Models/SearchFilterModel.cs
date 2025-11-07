using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public sealed record SearchFilterModel : IValidatableObject
    {
        #region Property
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
                maximum: 12,
                ErrorMessage = "El mes debe estar entre 1 y 12."
            )
        ]
        public byte? Month { get; init; }
        #endregion



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            #region Variables
            DateOnly Date;
            #endregion

            Date = DateOnly.FromDateTime(dateTime: DateTime.Now);

            if (this.Year > Date.Year)
            {
                yield return new ValidationResult(
                    errorMessage: $"El año no puede ser superior a {Date.Year}.",
                    memberNames: new[]
                    {
                        nameof(this.Year)
                    }
                );
            }
            else
            if (this.Month > Date.Month
                &&
                (this.Year == Date.Year || this.Year.Equals(obj: Date.Year)))
            {
                yield return new ValidationResult(
                    errorMessage: $"El mes y año no puede ser posterior a {Date.ToString(format: "MMMM-yyyy")}.",
                    memberNames: new[]
                    {
                        nameof(this.Month),
                        nameof(this.Year)
                    }
                );
            }
        }
    }
}