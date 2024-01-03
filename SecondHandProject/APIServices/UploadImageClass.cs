namespace SecondHandProject.APIServices
{
    public class UploadImageClass
    {
        public static string UploadProfileImage(byte[] ImageArray)
        {
            var Mstream = new MemoryStream(ImageArray);
            var ImageName = Guid.NewGuid().ToString();
            var file = $"{ImageName}.jpg";
            var folder = "wwwroot/ProfileImages";
            var fullpath = $"{folder}/{file}";
            var ImageFullPath = fullpath.Remove(0, 7);
            FileStream fs = new FileStream(fullpath, FileMode.Create);
            Mstream.WriteTo(fs);
            Mstream.Close();
            fs.Close();
            fs.Dispose();
            return ImageFullPath;
        }

        public static string UploadItemImage(byte[] ImageArray)
        {
            var Mstream = new MemoryStream(ImageArray);
            var ImageName = Guid.NewGuid().ToString();
            var file = $"{ImageName}.jpg";
            var folder = "wwwroot/ItemImages";
            var fullpath = $"{folder}/{file}";
            var ImageFullPath = fullpath.Remove(0, 7);
            FileStream fs = new FileStream(fullpath, FileMode.Create);
            Mstream.WriteTo(fs);
            Mstream.Close();
            fs.Close();
            fs.Dispose();
            return ImageFullPath;
        }
    }
}
