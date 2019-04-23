namespace Balance.Models.Users
{
    public class UserResponse
    {
        public long Id { get; set; }

        public decimal Balance { get; set; }

        public string UserName { get; set; }
    }
}