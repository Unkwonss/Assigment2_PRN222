# PRN222 Assignment 2

Đây là solution cho bài tập PRN222, được chia thành 3 lớp chính:

- `BusinessLayer`: xử lý nghiệp vụ, DTO, service và helper.
- `DataAccessLayer`: entity, `DbContext`, migration và truy cập dữ liệu.
- `PRN222_assigment2`: lớp trình bày, `Program`, `Pages`, `Hubs`, `wwwroot`.

## Mục tiêu của repository

Repository này được giữ theo hướng an toàn cho việc cộng tác:

- Giữ thay đổi tài liệu tách biệt với code chạy.
- Không chỉnh sửa logic ứng dụng nếu chỉ cần cập nhật tài liệu hoặc quy ước đóng góp.
- Ưu tiên commit nhỏ, rõ ràng, dễ review.

## Cấu trúc thư mục

```text
PRN222_assigment2.sln
BusinessLayer/
DataAccessLayer/
PRN222_assigment2/
```

## Chạy dự án

1. Mở solution `PRN222_assigment2.sln` trong Visual Studio hoặc VS Code.
2. Khôi phục NuGet packages.
3. Cập nhật chuỗi kết nối nếu cần trong cấu hình của lớp trình bày.
4. Build và chạy project `PRN222_assigment2`.

## Quy ước đóng góp

- Tạo branch riêng cho mỗi thay đổi.
- Tránh sửa trực tiếp vào file sinh tự động hoặc migration nếu không cần thiết.
- Tách tài liệu, cấu hình và code nghiệp vụ thành các commit khác nhau.
- Mô tả rõ lý do thay đổi trong pull request.

## Tài liệu liên quan

- Xem [CONTRIBUTING.md](CONTRIBUTING.md) để biết quy trình đóng góp.
- Xem [docs/GIT_WORKFLOW.md](docs/GIT_WORKFLOW.md) để nắm quy ước làm việc với Git.