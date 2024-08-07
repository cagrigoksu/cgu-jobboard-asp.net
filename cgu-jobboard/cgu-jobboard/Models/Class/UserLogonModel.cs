namespace cgu_jobboard.Models.Class
{
    public class UserLogonModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public bool IsCompanyUser { get; set; }
    }
}
