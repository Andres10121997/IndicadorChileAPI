using API.App.DTO;

namespace API
{
    public sealed class Result<TValue>
        where TValue : notnull
    {
        #region Variables
        private readonly bool isSuccess;
        private readonly TValue value;
        private readonly ResultErrorDto error;
        #endregion



        #region Constructor Method
        public Result(bool IsSuccess,
                      TValue Value,
                      ResultErrorDto Error)
            : base()
        {
            this.isSuccess = IsSuccess;
            this.value = Value;
            this.error = Error;
        }
        #endregion



        #region Field
        public TValue Value
        {
            get => this.value;
        }

        public bool IsSuccess
        {
            get => this.isSuccess;
        }

        public ResultErrorDto Error
        {
            get => this.error;
        }
        #endregion



        public static Result<TValue> Success(TValue Value)
        {
            return new Result<TValue>(IsSuccess: true, Value: Value, Error: default!);
        }

        public static Result<TValue> Failure(ResultErrorDto Error)
        {
            return new Result<TValue>(IsSuccess: false, Value: default!, Error: Error);
        }
    }
}