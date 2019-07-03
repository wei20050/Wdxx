using System.ComponentModel.DataAnnotations;

namespace Ztest
{
    public class user
    {
        [Key]
        public int? id { get; set; }
        public string name { get; set; }
    }
}
