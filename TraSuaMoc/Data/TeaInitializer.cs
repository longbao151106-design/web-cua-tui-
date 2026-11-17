using System.Data.Entity;
using TraSuaMoc.Models;
using TraSuaMoc.Services;

namespace TraSuaMoc.Data
{
    public class TeaInitializer : CreateDatabaseIfNotExists<TeaContext>
    {
        protected override void Seed(TeaContext db)
        {
            var milk = new Category { Name = "Trà sữa" };
            var fresh = new Category { Name = "Trà trái cây" };
            var special = new Category { Name = "Mộc đặc biệt" };
            db.Categories.AddRange(new[] { milk, fresh, special });
            db.Products.AddRange(new[] {
                new Product { Name="Trà sữa ô long", Price=39000, Category=milk, Color="oolong", Badge="Bán chạy", Description="Ô long rang thơm dịu từ những lá trà chọn lọc, hòa cùng sữa béo nhẹ và trân châu đen mềm dẻo. Vị trà rõ, hậu ngọt thanh. Phục vụ lạnh, ly 500 ml. Thành phần có sữa; trân châu đã bao gồm trong giá." },
                new Product { Name="Matcha sữa tươi", Price=49000, Category=special, Color="matcha", Badge="Mộc yêu thích", Description="Matcha xanh mượt quyện cùng sữa tươi, giữ lại vị chát nhẹ đặc trưng và hương trà thanh khiết. Phục vụ lạnh, ly 500 ml. Thành phần có sữa; trân châu đã bao gồm trong giá." },
                new Product { Name="Trà sữa khoai môn", Price=45000, Category=milk, Color="taro", Badge="", Description="Khoai môn thơm bùi kết hợp cùng nền trà đen và sữa, tạo nên một ly trà tím nhẹ, béo vừa và dễ uống. Phục vụ lạnh, ly 500 ml. Thành phần có sữa; trân châu đã bao gồm trong giá." },
                new Product { Name="Sữa tươi đường đen", Price=45000, Category=special, Color="brown", Badge="Đậm vị", Description="Sữa tươi mát lành cùng đường đen nấu thơm và trân châu dẻo. Những vệt đường đen hòa dần vào sữa cho vị ngọt tròn đầy. Phục vụ lạnh, ly 500 ml. Thành phần có sữa." },
                new Product { Name="Trà đào cam sả", Price=42000, Category=fresh, Color="peach", Badge="Thanh mát", Description="Trà lài thanh nhẹ, đào thơm, lát cam tươi và hương sả dịu. Một lựa chọn tươi mát cho buổi chiều nhiều nắng. Phục vụ lạnh, ly 500 ml. Không chứa sữa; không kèm trân châu." },
                new Product { Name="Trà sữa lài", Price=35000, Category=milk, Color="jasmine", Badge="", Description="Hương hoa lài dịu dàng trong nền trà xanh, kết hợp sữa mềm mịn và trân châu đen. Vị nhẹ, thơm và thanh. Phục vụ lạnh, ly 500 ml. Thành phần có sữa; trân châu đã bao gồm trong giá." }
            });
            db.Customers.Add(new Customer { Name="Khách của Mộc", Email="demo@trasuamoc.vn", PasswordHash=PasswordService.Hash("TraSua@123") });
            db.SaveChanges();
        }
    }
}
