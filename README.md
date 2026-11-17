# Mộc Trà Sữa

Website bán trà sữa cho môn Lập trình Web. Dự án **ASP.NET Web Application trên .NET Framework 4.8, ASP.NET MVC 5, Entity Framework 6 và SQL Server**. Không sử dụng ASP.NET Core.

## Mở bằng Visual Studio

1. Dùng Visual Studio 2022 hoặc 2019 có workload **ASP.NET and web development** và **.NET Framework 4.8 targeting pack**.
2. Cài hoặc sử dụng SQL Server Express có sẵn. Kết nối mặc định là `.\SQLEXPRESS`, Windows Authentication.
3. Mở `TraSuaMoc.sln`. Cho phép Visual Studio khôi phục NuGet; nếu cần, nhấn phải solution → **Restore NuGet Packages**.
4. Chọn `TraSuaMoc` làm **Startup Project**, Build Solution, rồi chạy bằng **IIS Express** (F5 hoặc Ctrl+F5).
5. Lần truy cập đầu tiên, Entity Framework tạo database `TraSuaMoc`, 5 bảng nghiệp vụ, 3 danh mục, 6 sản phẩm và tài khoản trải nghiệm. Có thể mất vài giây.

Không cần tải kho tài liệu giảng viên về máy để chạy dự án. NuGet chỉ khôi phục các thư viện ứng dụng cần dùng.

### Tài khoản trải nghiệm

- Email: `demo@trasuamoc.vn`
- Mật khẩu: `TraSua@123`

Đây là tài khoản mẫu dùng chung để kiểm tra chức năng, không dùng lưu thông tin cá nhân. Trước khi triển khai công khai, thay tài khoản mẫu và thông tin cấu hình theo môi trường thực tế.

### Nếu SQL Server có tên khác

Sửa duy nhất chuỗi kết nối `TeaContext` trong `TraSuaMoc/Web.config`:

```xml
<add name="TeaContext"
     connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=TraSuaMoc;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True"
     providerName="System.Data.SqlClient" />
```

Nếu máy dùng LocalDB, thay `Data Source=.\SQLEXPRESS` bằng `Data Source=(LocalDB)\MSSQLLocalDB`. Tài khoản chạy ứng dụng cần có quyền tạo database ở lần khởi tạo đầu. Ứng dụng dùng `CreateDatabaseIfNotExists`, **không tự xóa hoặc tạo lại database đã có**. Khi thay đổi model sau này, dùng EF migrations hoặc cập nhật schema có kiểm soát.

## Chức năng theo đề

| Yêu cầu | Trang / cách kiểm tra |
|---|---|
| Màn hình login | `/Account/Login`, đăng nhập bằng tài khoản mẫu; thông báo khi sai |
| Danh sách có tên, giá, nút xem giỏ | `/Products`, giỏ hàng luôn có trên thanh điều hướng |
| Chi tiết có tên, giá, mô tả, thêm giỏ, xem giỏ | `/Products/Details/1` |
| Giỏ có sản phẩm, đơn giá, bỏ món, tổng và thanh toán | `/Cart`, có thêm cập nhật số lượng |
| Hóa đơn sau thanh toán | `/Orders/Invoice/{id}`, lưu tên món và giá tại thời điểm đặt |
| ASP.NET Web Application (.NET Framework) | Solution `.sln`, project MVC trên .NET Framework 4.8 |
| Entity Framework | `Data/TeaContext.cs`, `Data/TeaInitializer.cs`, các controller |
| LINQ | Tìm kiếm, lọc sản phẩm, tính tổng, truy vấn hóa đơn |
| SQL Server | Database `TraSuaMoc`, provider `System.Data.SqlClient` |
| ADO.NET | `Services/OrderStatisticsService.cs`: `SqlConnection`, `SqlCommand`, tham số `@CustomerId`, tổng đã thanh toán trên trang đơn hàng |

Trang chủ được bổ sung để giới thiệu thương hiệu. Có tìm kiếm, lọc danh mục, lịch sử hóa đơn, nút in hóa đơn, giao diện responsive và hỗ trợ giảm chuyển động. Không có trang quản trị hoặc đăng ký vì đề chưa yêu cầu.

## Luồng demo

