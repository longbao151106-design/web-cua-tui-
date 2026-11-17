# Đưa ứng dụng lên host khoa

Đây là checklist triển khai, không phải xác nhận đã đạt điểm cộng. Ứng dụng phải chạy được trên host khoa cấp thì mới có bằng chứng cho tiêu chí đó.

## Thông tin cần xin từ giảng viên / quản trị host

- Host Windows IIS hỗ trợ **ASP.NET .NET Framework 4.8**, MVC 5; application pool **.NET CLR v4.0**, **Integrated**. Không yêu cầu cài .NET 9 để chạy ứng dụng này.
- Tên miền / đường dẫn ứng dụng, cách tải tệp: FTP, Web Deploy hoặc bảng điều khiển.
- SQL Server: địa chỉ máy chủ, tên database được cấp, tài khoản, quyền tạo bảng. Máy chủ ứng dụng phải truy cập được SQL Server.
- HTTPS, giới hạn ứng dụng và cách xem lỗi từ máy chủ.

Host chỉ hỗ trợ PHP/Linux hoặc chỉ ASP.NET Core sẽ không chạy trực tiếp dự án này. Cần xác nhận với khoa trước khi triển khai.

## Các bước

1. Chạy `scripts/Publish-Local.ps1` để biên dịch Release và xuất tệp chạy vào thư mục mới `artifacts/publish`. Script chỉ tạo gói local, không gửi dữ liệu ra ngoài.
2. Sao lưu bản cấu hình và database của host trước khi thay đổi. Cấu hình `TeaContext` trên bản publish theo SQL Server khoa cấp. Không dùng `.\SQLEXPRESS` của máy cá nhân trên host.
3. Nếu khoa cấp database rỗng có sẵn, dùng SSMS → database local `TraSuaMoc` → **Tasks → Generate Scripts**, chọn schema và dữ liệu danh mục/sản phẩm/tài khoản demo cần dùng. Không chuyển hóa đơn thử của người khác hoặc dữ liệu riêng. Nếu xuất cả EF metadata, giữ schema tương ứng. Thực hiện theo quyền và hướng dẫn của khoa; không ghi đè database đang có dữ liệu.
4. Tải nội dung thư mục publish vào thư mục ứng dụng. Các thư viện MVC/EF nằm trong `bin`; host không cần khôi phục NuGet.
5. Với HTTPS, bật yêu cầu cookie bảo mật bằng `requireSSL="true"` cho `httpCookies` và `forms` trong `Web.config`; đặt machineKey cố định do quản trị cung cấp nếu host dùng nhiều máy. Không commit khóa và mật khẩu vào GitHub.
6. Thử từ trình duyệt ngoài máy cá nhân: login, danh sách, chi tiết, thêm/sửa/xóa giỏ, checkout, hóa đơn và lịch sử. Kiểm tra lại sau khi ứng dụng khởi động lại để xác nhận hóa đơn còn trong SQL Server.
7. Ghi lại URL hoạt động và ảnh kiểm tra để đưa vào báo cáo. Điểm cộng phụ thuộc tiêu chí chấm của giảng viên.

## GitHub

Chỉ push khi chủ dự án yêu cầu và cung cấp đúng repository của người nộp bài. Source cần `.sln`, `.csproj`, Controllers, Models, Data, Services, Views, Content, Scripts, Web.config mẫu, README và các script cần thiết. Không push database thật, thông tin host, tệp build/IDE hoặc thư mục công cụ hỗ trợ ngoài ứng dụng.
