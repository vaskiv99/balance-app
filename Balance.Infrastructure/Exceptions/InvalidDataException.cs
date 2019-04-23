namespace Balance.Infrastructure.Exceptions
{
    public class InvalidDataException : BaseException
    {
        public InvalidDataException()
            : base("Invalid Data")
        {
            StatusCode = 400;
        }

        public InvalidDataException(string message)
            : base(message)
        {
            StatusCode = 400;
        }
    }
}