using System.Security.Cryptography;
using System.Text;

namespace SecondHandProject.APIServices
{
    public class HashPassword
    {
        public static string HashUserPassword(string input)
        {
            string output = string.Empty;
            MD5 hash = MD5.Create();
            byte[] data = hash.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            for(int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("X"));
            }
            output= builder.ToString();
            return output;
        }
    }
}
