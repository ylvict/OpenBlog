namespace OpenBlog.Web.Models
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsRemberme { get; set; }
        public RemeberOptions? RemberDays { get; set; }

        public LoginViewModel()
        {
            IsRemberme = true;
            RemberDays = RemeberOptions.OneWeek;
        }
    }
}
