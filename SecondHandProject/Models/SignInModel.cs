namespace SecondHandProject.Models
{
    public class SignInModel
    {
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
    }

    public class SignInResultModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public bool EmailIsComfirmed { get; set; }

    }
}
