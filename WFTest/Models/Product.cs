using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace WFTest.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        [Required, StringLength(100), Display(Name = "Name")]
        public string ProductName { get; set; }
        public string ProductDescription { get; set; } = string.Empty;
        [Display(Name = "Price")]
        public double ProductPrice { get; set; }
        public string ImagePath { get; set; }
        public int? CategoryId { get; set; }
        public virtual Category Category { get; set; }

    }
}