namespace SecondHandProject.Models
{
    public class ItemModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string ItemTitle { get; set; }
        public string ItemDescription { get; set; }
        public decimal ItemPrice { get; set; }
        public DateTime CreatedDate { get; set; }
        public int AnnoucementLevel { get; set; }
        
    }
}
