using System.Diagnostics;

namespace SecondHandProject.Models
{
    public class EmailVerificationModel
    {
        public String UserEmail { get; set; }
        public int CodeGenerated { get; set; }
    }
}
