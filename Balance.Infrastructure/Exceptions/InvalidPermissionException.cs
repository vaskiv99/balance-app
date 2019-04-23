namespace Balance.Infrastructure.Exceptions
{
    public class InvalidPermissionException : BaseException
    {
        public InvalidPermissionException()
            : base("Invalid Permission")
        {
            StatusCode = 403;
        }

        public InvalidPermissionException(string message)
            : base(message)
        {
            StatusCode = 403;
        }
    }
}
