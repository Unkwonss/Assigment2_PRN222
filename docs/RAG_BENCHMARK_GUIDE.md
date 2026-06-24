# VietRAG System - Tài liệu Hướng dẫn RAG & Đánh giá Hiệu năng (Benchmarking Guide)

Hướng dẫn này cung cấp cái nhìn chi tiết về mặt lý thuyết và thực tiễn hoạt động của hệ thống RAG (Retrieval-Augmented Generation) và công cụ đánh giá hiệu năng (Benchmark) được phát triển trong dự án VietRAG. Tài liệu này đóng vai trò hướng dẫn kỹ thuật cho các lập trình viên phát triển hệ thống và giáo viên nghiên cứu.

---

## 📖 Mục lục
1. [Khái quát về Kiến trúc RAG (RAG Architecture Overview)](#1-khái-quát-về-kiến-trúc-rag-rag-architecture-overview)
2. [Chi tiết về các Chiến lược Phân đoạn (Chunking Strategies)](#2-chi-tiết-về-các-chiến-lược-phân-đoạn-chunking-strategies)
3. [Mô hình Vector hóa & Đại số So sánh (Embedding & Cosine Similarity)](#3-mô-hình-vector-hóa--đại-số-so-sánh-embedding--cosine-similarity)
4. [Quy trình Đánh giá Hiệu năng (Benchmarking Workflow)](#4-quy-trình-đánh-giá-hiệu-năng-benchmarking-workflow)
5. [Cấu hình Dịch vụ Trí tuệ Nhân tạo (AI Providers Configuration)](#5-cấu-hình-dịch-vụ-trí-tuệ-nhân-tạo-ai-providers-configuration)
6. [Xử lý Sự cố Thường gặp (Troubleshooting Guide)](#6-xử-lý-sự-cố-thường-gặp-troubleshooting-guide)

---

## 1. Khái quát về Kiến trúc RAG (RAG Architecture Overview)

Hệ thống RAG giải quyết điểm yếu lớn nhất của các Mô hình Ngôn ngữ Lớn (LLMs) – đó là **Sự ảo tưởng (Hallucination)** và **Thiếu kiến thức cập nhật hoặc kiến thức chuyên biệt của doanh nghiệp/trường học**. Quy trình RAG trong VietRAG diễn ra qua hai pha chính:

### A. Pha Lập Chỉ Mục (Indexing Phase - Offline)
```
[Tài liệu PDF/TXT] ──> [Trích xuất Văn bản (PdfPig)]
                               │
                               ▼
                    [Phân đoạn văn bản (Chunking)]
                               │
                               ▼
                [Vector hóa (Embedding Provider)]
                               │
                               ▼
              [Lưu trữ DB (SqlServer / Vector Store)]
```

### B. Pha Truy vấn & Sinh phản hồi (Query & Generation Phase - Real-time)
```
[Câu hỏi Người dùng] ──> [Vector hóa Câu hỏi]
                                 │
                                 ▼
                     [Truy xuất (Retrieval)] ──> So sánh Cosine Similarity với các Chunks trong DB
                                 │
                                 ▼
                      [Tạo ngữ cảnh (Context)] ──> Gom các Chunks có độ tương đồng cao nhất
                                 │
                                 ▼
                      [Sinh câu trả lời (LLM)] ──> Prompt: "Dựa vào ngữ cảnh sau hãy trả lời..."
                                 │
                                 ▼
                        [Hiện trích dẫn nguồn]
```

---

## 2. Chi tiết về các Chiến lược Phân đoạn (Chunking Strategies)

Chiến lược phân đoạn ảnh hưởng lớn nhất đến chất lượng dữ liệu được truy xuất. Hệ thống hỗ trợ 4 thuật toán chính:

### 📑 A. Fixed-Size Chunking (Phân đoạn kích thước cố định)
Cắt văn bản thành các đoạn có số ký tự cố định (`ChunkSize`), có chồng chéo (`ChunkOverlap`) giữa các đoạn để tránh mất mát thông tin ở ranh giới cắt.

**Mô hình cửa sổ trượt (Sliding Window):**
```
Đoạn 1: [--- Kích thước Chunk (ví dụ: 500 ký tự) ---]
                                  ⇅ (Chồng chéo - Overlap)
Đoạn 2:                     [--- Kích thước Chunk (500 ký tự) ---]
```
- **Ưu điểm**: Đơn giản, tốc độ xử lý nhanh nhất.
- **Nhược điểm**: Có thể cắt ngang từ hoặc câu, làm vỡ cấu trúc ngữ nghĩa.

### 📝 B. Paragraph Chunking (Phân đoạn theo đoạn văn)
Sử dụng các ký tự xuống dòng (`\n\n`, `\r\n\r\n`) để phân chia văn bản thành các đoạn văn tự nhiên.
- **Ưu điểm**: Giữ nguyên tính toàn vẹn của một ý tưởng lớn do tác giả trình bày.
- **Nhược điểm**: Kích thước các đoạn không đều nhau, đoạn văn quá dài vượt quá ngữ cảnh, đoạn quá ngắn làm nhiễu dữ liệu.

### 💬 C. Sentence Chunking (Phân đoạn theo câu)
Sử dụng các dấu kết thúc câu (`.`, `?`, `!`) để chia nhỏ văn bản. Sau đó, ghép các câu lại với nhau sao cho tổng số ký tự của một chunk gần bằng và không vượt quá `ChunkSize`.
- **Ưu điểm**: Giữ trọn cấu trúc ngữ pháp và ý nghĩa của từng phát biểu đơn lẻ.
- **Nhược điểm**: Yêu cầu xử lý ngôn ngữ tự nhiên tốt để tránh nhận diện nhầm dấu chấm viết tắt (ví dụ: "TP. HCM", "Dr. Jones").

### 🌳 D. Recursive Character Chunking (Phân đoạn đệ quy)
Chiến lược khuyên dùng cho hầu hết các tác vụ RAG. Thuật toán cố gắng chia nhỏ văn bản theo danh sách ký tự ưu tiên giảm dần: `["\n\n", "\n", " ", ""]`.
1. Hệ thống sẽ cố gắng chia văn bản thành các đoạn lớn dựa trên ranh giới đoạn văn (`\n\n`).
2. Nếu đoạn nào vẫn lớn hơn `ChunkSize`, hệ thống đệ quy chia tiếp đoạn đó theo ranh giới dòng (`\n`).
3. Nếu vẫn quá lớn, tiếp tục chia theo từ (` `) và cuối cùng là theo từng ký tự (``).
- **Ưu điểm**: Bảo toàn cấu trúc tài liệu một cách tối đa, giảm thiểu việc vỡ câu và giữ kích thước các đoạn đồng đều nhất có thể.

---

## 3. Mô hình Vector hóa & Đại số So sánh (Embedding & Cosine Similarity)

### A. Embedding Models
Mô hình nhúng biến các đoạn văn bản (Chunks) thành các chuỗi số thực (Vector) đại diện cho tọa độ ngữ nghĩa của đoạn văn đó trong không gian n-chiều.
- **Gemini Embeddings**: Vector 768 chiều, tối ưu hóa cho đa ngôn ngữ và các tác vụ của Google.
- **OpenAI Ada-002 / text-embedding-3-small**: Vector 1536 chiều, chất lượng ngữ nghĩa hàng đầu thế giới.
- **HuggingFace (cohere-multilingual, v.v.)**: Vector cấu hình linh hoạt từ 384 đến 1024 chiều.
- **Ollama (Nomic-embed-text)**: Vector nhúng chạy local cực kỳ bảo mật và miễn phí.

### B. So sánh Cosine Similarity
Độ tương đồng ngữ nghĩa giữa Câu hỏi (Query) và Đoạn văn bản (Chunk) được đo bằng cosin của góc giữa hai vector trong không gian đa chiều. Công thức toán học:

$$\text{Cosine Similarity}(A, B) = \frac{A \cdot B}{\|A\| \|B\|} = \frac{\sum_{i=1}^{n} A_i B_i}{\sqrt{\sum_{i=1}^{n} A_i^2} \sqrt{\sum_{i=1}^{n} B_i^2}}$$

- **Giá trị gần 1**: Hai đoạn văn bản có ý nghĩa ngữ nghĩa rất gần nhau (dù từ ngữ viết có thể khác nhau).
- **Giá trị gần 0**: Hai đoạn văn bản hoàn toàn không liên quan đến nhau.
- **Giá trị gần -1**: Có ý nghĩa trái ngược nhau hoàn toàn.

---

## 4. Quy trình Đánh giá Hiệu năng (Benchmarking Workflow)

Đánh giá hiệu năng của RAG là tính năng đột phá của VietRAG, giúp đo lường định lượng xem cấu hình nào hoạt động tốt nhất.

### 📈 Các chỉ số đánh giá (Metrics)
1. **Cosine Similarity (Độ tương đồng Vector)**: Đo độ khớp về mặt ngữ nghĩa giữa câu trả lời sinh ra bởi AI và Câu trả lời chuẩn (Ground Truth).
2. **Bleu Score (Bilingual Evaluation Understudy)**: So sánh độ trùng khớp từng từ (n-gram) giữa câu trả lời sinh ra và câu trả lời chuẩn. Rất khắt khe về mặt từ vựng.
3. **Retrieval Recall**: Đo lường tỷ lệ các tài liệu/chương học chính xác được hệ thống truy xuất lên thành công so với tài liệu gốc chứa đáp án.

### 🔄 Các bước thực hiện thực nghiệm (Experiment Pipeline)
```
[Chọn Experiment Cấu hình] ──> [Lấy bộ đề TestSet của Môn học]
                                          │
                                          ▼
                             [Chạy tự động cho từng câu]
                                          │
                   ┌──────────────────────┴──────────────────────┐
                   ▼                                             ▼
          [Truy xuất Context]                            [Gọi API sinh đáp án]
                   │                                             │
                   └──────────────────────┬──────────────────────┘
                                          ▼
                              [Tính toán chỉ số Metric]
                                          │
                                          ▼
                            [Lưu kết quả BenchmarkResult]
                                          │
                                          ▼
                            [Trực quan hóa trên Dashboard]
```

---

## 5. Cấu hình Dịch vụ Trí tuệ Nhân tạo (AI Providers Configuration)

Hệ thống hỗ trợ cấu hình đa dạng các Provider trong tệp `appsettings.json`.

### 🟢 Cấu hình API Gemini (Mặc định)
Hệ thống sử dụng HttpClient của .NET để gọi trực tiếp tới Google Gemini API:
```json
"GeminiSettings": {
  "ApiKey": "AIzaSy...",
  "Model": "gemini-2.5-flash",
  "BaseUrl": "https://generativelanguage.googleapis.com/v1beta/models",
  "MaxOutputTokens": 1024,
  "Temperature": 0.3
}
```

### 🔴 Cấu hình API OpenAI
Sử dụng cho các mô hình GPT-4o, GPT-3.5-Turbo hoặc Embedding models:
```json
"OpenAIKey": "sk-proj-...",
```

### 🔵 Cấu hình Ollama (Chạy Offline)
Ollama cho phép chạy các mô hình nguồn mở như Llama 3, Phi-3, Qwen cục bộ trên máy tính của bạn:
```json
"Ollama": {
  "Enabled": "true",
  "Url": "http://localhost:11434/api/generate",
  "Model": "llama3"
}
```
*Yêu cầu*: Máy tính cần cài ứng dụng Ollama và kéo mô hình về bằng lệnh `ollama run llama3`.

---

## 6. Xử lý Sự cố Thường gặp (Troubleshooting Guide)

### ❌ 1. Lỗi kết nối Cơ sở dữ liệu (Database Connection Failures)
* **Triệu chứng**: Gặp lỗi `SqlException` hoặc `Cannot connect to database` khi chạy chương trình hoặc chạy migration.
* **Cách khắc phục**:
  1. Kiểm tra lại chuỗi kết nối `"DefaultConnection"` trong `appsettings.json`.
  2. Đảm bảo dịch vụ SQL Server (MSSQLSERVER) đang chạy trong hệ thống Windows Services.
  3. Nếu sử dụng SQL Express, thay đổi `Server=localhost` hoặc `Server=YOUR_PC\SQLEXPRESS`.
  4. Đảm bảo cấu hình `TrustServerCertificate=True` có trong chuỗi kết nối.

### ⏳ 2. Lỗi Hết hạn Yêu cầu API (API Request Timeout)
* **Triệu chứng**: Lỗi `TaskCanceledException` hoặc `Gateway Timeout` khi tải tài liệu lớn hoặc chạy benchmark.
* **Cách khắc phục**:
  1. Giới hạn số lượng tài liệu hoặc kích thước tài liệu PDF tải lên dưới 10MB.
  2. Tăng giá trị timeout HttpClient trong tệp `Program.cs` (mặc định đã cấu hình từ 30 đến 60 giây).
  3. Kiểm tra kết nối mạng Internet hoặc trạng thái hoạt động của nhà cung cấp API (Gemini/OpenAI status page).

### 🔑 3. Lỗi Khóa API không hợp lệ (Unauthorized / Invalid API Key)
* **Triệu chứng**: Hệ thống trả về câu trả lời rỗng hoặc lỗi `401 Unauthorized` / `403 Forbidden`.
* **Cách khắc phục**:
  1. Đảm bảo khóa API của bạn không chứa ký tự khoảng trắng thừa.
  2. Đảm bảo thẻ tín dụng hoặc hạn mức tài khoản API của bạn vẫn còn tiền/lượt gọi khả dụng.

### ⚙️ 4. Lỗi Phiên bản EF Core Tool (ef Command Not Found)
* **Triệu chứng**: Chạy lệnh `dotnet ef` báo lỗi lệnh không tồn tại.
* **Cách khắc phục**:
  1. Chạy lệnh cài đặt công cụ toàn cục: `dotnet tool install --global dotnet-ef`
  2. Nếu đã cài nhưng báo lỗi cũ, hãy thử cập nhật: `dotnet tool update --global dotnet-ef`

---

## 7. Chi tiết Cấu trúc CSDL & Quan hệ (Database Entity Relationships)

Cơ sở dữ liệu của `VietRAG` được chuẩn hóa cao nhằm đáp ứng các yêu cầu khắt khe của việc theo dõi lịch sử chat, các phiên bản lập chỉ mục tài liệu khác nhau, và lưu trữ kết quả đánh giá thực nghiệm.

### 📊 Mô tả các Bảng dữ liệu chính

1. **`Users` (Người dùng)**:
   - Lưu trữ danh tính người dùng trong hệ thống.
   - Phân biệt vai trò bằng trường `Role` (`Admin`, `Teacher`, `Student`).
   - Mật khẩu lưu trữ dạng chuỗi băm SHA-256 an toàn.

2. **`Subjects` (Môn học)**:
   - Quản lý các môn học đại diện cho dữ liệu học tập cần RAG.
   - Quản lý bởi một Giáo viên chủ nhiệm thông qua khóa ngoại `ManagedByUserId`.

3. **`SubjectTeachers` (Bảng liên kết Giáo viên - Môn học)**:
   - Liên kết nhiều - nhiều biểu diễn các giáo viên phụ trách cùng giảng dạy một môn học.

4. **`Chapters` (Chương học)**:
   - Một môn học gồm nhiều chương học. Cấu trúc này giúp RAG thu hẹp phạm vi tìm kiếm tài liệu theo đúng chương sinh viên đang học.

5. **`Documents` (Tài liệu)**:
   - Tài liệu học tập (chủ yếu là PDF) được tải lên bởi giáo viên.
   - Liên kết trực tiếp với chương học thông qua `ChapterId`.
   - Trạng thái `Status` chuyển từ `Pending` sang `Processed` sau khi hoàn tất trích xuất chữ.

6. **`DocumentIndexes` (Bản ghi chỉ mục)**:
   - Đại diện cho một cấu hình lập chỉ mục cụ thể của tài liệu.
   - Một tài liệu có thể có nhiều bản ghi chỉ mục nếu được thử nghiệm với các cấu hình khác nhau (ví dụ: mô hình Embedding A + ChunkSize 500 so với mô hình Embedding B + ChunkSize 1000).

7. **`DocumentChunks` (Các đoạn văn bản)**:
   - Lưu trữ nội dung văn bản thực tế sau khi bị phân đoạn.
   - Lưu trữ khóa tham chiếu vector store `VectorStoreKey` hoặc biểu diễn vector dạng chuỗi JSON `EmbeddingVector`.

8. **`ChatSessions` & `ChatHistories`**:
   - Quản lý các cuộc trò chuyện học tập của người dùng.
   - Mỗi câu hỏi và câu trả lời trong `ChatHistories` liên kết với nhiều `ChatCitations` để hiển thị căn cứ tài liệu.

9. **`Experiments` (Thực nghiệm)**:
   - Định nghĩa cấu hình tham số RAG bao gồm mô hình ngôn ngữ lớn (LLM), mô hình Embedding, chiến lược Chunking, kích thước đoạn và độ chồng chéo.

10. **`TestSets` (Bộ câu hỏi mẫu)**:
    - Lưu trữ danh sách câu hỏi và câu trả lời chuẩn (Ground Truth) dùng để đánh giá hiệu năng hệ thống.

11. **`BenchmarkResults` (Kết quả đánh giá)**:
    - Điểm số tự động tính toán sau khi chạy thực nghiệm `Experiment` trên bộ câu hỏi `TestSet`.

---

## 8. Quy trình Sinh Prompt & Cấu trúc Prompt mẫu (Prompt Engineering & Template Examples)

VietRAG áp dụng quy trình thiết kế prompt có cấu trúc chặt chẽ để đảm bảo mô hình AI phản hồi chính xác và chỉ trích dẫn thông tin có thật trong tài liệu.

### 📝 Prompt mẫu sinh câu trả lời chatbot (C# Implementation Example)

```csharp
public string BuildRAGPrompt(string question, List<string> retrievedChunks)
{
    var contextBuilder = new System.Text.StringBuilder();
    for (int i = 0; i < retrievedChunks.Count; i++)
    {
        contextBuilder.AppendLine($"[Tài liệu tham khảo {i + 1}]: {retrievedChunks[i]}");
        contextBuilder.AppendLine("--------------------------------------------------");
    }

    string systemInstruction = 
        "Bạn là một trợ lý ảo học thuật thông minh của Đại học FPT (VietRAG System).\n" +
        "Nhiệm vụ của bạn là trả lời câu hỏi của sinh viên một cách chính xác, ngắn gọn và chuyên nghiệp.\n" +
        "Yêu cầu bắt buộc:\n" +
        "1. CHỈ sử dụng thông tin được cung cấp trong các 'Tài liệu tham khảo' dưới đây để trả lời câu hỏi.\n" +
        "2. Nếu tài liệu tham khảo không chứa đủ thông tin để trả lời, hãy trả lời lịch sự: " +
        "'Rất tiếc, tài liệu học tập hiện tại chưa có thông tin chi tiết để trả lời câu hỏi này. Bạn hãy liên hệ giáo viên để được hỗ trợ.' " +
        "Tuyệt đối không tự bịa ra câu trả lời.\n" +
        "3. Khi sử dụng thông tin từ tài liệu tham khảo nào, hãy ghi rõ nguồn bằng cách chỉ ra số thứ tự của tài liệu đó ở cuối câu (ví dụ: [Tài liệu tham khảo 1]).";

    string finalPrompt = 
        $"{systemInstruction}\n\n" +
        $"== DANH SÁCH TÀI LIỆU THAM KHẢO ==\n" +
        $"{contextBuilder}\n" +
        $"== CÂU HỎI CỦA SINH VIÊN ==\n" +
        $"Câu hỏi: {question}\n\n" +
        $"Câu trả lời của bạn:";

    return finalPrompt;
}
```

---

## 9. Cách thức Tính toán Điểm số BLEU & Cosine Similarity trong C# (Algorithmic Implementation in C#)

### 📐 A. Thuật toán tính độ tương đồng Vector (Cosine Similarity)

Trong `BusinessLayer`, độ tương đồng giữa vector câu hỏi và vector của các chunk tài liệu được tính toán thông qua tích vô hướng của hai mảng float:

```csharp
public static float CalculateCosineSimilarity(float[] vectorA, float[] vectorB)
{
    if (vectorA.Length != vectorB.Length)
        throw new ArgumentException("Độ dài hai vector phải bằng nhau.");

    float dotProduct = 0f;
    float normA = 0f;
    float normB = 0f;

    for (int i = 0; i < vectorA.Length; i++)
    {
        dotProduct += vectorA[i] * vectorB[i];
        normA += vectorA[i] * vectorA[i];
        normB += vectorB[i] * vectorB[i];
    }

    if (normA == 0f || normB == 0f)
        return 0f; // Tránh lỗi chia cho 0

    return dotProduct / (MathF.Sqrt(normA) * MathF.Sqrt(normB));
}
```

### 📝 B. Thuật toán đánh giá sự tương quan từ vựng (BLEU-1 Score Simplification)

Dưới đây là phiên bản đơn giản hóa tính toán độ khớp từ vựng đơn (Unigram BLEU-1) giữa chuỗi kết quả của AI và chuỗi câu trả lời chuẩn (Ground Truth):

```csharp
public static double CalculateBleu1(string generatedText, string groundTruthText)
{
    if (string.IsNullOrWhiteSpace(generatedText) || string.IsNullOrWhiteSpace(groundTruthText))
        return 0.0;

    // Chuẩn hóa và tách từ (Word Tokenization)
    char[] separators = new char[] { ' ', '.', ',', '?', '!', ';', ':', '-', '\n', '\r' };
    string[] genTokens = generatedText.ToLower().Split(separators, StringSplitOptions.RemoveEmptyEntries);
    string[] refTokens = groundTruthText.ToLower().Split(separators, StringSplitOptions.RemoveEmptyEntries);

    if (genTokens.Length == 0 || refTokens.Length == 0)
        return 0.0;

    // Đếm tần suất xuất hiện các từ trong Ground Truth
    var refCounts = new Dictionary<string, int>();
    foreach (var token in refTokens)
    {
        if (refCounts.ContainsKey(token))
            refCounts[token]++;
        else
            refCounts[token] = 1;
    }

    // Đếm số từ trùng khớp được giới hạn (Clipped Precision)
    int matches = 0;
    var genCounts = new Dictionary<string, int>();
    foreach (var token in genTokens)
    {
        if (refCounts.ContainsKey(token))
        {
            if (genCounts.ContainsKey(token))
            {
                if (genCounts[token] < refCounts[token])
                {
                    genCounts[token]++;
                    matches++;
                }
            }
            else
            {
                genCounts[token] = 1;
                matches++;
            }
        }
    }

    // Tính độ chính xác của unigram
    double precision = (double)matches / genTokens.Length;

    // Tính hình phạt độ dài ngắn (Brevity Penalty - BP)
    double bp = 1.0;
    if (genTokens.Length < refTokens.Length)
    {
        bp = Math.Exp(1.0 - ((double)refTokens.Length / genTokens.Length));
    }

    return bp * precision;
}
```

---

*Tài liệu kỹ thuật được duy trì bởi nhóm nghiên cứu phát triển VietRAG 2026.*
