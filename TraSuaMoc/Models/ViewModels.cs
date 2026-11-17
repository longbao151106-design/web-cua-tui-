using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TraSuaMoc.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email."), EmailAddress(ErrorMessage = "Email chưa đúng định dạng.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu."), DataType(DataType.Password)]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
    public class MenuViewModel
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public string Search { get; set; }
        public int? CategoryId { get; set; }
    }
    [System.Serializable]
    public class CartEntry
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
    public class CartLine
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get { return Product.Price * Quantity; } }
    }
    public class CartViewModel
    {
        public List<CartLine> Lines { get; set; } = new List<CartLine>();
        public decimal Total { get { return Lines.Sum(x => x.Subtotal); } }
        public string CheckoutToken { get; set; }
    }
}
