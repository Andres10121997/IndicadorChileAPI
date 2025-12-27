using API.Controllers;
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
                Description = "",
                GroupName = nameof(SearchFilterModel),
                Name = "Tipo de divisa",
                Order = 1,
                Prompt = "",
                ShortName = "Tipo de divisa"
            ),
            Editable(
                allowEdit: true
            ),
            Required(
                AllowEmptyStrings = false,
                ErrorMessageResourceType = typeof(BaseController.CurrencyTypeEnum)
            )
        ]
        public required BaseController.CurrencyTypeEnum CurrencyType { get; init; }

        [
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "Parámetro para buscar el año",
                GroupName = nameof(SearchFilterModel),
                Name = "Año",
                Order = 2,
                Prompt = "Ingrese aquí el año.",
                ShortName = "Año"
            ),
            Editable(
                allowEdit: true
            ),
            Required(
                AllowEmptyStrings = false,
                ErrorMessageResourceType = typeof(ushort)
            ),
            Range(
                minimum: 2013,
                maximum: ushort.MaxValue,
                ErrorMessageResourceType = typeof(ushort)
            )
        ]
        public required ushort Year
        {
            get => field;
            init
            {
                #region Variables
                DateOnly Date = DateOnly.FromDateTime(dateTime: DateTime.Now);
                #endregion

                #region Exception
                ArgumentOutOfRangeException.ThrowIfLessThan<int>(
                    value: value,
                    other: 2013
                );
                ArgumentOutOfRangeException.ThrowIfGreaterThan<int>(
                    value: value,
                    other: Date.Year
                );
                #endregion

                field = value;
            }
        }

        [
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "Parámetro para buscar el mes",
                GroupName = nameof(SearchFilterModel),
                Name = "Mes",
                Order = 3,
                Prompt = "Ingrese aquí el mes.",
                ShortName = "Mes"
            ),
            Editable(
                allowEdit: true
            ),
            Range(
                minimum: 1,
                maximum: 12,
                ErrorMessageResourceType = typeof(byte?)
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