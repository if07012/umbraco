using System.ComponentModel.DataAnnotations.Schema;

namespace Learning.Entities
{
    public class Profile
    {
        public int Id { get; set; }
        public string ProfileDescription { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
    }
}
