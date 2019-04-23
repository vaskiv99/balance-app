using System.ComponentModel.DataAnnotations;

namespace Balance.Models.Users
{
    public class SignRequest
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(255)]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}