"""Kiểm thử HTTP trên IIS Express và SQL Server thật; chỉ dùng thư viện chuẩn Python.
Chạy: python -X utf8 tests/integration.py [http://localhost:5186]
Tạo một đơn hàng mô phỏng trong tài khoản demo, không xóa dữ liệu.
"""
import html
import http.cookiejar
import re
import sys
import urllib.error
import urllib.parse
import urllib.request

BASE = sys.argv[1] if len(sys.argv) > 1 else 'http://localhost:5186'
client = urllib.request.build_opener(urllib.request.HTTPCookieProcessor(http.cookiejar.CookieJar()))
checks = 0

def request(path, data=None, agent=client):
    encoded = urllib.parse.urlencode(data).encode() if data is not None else None
    try:
        response = agent.open(BASE + path, encoded, timeout=60)
    except urllib.error.HTTPError as error:
        response = error
    body = html.unescape(response.read().decode('utf-8'))
    return response.code, body, response.geturl()

def check(condition, description):
    global checks
    if not condition:
        raise AssertionError(description)
    checks += 1
    print('PASS ' + description)

def field(body, name):
    for tag in re.findall(r'<input\b[^>]*>', body):
        if re.search(r'\bname="' + re.escape(name) + '"', tag):
            found = re.search(r'\bvalue="([^"]*)"', tag)
            return html.unescape(found.group(1)) if found else ''
    raise AssertionError('Missing form field: ' + name)

def post(path, body, data):
    values = dict(data, __RequestVerificationToken=field(body, '__RequestVerificationToken'))
    return request(path, values)

status, home, _ = request('/')
check(status == 200 and 'Một ly trà sữa.' in home, 'Trang chủ trà sữa trả 200 và tiếng Việt đúng UTF-8')
check(request('/Content/site.css')[0] == 200, 'CSS được phục vụ')
status, menu, _ = request('/Products')
check(status == 200 and menu.count('class="product-card"') == 6, 'Danh sách hiển thị đủ 6 sản phẩm')
check('39.000' in menu and 'Giỏ hàng' in menu, 'Danh sách có giá và liên kết giỏ hàng')
check('Chưa tìm thấy' in request('/Products?search=khongcomonnay')[1], 'Tìm kiếm không có kết quả')
check(request('/Products?categoryId=1')[1].count('class="product-card"') == 3, 'Lọc danh mục bằng LINQ')
status, details, _ = request('/Products/Details/1')
check(status == 200 and 'Thành phần có sữa' in details and 'Thêm vào giỏ' in details, 'Chi tiết có mô tả, giá và nút mua')
check(request('/Products/Details/999999')[0] == 404, 'Sản phẩm không tồn tại trả 404')
check(request('/Cart/Add', {'productId': 1})[0] >= 400, 'Không chấp nhận POST thiếu token chống giả mạo')
check(post('/Cart/Add', details, {'productId': 1, 'quantity': -1})[0] == 400, 'Từ chối số lượng âm')
status, cart, _ = post('/Cart/Add', details, {'productId': 1, 'quantity': 2})
check(status == 200 and '78.000' in cart, 'Thêm 2 ly, thành tiền 78.000 đ')
status, cart, _ = post('/Cart/Update', cart, {'productId': 1, 'quantity': 0})
check('Số lượng mỗi món từ 1 đến 20' in cart and '78.000' in cart, 'Số lượng không hợp lệ không làm đổi giỏ')
status, cart, _ = post('/Cart/Update', cart, {'productId': 1, 'quantity': 3})
check('117.000' in cart, 'Cập nhật số lượng và tổng tiền')
status, cart, _ = post('/Cart/Remove', cart, {'productId': 1})
check('Giỏ hàng đang đợi' in cart, 'Gỡ bỏ món và hiển thị giỏ trống')
status, cart, _ = post('/Cart/Add', details, {'productId': 1, 'quantity': 2})
_, details2, _ = request('/Products/Details/2')
status, cart, _ = post('/Cart/Add', details2, {'productId': 2, 'quantity': 1})
check('127.000' in cart, 'Tổng nhiều món: 2 × 39.000 + 49.000 = 127.000 đ')
status, login, url = request('/Orders')
check('/Account/Login' in url, 'Trang đơn hàng yêu cầu đăng nhập')
status, login, _ = post('/Account/Login', login, {'Email': 'demo@trasuamoc.vn', 'Password': 'sai', 'ReturnUrl': '/Cart'})
check('Email hoặc mật khẩu chưa đúng' in login, 'Đăng nhập sai hiển thị thông báo')
status, cart, url = post('/Account/Login', login, {'Email': 'demo@trasuamoc.vn', 'Password': 'TraSua@123', 'ReturnUrl': '/Cart'})
check(status == 200 and '/Cart' in url and '127.000' in cart, 'Đăng nhập đúng giữ lại giỏ hàng')
checkout_token = field(cart, 'checkoutToken')
csrf = field(cart, '__RequestVerificationToken')
check(request('/Cart/Checkout', {'checkoutToken': 'invalid', '__RequestVerificationToken': csrf})[0] == 400, 'Từ chối mã thanh toán không hợp lệ')
status, invoice, url = request('/Cart/Checkout', {'checkoutToken': checkout_token, '__RequestVerificationToken': csrf})
check(status == 200 and '/Orders/Invoice/' in url, 'Thanh toán tạo hóa đơn trong SQL Server')
check('Trà sữa ô long' in invoice and 'Matcha sữa tươi' in invoice and '127.000' in invoice, 'Hóa đơn có đúng sản phẩm và tổng tiền')
check('Đã thanh toán (mô phỏng)' in invoice, 'Trạng thái thanh toán ghi rõ mô phỏng')
invoice_path = urllib.parse.urlparse(url).path
status, _, duplicate_url = request('/Cart/Checkout', {'checkoutToken': checkout_token, '__RequestVerificationToken': csrf})
check(urllib.parse.urlparse(duplicate_url).path == invoice_path, 'Gửi lại thanh toán trả cùng hóa đơn, không tạo đơn trùng')
check('Giỏ hàng đang đợi' in request('/Cart')[1], 'Giỏ hàng được làm trống sau thanh toán')
status, history, _ = request('/Orders')
check(status == 200 and 'Tổng đã thanh toán' in history and invoice_path in history, 'Lịch sử đơn và tổng hợp ADO.NET hoạt động')
anonymous = urllib.request.build_opener(urllib.request.HTTPCookieProcessor(http.cookiejar.CookieJar()))
check('/Account/Login' in request(invoice_path, agent=anonymous)[2], 'Khách chưa đăng nhập không xem được hóa đơn')
status, _, _ = post('/Account/Logout', history, {})
check('/Account/Login' in request('/Orders')[2], 'Đăng xuất thu hồi quyền truy cập trang riêng')
print('TOTAL: ' + str(checks) + ' checks passed. Invoice: ' + invoice_path)
