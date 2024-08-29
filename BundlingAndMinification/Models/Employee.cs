using System.ComponentModel.DataAnnotations;

namespace BundlingAndMinification.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string JobDescription { get; set; }
        public int Experience { get; set; }
    }
}
