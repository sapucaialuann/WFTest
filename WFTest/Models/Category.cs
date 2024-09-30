using System.ComponentModel.DataAnnotations;

namespace WFTest.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required, StringLength(100), Display(Name = "Name")]
        public string CategoryName { get; set; }
    }
}