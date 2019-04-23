namespace Balance.Models.Responses
{
    public class ResponseError
    {
        public string Message { get; set; }

        public ResponseError()
        { }

        public ResponseError(string message)
        {
            Message = message;
        }
    }
}