1. Trang chủ → Thực đơn → Trà sữa ô long → chọn 2 ly → Thêm vào giỏ.
2. Chọn thêm 1 Matcha sữa tươi. Tổng phải là **127.000 đ**.
3. Trong giỏ, thử sửa số lượng hoặc gỡ bỏ một món.
4. Đăng nhập. Giỏ hàng giữ lại các món đã chọn trong cùng phiên.
5. Chọn **Thanh toán & tạo hóa đơn**.
6. Xem hóa đơn có tên món, đơn giá, số lượng, thành tiền và tổng; có thể in bằng trình duyệt.
7. Vào **Đơn của bạn** để xem lại dữ liệu đã lưu, kể cả sau khi khởi động lại ứng dụng.

Thanh toán là **mô phỏng**, không kết nối ngân hàng, không thu tiền thật, không tạo hóa đơn thuế. Giỏ hàng nằm trong Session (30 phút); hóa đơn nằm trong SQL Server. Giỏ hết hạn không làm mất các hóa đơn đã lưu.

## Cấu trúc source

```text
TraSuaMoc.sln
TraSuaMoc/
  Controllers/        Điều hướng, xác thực, giỏ hàng và hóa đơn
  Models/             Thực thể và view model
  Data/               DbContext, cấu hình ánh xạ và dữ liệu khởi tạo
  Services/           PBKDF2 mật khẩu và tổng hợp ADO.NET
  Views/              Razor, layout và từng màn hình
  Content/            CSS, biểu tượng SVG và ảnh chụp sản phẩm local
  Scripts/            Điều chỉnh số lượng, hiện mật khẩu và in hóa đơn
  Web.config          SQL Server, Forms Authentication, UTF-8
scripts/              Chạy local và xuất gói triển khai
tests/                Kiểm thử tích hợp HTTP bằng Python chuẩn
docs/                 Ghi chú bàn giao hosting
```

Tiền dùng `decimal(18,2)`. Giá được lấy lại từ database khi thanh toán, không nhận giá do trình duyệt gửi lên. `Order` và `OrderDetails` được lưu trong một transaction. Token thanh toán có unique index để chống tạo hóa đơn trùng; các POST dùng anti-forgery token. Mật khẩu được băm PBKDF2-SHA256 với salt ngẫu nhiên. Chỉ tài khoản sở hữu hóa đơn được xem hóa đơn đó.

## Chạy và kiểm tra ngoài IDE

Máy có .NET SDK có thể biên dịch classic project bằng:

```powershell
dotnet restore TraSuaMoc.sln
dotnet build TraSuaMoc.sln --no-restore
powershell -ExecutionPolicy Bypass -File scripts/Start-Local.ps1
```

Mở `http://localhost:5186/`. Script dùng IIS Express đã cài sẵn, không cài máy chủ mới. Để dừng, dùng PID mà script trả về hoặc dừng IIS Express từ Visual Studio.

Kiểm thử HTTP (cần Python 3; không có package Python bổ sung):

```powershell
python -X utf8 tests/integration.py http://localhost:5186
```

Bài kiểm tra tạo một hóa đơn mô phỏng cho tài khoản demo, không xóa dữ liệu. Có 27 kiểm tra: UTF-8, sản phẩm, lọc, tìm kiếm, thêm/sửa/xóa giỏ, tổng tiền, đăng nhập, anti-forgery, checkout, gửi trùng, lịch sử và quyền truy cập. Kiểm thử HTTP không thay cho kiểm tra giao diện trực quan hoặc kiểm tra host khoa.

## Bàn giao

Đây là source **bản v5** đã chốt: giao diện kem–xanh, nền trang có họa tiết nhẹ, ảnh chụp sản phẩm và đầy đủ luồng đặt hàng theo yêu cầu. Website đã được triển khai lên host khoa tại https://2400003600.kcntt.edu.vn. Bản cấu hình trong kho mã nguồn dùng SQL Server trên máy cá nhân; thông tin đăng nhập host được quản lý riêng.

Xem `docs/Hosting.md` khi có thông tin host. `.gitignore` loại trừ tệp IDE/build, thông tin riêng và thư mục công cụ không thuộc ứng dụng. Không đưa `bin/`, `obj/`, `.vs/`, `.env` hoặc thông tin đăng nhập hosting lên GitHub.
