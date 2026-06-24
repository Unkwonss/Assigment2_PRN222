# Hướng Dẫn Đóng Góp

Tài liệu này mô tả quy trình đóng góp an toàn cho repository.

## Nguyên tắc chung

- Chỉ sửa đúng phạm vi của task.
- Không đổi tên hoặc di chuyển file nếu không có lý do rõ ràng.
- Giữ thay đổi nhỏ, dễ kiểm tra và dễ review.
- Ưu tiên tài liệu và cấu hình riêng cho các thay đổi không liên quan tới logic.

## Quy trình đề xuất

1. Tạo branch mới từ nhánh chính.
2. Thực hiện thay đổi trong phạm vi hẹp nhất có thể.
3. Kiểm tra build hoặc test nếu thay đổi ảnh hưởng code.
4. Cập nhật tài liệu nếu hành vi hoặc quy ước thay đổi.
5. Mở pull request với mô tả ngắn gọn, rõ ràng.

## Quy ước commit

- `docs:` cho thay đổi tài liệu.
- `chore:` cho thay đổi cấu hình, template, hoặc housekeeping.
- `fix:` cho sửa lỗi.
- `feat:` cho tính năng mới.

## Điều cần tránh

- Không commit file build output.
- Không chỉnh sửa file sinh tự động trừ khi thật sự cần.
- Không trộn nhiều mục tiêu khác nhau trong cùng một commit.

## Checklist trước khi gửi PR

- Thay đổi đã được kiểm tra lại.
- Không có file thừa hoặc file tạm.
- README hoặc tài liệu liên quan đã được cập nhật.
- Mô tả PR nêu rõ mục tiêu và phạm vi thay đổi.