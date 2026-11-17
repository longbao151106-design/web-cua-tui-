using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TraSuaMoc.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required, StringLength(80)] public string Name { get; set; }
    }
    public class Product
    {
        public int Id { get; set; }
        [Required, StringLength(150)] public string Name { get; set; }
        [Required, StringLength(2000)] public string Description { get; set; }
        [Range(1000, 1000000)] public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        [Required, StringLength(20)] public string Color { get; set; }
        [StringLength(40)] public string Badge { get; set; }
        public bool IsActive { get; set; } = true;
    }
    public class Customer
    {
        public int Id { get; set; }
        [Required, StringLength(100)] public string Name { get; set; }
        [Required, StringLength(150), Index(IsUnique = true)] public string Email { get; set; }
        [Required, StringLength(300)] public string PasswordHash { get; set; }
    }
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        [Required, StringLength(100)] public string CustomerName { get; set; }
        public DateTime CreatedAt { get; set; }
        [Required, StringLength(40)] public string PaymentStatus { get; set; }
        public decimal Total { get; set; }
        [Required, StringLength(64), Index(IsUnique = true)] public string CheckoutToken { get; set; }
        public virtual ICollection<OrderDetail> Details { get; set; } = new List<OrderDetail>();
    }
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        [Required, StringLength(150)] public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        [Range(1, 20)] public int Quantity { get; set; }
    }
}
