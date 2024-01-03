namespace SecondHandProject.Models
{
    public class UserImageModel
    {
        public int UserId { get; set; }
        public byte[] ImageArray { get; set; }
    }

    public class GetUserImage
    {
        public string UserImage { get; set; }
    }

}
