using IndicadorChileAPI.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IndicadorChileAPI.Models
{
    public sealed record SearchFilterModel : IValidatableObject
    {
        #region Field
        [
            Display(
                AutoGenerateField = false,
                AutoGenerateFilter = false,
                Description = "0 = Dólar; 1 = UF",
                GroupName = nameof(SearchFilterModel),
                Name = "Tipo de divisa",
                Order = 1,
                Prompt = "Ingrese aquí el tipo de divisa.",
                ShortName = "Tipo de divisa"
            ),
            Editable(
                allowEdit: true
            ),
            EnumDataType(
                enumType: typeof(Enums.currencyTypeEnum)
            ),
            Required(
                AllowEmptyStrings = false
            )
        ]
        public required Enums.currencyTypeEnum CurrencyType { get; init; }

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
                AllowEmptyStrings = false
            ),
            Range(
                minimum: 2013,
                maximum: ushort.MaxValue
            )
        ]
        public required ushort Year { get; init; }

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
                maximum: 12
            )
        ]
        public Enums.monthNameEnum? Month { get; init; }
        #endregion



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            #region Variables
            DateOnly Now;
            #endregion

            Now = DateOnly.FromDateTime(dateTime: DateTime.Now);

            #region Validations
            if (this.Year > Now.Year)
            {
                yield return new ValidationResult(
                    errorMessage: $"La propiedad '{nameof(this.Year)}' no puede ser posterior al año {Now.Year}",
                    memberNames: new[]
                    {
                        nameof(this.Year)
                    }
                );
            }

            if (this.Month != null
                &&
                (int)this.Month > Now.Month
                &&
                (this.Year == Now.Year || this.Year.Equals(obj: Now.Year)))
            {
                yield return new ValidationResult(
                    errorMessage: $"Las propiedades '{nameof(this.Month)}' y '{nameof(this.Year)}' no puede ser posterior a {Now.ToString(format: "MMMM-yyyy")}.",
                    memberNames: new[]
                    {
                        nameof(this.Month),
                        nameof(this.Year)
                    }
                );
            }
            #endregion
        }
    }
}