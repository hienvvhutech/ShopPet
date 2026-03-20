# ShopPet

ShopPet là dự án fullstack mô phỏng hệ thống bán thú cưng và phụ kiện thú cưng, bao gồm backend xử lý API, cơ sở dữ liệu và frontend hiển thị giao diện cho người dùng. Dự án được xây dựng nhằm phục vụ học tập, thực hành phát triển phần mềm và làm portfolio cá nhân.

---

## Mô tả dự án

Hệ thống hỗ trợ các chức năng cơ bản của một website bán hàng, bao gồm:

- Quản lý sản phẩm
- Quản lý danh mục
- Quản lý người dùng
- Giỏ hàng và đặt hàng
- Theo dõi đơn hàng
- Phân quyền quản trị và người dùng

Dự án được tổ chức theo cấu trúc tách riêng giữa backend và frontend để dễ phát triển, bảo trì và mở rộng.

---

## Cấu trúc thư mục
ShopPet/
├── backend/    # Source code backend
└── frontend/   # Source code frontend

Công nghệ sử dụng
Backend: 
-ASP.NET Core 
-Entity Framework Core
-SQL Server
-RESTful API
-JWT Authentication
-SignalR

Frontend:
-HTML
-CSS
-JavaScript
-Flutter 

Chức năng chính:
Người dùng:
-Xem danh sách sản phẩm
-Xem chi tiết sản phẩm
-Tìm kiếm sản phẩm
-Thêm sản phẩm vào giỏ hàng
-Đặt hàng
-Xem lịch sử đơn hàng
-Đăng ký / đăng nhập tài khoản

Quản trị viên (admin) :
-Thêm / sửa / xóa sản phẩm
-Quản lý danh mục
-Quản lý người dùng
-Quản lý đơn hàng
-Theo dõi hoạt động hệ thống

Hướng dẫn cài đặt và chạy dự án
1. Clone project
git clone https://github.com/hienvvhutech/ShopPet.git
cd ShopPet
2. Chạy Backend
Di chuyển vào thư mục backend:
cd backend
Cấu hình chuỗi kết nối trong file appsettings.json:
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=ShopPetDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
Nếu dự án có migration, chạy lệnh:

dotnet ef database update

Chạy backend:

dotnet run

Sau khi chạy, backend sẽ hoạt động tại địa chỉ localhost được hiển thị trên terminal, ví dụ:

https://localhost:5001
http://localhost:5000

3. Chạy Frontend
Di chuyển vào thư mục frontend:
cd frontend
Nếu frontend là HTML / CSS / JavaScript:

Mở file index.html bằng trình duyệt
Hoặc chạy bằng Live Server trong VS Code
Flutter mobile app:

flutter pub get
flutter run

API mẫu
Product
-GET /api/products : Lấy danh sách sản phẩm
-GET /api/products/{id} : Lấy chi tiết sản phẩm
-POST /api/products : Thêm sản phẩm mới
-PUT /api/products/{id} : Cập nhật sản phẩm
-DELETE /api/products/{id} : Xóa sản phẩm

Category
-GET /api/categories : Lấy danh sách danh mục
-POST /api/categories : Thêm danh mục
-PUT /api/categories/{id} : Cập nhật danh mục
-DELETE /api/categories/{id} : Xóa danh mục

User
-GET /api/users : Lấy danh sách người dùng
-POST /api/users : Thêm người dùng

Điểm nổi bật

-Tổ chức source code rõ ràng với 2 phần backend và frontend
-Hỗ trợ các chức năng CRUD cơ bản cho hệ thống bán hàng
-Dễ mở rộng thêm tính năng chat, thanh toán online, dashboard thống kê
-Phù hợp để làm đồ án, bài tập lớn hoặc portfolio cá nhân

Hướng phát triển trong tương lai:
-Tích hợp thanh toán online
-Thêm chat real-time giữa khách hàng và admin
-Xây dựng dashboard thống kê doanh thu
-Tối ưu giao diện responsive trên mobile
-Triển khai hệ thống lên cloud
