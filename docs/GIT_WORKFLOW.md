# Git Workflow

Tài liệu này ghi lại quy ước làm việc với Git để tránh ảnh hưởng tới code hiện tại.

## Branching

- `main`: trạng thái ổn định.
- `feature/<ten-nhanh>`: tính năng mới.
- `fix/<mo-ta-ngan>`: sửa lỗi.
- `docs/<mo-ta-ngan>`: thay đổi tài liệu.

## Quy tắc thao tác

- Luôn xem diff trước khi commit.
- Tách thay đổi tài liệu khỏi thay đổi code nếu có thể.
- Không đưa file build output vào Git.
- Chỉ commit các file thực sự cần thiết cho thay đổi đó.

## Gợi ý cho pull request

- Một PR nên có một mục tiêu chính.
- Mô tả rõ phần nào thay đổi và phần nào không thay đổi.
- Nếu chỉ cập nhật tài liệu, ghi rõ rằng code runtime không bị ảnh hưởng.

## Khi làm việc với repository này

- Ưu tiên cập nhật README, CONTRIBUTING và template PR trước khi đụng tới logic ứng dụng.
- Nếu thay đổi chỉ là tài liệu, không cần sửa `BusinessLayer`, `DataAccessLayer` hoặc `PRN222_assigment2`.