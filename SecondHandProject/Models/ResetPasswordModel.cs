namespace SecondHandProject.Models
{
    public class ResetPasswordModel
    {
        public string Email { get; set; }
        public int Code { get; set; }
        public string NewPassword { get; set; }
    }
}
