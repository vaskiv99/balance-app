namespace Balance.Models.Responses
{
    public class OperationResponse<T>
    {
        public T Data { get; set; }
        public bool IsSuccess { get; set; }
        public ResponseError Error { get; set; }

        public OperationResponse(string errorMessage)
        {
            Error = new ResponseError(errorMessage);
            Data = default;
            IsSuccess = false;
        }

        public OperationResponse(T data)
        {
            Error = default;
            Data = data;
            IsSuccess = true;
        }
    }
}