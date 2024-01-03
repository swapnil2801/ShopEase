namespace SecondHandProject.Models
{
    public class GetCodeModel
    {
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public int CodeGenerated { get; set; }
        public DateTime UpadatedDate { get; set; }
    }
}
