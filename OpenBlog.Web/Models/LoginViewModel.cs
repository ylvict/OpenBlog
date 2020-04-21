namespace OpenBlog.Web.Models
{
    public class LoginViewModel
    {
        public bool IsRemberme { get; set; }
        public RemeberOptions? RemberDays { get; set; }

        public LoginViewModel()
        {
            IsRemberme = true;
            RemberDays = RemeberOptions.OneWeek;
        }
    }
}
