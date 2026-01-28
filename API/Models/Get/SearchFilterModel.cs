using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Get
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
            FromQuery,
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
            FromQuery,
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
            FromQuery,
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
            DateOnly Now;
            #endregion

            Now = DateOnly.FromDateTime(dateTime: DateTime.Now);

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

            if (this.Month > Now.Month
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
        }
    }
}