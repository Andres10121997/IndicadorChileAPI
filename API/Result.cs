namespace API
{
    internal sealed class Result<T>
        where T : notnull
    {
        #region Variables
        private bool isSuccess;
        private T value;
        private string error;
        #endregion



        #region Constructor Method
        internal Result(bool IsSuccess,
                        T Value,
                        string Error)
            : base()
        {
            this.isSuccess = IsSuccess;
            this.value = Value;
            this.error = Error;
        }
        #endregion



        #region Field
        internal T Value
        {
            get => this.value;
        }

        internal bool IsSuccess
        {
            get => this.isSuccess;
        }

        internal string Error
        {
            get => this.error;
        }
        #endregion



        internal static Result<T> Success(T Value)
        {
            return new Result<T>(IsSuccess: true, Value: Value, Error: string.Empty);
        }

        internal static Result<T> Failure(string Error)
        {
            return new Result<T>(IsSuccess: false, Value: default!, Error: Error);
        }
    }
}