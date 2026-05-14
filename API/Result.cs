namespace API
{
    public sealed class Result<TValue, TError>
        where TValue : notnull
        where TError : notnull
    {
        #region Variables
        private readonly bool isSuccess;
        private readonly TValue value;
        private readonly TError error;
        #endregion



        #region Constructor Method
        public Result(bool IsSuccess,
                      TValue Value,
                      TError Error)
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

        public TError Error
        {
            get => this.error;
        }
        #endregion



        public static Result<TValue, TError> Success(TValue Value)
        {
            return new Result<TValue, TError>(IsSuccess: true, Value: Value, Error: default!);
        }

        public static Result<TValue, TError> Failure(TError Error)
        {
            return new Result<TValue, TError>(IsSuccess: false, Value: default!, Error: Error);
        }
    }
}