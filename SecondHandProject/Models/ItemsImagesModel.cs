namespace SecondHandProject.Models
{
    public class ItemsImagesModel
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public byte[] ImageArray { get; set; }
    }

    public class GetItemImage
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ImageUrl { get; set;}
    }
}
