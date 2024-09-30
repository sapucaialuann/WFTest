using System.ComponentModel.DataAnnotations;

namespace WFTest.Models
{
    public class CartItem
    {
        [Key]
        public string ItemId { get; set; }
        public string CartId { get; set; }
        public int Quantity { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}