# VietRAG System - Vietnamese RAG Chatbot & Research Benchmark

![.NET Version](https://img.shields.io/badge/.NET-9.0-blue.svg) ![Entity Framework](https://img.shields.io/badge/EF%20Core-9.0-purple.svg) ![SignalR](https://img.shields.io/badge/SignalR-Real--time-orange.svg) ![License](https://img.shields.io/badge/License-MIT-green.svg)

**VietRAG System** (Vietnamese RAG Chatbot & Research Benchmark) là một nền tảng RAG (Retrieval-Augmented Generation) kết hợp với công cụ đánh giá (Benchmarking) hiệu năng AI. Dự án được thiết kế chuyên biệt để hỗ trợ học tập cho sinh viên và phục vụ nghiên cứu học thuật tại Đại học FPT (hỗ trợ các môn học như PRN222, PRN212, SWP391).

Nền tảng cho phép người dùng thử nghiệm, so sánh và đánh giá các cấu hình RAG khác nhau (sự kết hợp giữa LLM, mô hình Embedding và các chiến lược Chunking) để tìm ra giải pháp tối ưu nhất cho từng bộ dữ liệu tài liệu học tập.

---

## 🏗️ Kiến trúc dự án (3-Layer Architecture)

Dự án tuân thủ mô hình kiến trúc 3 lớp tiêu chuẩn trong phát triển ứng dụng .NET:

1. **Presentation Layer (`PRN222_assigment2`)**:
   - Được xây dựng trên **ASP.NET Core Razor Pages**.
   - Sử dụng **SignalR** cho tính năng chat và thông báo thời gian thực.
   - Giao diện hiện đại, trực quan, hỗ trợ Responsive đầy đủ (sử dụng Bootstrap, Bootstrap Icons, và phong cách thiết kế Dark Mode / Glassmorphism đặc trưng).
   - Trực quan hóa dữ liệu hiệu năng benchmark bằng biểu đồ động qua **Chart.js** và thông báo tương tác qua **SweetAlert2**.

2. **Business Layer (`BusinessLayer`)**:
   - Xử lý toàn bộ logic nghiệp vụ của hệ thống: Quản lý người dùng, quản lý tài liệu, quản lý phiên chat.
   - Cung cấp các chiến lược phân đoạn văn bản (**Chunking Strategies**): `FixedSizeChunker`, `ParagraphChunker`, `SentenceChunker`, `RecursiveChunker`.
   - Tích hợp các nhà cung cấp mô hình vector hóa (**Embedding Providers**): Gemini API, OpenAI API, HuggingFace Inference API, và Ollama (chạy cục bộ).
   - Bộ máy chạy thử nghiệm đánh giá hiệu năng tự động (**Benchmark Runner**).

3. **Data Access Layer (`DataAccessLayer`)**:
   - Sử dụng **Entity Framework Core** để giao tiếp với **Microsoft SQL Server**.
   - Quản lý các Migration và cấu hình Fluent API cho cơ sở dữ liệu.
   - Cung cấp Pattern Generic Repository để đơn giản hóa các truy vấn dữ liệu.

---

## ✨ Các tính năng chính

### 👥 1. Phân quyền và quản lý tài khoản người dùng
Hệ thống hỗ trợ 3 vai trò (Roles) chính được phân quyền rõ rệt:
- **Admin**: Quản lý toàn bộ tài khoản trong hệ thống, quản lý thông tin sinh viên, quản lý tài liệu, truy cập toàn quyền các bảng điều khiển RAG Chatbot và Benchmarks.
- **Teacher**: Quản lý các tài liệu học tập theo chương học (Chapters), truy cập RAG Chatbot và thiết lập/chạy các thực nghiệm (Experiments) để đánh giá cấu hình RAG.
- **Student**: Sử dụng giao diện chat học tập trực quan để hỏi đáp dựa trên tài liệu học tập được chia sẻ bởi giáo viên.

### 📚 2. Quản lý tài liệu & Phân đoạn (Chunking)
- Upload tài liệu định dạng PDF (sử dụng thư viện **PdfPig** để trích xuất văn bản chất lượng cao).
- Cấu hình linh hoạt kích thước đoạn (`Chunk Size`) và độ chồng chéo (`Chunk Overlap`).
- Hỗ trợ nhiều chiến lược chunking thông minh nhằm bảo toàn ngữ nghĩa của tài liệu tốt nhất.

### 💬 3. Hệ thống Chatbot RAG thời gian thực
- Nhờ công nghệ **SignalR**, việc gửi nhận tin nhắn diễn ra tức thời mà không cần reload trang.
- Trả lời thông minh dựa trên ngữ cảnh được truy xuất (Retrieve) từ tài liệu đã được nhúng (Embedding) trong DB.
- **Cung cấp nguồn trích dẫn (Citations)**: Hiển thị rõ ràng đoạn văn bản gốc và tài liệu nguồn đã được dùng để AI sinh ra câu trả lời, giúp sinh viên kiểm chứng thông tin dễ dàng.
- Lưu trữ lịch sử chat theo từng chủ đề môn học.

### 📊 4. Nghiên cứu & Đánh giá hiệu năng (Benchmarking)
- Giáo viên/Admin có thể tạo các **Thực nghiệm (Experiments)** kết hợp giữa:
  - **Mô hình ngôn ngữ lớn (LLM)**: Gemini, OpenAI, HuggingFace, Ollama, Simulated Engine.
  - **Mô hình Embedding**: Mô hình nhúng của Google, OpenAI, Hugging Face, Ollama.
  - **Chiến lược Chunking**: Fixed Size, Paragraph, Sentence, Recursive.
- Đánh giá tự động cấu hình trên bằng cách chạy thử nghiệm các **Bộ đề mẫu (Test Sets)** của môn học (ví dụ câu hỏi PRN212, SWP391).
- Tính toán và lưu trữ các chỉ số đánh giá: **Cosine Similarity** (Độ tương đồng Vector), **Bleu Score** (Độ chính xác từ ngữ dịch thuật), **Retrieval Recall** (Khả năng truy xuất đúng thông tin).
- Trực quan hóa kết quả so sánh giữa các Experiment thông qua Dashboard biểu đồ cột và đường.

---

## 🛠️ Công nghệ sử dụng

- **Framework**: .NET 9.0 SDK
- **Web App**: ASP.NET Core Razor Pages
- **Database**: SQL Server 2019+
- **ORM**: Entity Framework Core 9.0
- **Real-time**: ASP.NET Core SignalR
- **Thư viện PDF**: PdfPig
- **Front-end**: Bootstrap 5, Bootstrap Icons, SweetAlert2, Chart.js, JQuery

---

## 🚀 Hướng dẫn cài đặt & Chạy ứng dụng

### 📋 Yêu cầu hệ thống
- Máy tính đã cài đặt **.NET 9.0 SDK**.
- **Microsoft SQL Server** đang hoạt động.
- (Tùy chọn) API Key của các dịch vụ: Gemini, OpenAI, HuggingFace (nếu muốn dùng mô hình online) hoặc Ollama (nếu muốn dùng mô hình offline).

### 🔧 Các bước thiết lập

#### Bước 1: Clone dự án hoặc tải mã nguồn về máy

#### Bước 2: Cấu hình ứng dụng
1. Đi tới thư mục dự án presentation: `PRN222_assigment2`.
2. Tạo file `appsettings.json` bằng cách sao chép từ file ví dụ:
   ```bash
   cp appsettings.json.example appsettings.json
   ```
3. Mở file `appsettings.json` vừa tạo và chỉnh sửa các thông tin:
   - **`ConnectionStrings:DefaultConnection`**: Cập nhật Connection String kết nối đến SQL Server của bạn.
   - **`AdminAccount`**: Cấu hình Email và Mật khẩu mặc định của tài khoản Admin khi seed dữ liệu.
   - **`GeminiSettings:ApiKey`**, **`HuggingFaceToken`**, **`OpenAIKey`**: Điền các khóa API tương ứng.
   - **`EmailSettings`**: Điền thông tin SMTP Gmail gửi mail thông báo mật khẩu cho sinh viên mới.

#### Bước 3: Cập nhật Cơ sở dữ liệu (EF Core Migrations)
Yêu cầu đã cài đặt công cụ `dotnet-ef`. Nếu chưa, hãy cài đặt bằng lệnh:
```bash
dotnet tool install --global dotnet-ef
```

Sau đó, tiến hành cập nhật cơ sở dữ liệu:
```bash
dotnet ef database update --project DataAccessLayer --startup-project PRN222_assigment2
```
*Lưu ý: Quá trình này sẽ chạy toàn bộ các migration và tự động cấu hình các bảng trong cơ sở dữ liệu SQL Server.*

#### Bước 4: Chạy ứng dụng
Chạy ứng dụng từ thư mục gốc của giải pháp (`solution directory`):
```bash
dotnet run --project PRN222_assigment2
```
Hoặc chuyển trực tiếp vào thư mục `PRN222_assigment2` và chạy:
```bash
dotnet run
```

Ứng dụng sẽ được khởi tạo tại địa chỉ mặc định: `https://localhost:7028` hoặc `http://localhost:5242`.

---

## 📝 Tài khoản đăng nhập mặc định (Seed Data)

Khi cơ sở dữ liệu được kết nối thành công lần đầu tiên, hệ thống sẽ tự động seed các dữ liệu sau:
- **Tài khoản Admin**:
  - **Email**: `admin@fpt.edu.vn` (hoặc email cấu hình trong `appsettings.json`)
  - **Mật khẩu**: `123456789` (hoặc mật khẩu cấu hình trong `appsettings.json`)
- **Dữ liệu câu hỏi test**: Tự động seed 50 câu hỏi test cho bộ môn PRN222/SWP391 và PRN212 từ các file JSON trong thư mục `Data`.

---

## 👥 Thành viên thực hiện
Dự án được phát triển bởi **Nhóm 4 - Lớp SE1938** phục vụ cho Assignment môn học PRN222.
