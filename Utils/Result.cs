using System;

namespace IndicadorChileAPI.Utils
{
    public sealed class Result<TValue, TError>
    {
        #region Variables
        private readonly bool isSuccess;
        private readonly TValue? value;
        private readonly TError? error;
        #endregion



        #region Constructor Method
        public Result(bool IsSuccess,
                      TValue? Value,
                      TError? Error)
            : base()
        {
            this.isSuccess = IsSuccess;
            this.value = Value;
            this.error = Error;
        }
        #endregion



        #region Field
        public bool IsSuccess
        {
            get => this.isSuccess;
        }

        public TValue? Value
        {
            get
            {
                if (!this.isSuccess)
                {
                    throw new Exception(message: "Operación inválida: no se puede obtener el valor de un resultado fallido.");
                }

                return this.value;
            }
        }

        public TError? Error
        {
            get
            {
                if (this.isSuccess)
                {
                    throw new Exception(message: "Operación inválida: no se puede obtener el error de un resultado exitoso.");
                }

                return this.error;
            }
        }
        #endregion



        public static Result<TValue, TError> Success(TValue Value)
        {
            return new Result<TValue, TError>(
                IsSuccess: true,
                Value: Value,
                Error: default!
            );
        }

        public static Result<TValue, TError> Failure(TError Error)
        {
            return new Result<TValue, TError>(
                IsSuccess: false,
                Value: default!,
                Error: Error
            );
        }
    }
}