namespace SecondHandProject.Models
{
    public class RegisterUserModel
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
    }

    public class RegisterUserResultModel
    {
        public int UserId { get; set; }
        public string UserName { get; set;}
        public string UserEmail { get; set;}
        public int CodeGenerated { get; set;}
    }
}
