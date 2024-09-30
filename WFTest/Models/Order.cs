using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using PayPal.Api;

namespace WFTest.Models
{
    public class DBOrder
    {
        [Key]
        public string OrderId { get; set; }

        public DateTime OrderDate { get; set; }
        public string FirstName { get; set; }
        [StringLength(160)]
        public string LastName { get; set; }
        [StringLength(24)]
        
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        [ScaffoldColumn(false)]
        public decimal Total { get; set; }
    }
}