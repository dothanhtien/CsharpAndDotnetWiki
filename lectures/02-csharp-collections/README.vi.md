# C# Collections

Bài giảng chi tiết về các loại collection trong `System.Collections.Generic`: `List<T>`, `Dictionary<TKey,TValue>`, `HashSet<T>`, `Queue<T>`, `Stack<T>`, `LinkedList<T>`, các interface liên kết chúng lại với nhau, và các method LINQ dùng để truy vấn chúng. Mỗi khái niệm bên dưới đều có ví dụ chạy được tương ứng trong [src/Program.cs](./src/Program.cs).

> Bài giảng này tiếp nối [01-csharp-basics](../01-csharp-basics/README.vi.md), nơi đã giới thiệu sơ qua mảng và `List<T>`. Ở đây chúng ta đi sâu hơn nhiều, và tìm hiểu thêm các loại collection dựng sẵn còn lại.

## Mục tiêu

- Biết khi nào nên dùng `List<T>` so với mảng và các loại collection khác.
- Dùng `Dictionary<TKey,TValue>` cho việc tra cứu key → value một cách an toàn (`TryGetValue`).
- Dùng `HashSet<T>` để kiểm tra tính duy nhất và thực hiện các phép toán tập hợp (hợp, giao, hiệu).
- Dùng `Queue<T>` (FIFO) và `Stack<T>` (LIFO) cho các bài toán xử lý nhạy cảm với thứ tự.
- Hiểu `LinkedList<T>` đánh đổi điều gì so với `List<T>`, và khi nào sự đánh đổi đó xứng đáng.
- Hiểu các interface của collection (`IEnumerable<T>`, `ICollection<T>`, `IList<T>`) và vì sao dùng interface hẹp nhất có thể giúp method tái sử dụng tốt hơn.
- Viết truy vấn LINQ với `Where`, `Select`, `OrderBy`, `First`/`FirstOrDefault`, `Any`/`All`, `Sum`/`Min`/`Max`, và `GroupBy`.
- Hiểu deferred execution (thực thi trì hoãn) - vì sao một truy vấn LINQ không chạy cho đến khi bạn enumerate nó.
- So sánh các collection theo độ phức tạp thời gian cho các thao tác bạn thực sự dùng.

## 1. Mảng - ôn lại

Mảng có kích thước cố định và cho phép truy cập theo chỉ số (index) nhanh nhất có thể, nhưng không thể tăng/giảm kích thước sau khi tạo:

```csharp
int[] fixedScores = { 90, 85, 77 };
fixedScores.Length; // 3, và không bao giờ thay đổi được
```

Mảng là công cụ phù hợp khi kích thước thực sự cố định (bàn cờ 3x3, giá trị RGB). Với hầu hết trường hợp còn lại - một collection có kích thước thay đổi trong suốt vòng đời chương trình - hãy dùng `List<T>` thay thế.

## 2. `List<T>`

`List<T>` là một collection có thứ tự, có thể thay đổi kích thước, truy cập được theo chỉ số - lựa chọn mặc định cho "một tập hợp các phần tử cùng kiểu."

```csharp
var names = new List<string> { "Alice", "Bob" };
names.Add("Carol");        // thêm vào cuối - O(1) amortized (trung bình)
names.Insert(1, "Dave");   // chèn vào một vị trí - O(n), phải dịch các phần tử phía sau
names.Remove("Bob");       // xóa theo giá trị - O(n), tìm rồi dịch chuyển
names[0];                  // truy cập theo chỉ số - O(1), giống mảng
names.Contains("Carol");   // O(n) - phải kiểm tra từng phần tử
```

Bên trong, `List<T>` được hỗ trợ bởi một mảng tự động thay đổi kích thước (tăng gấp đôi) khi cần - đó là lý do `Add` nhanh trung bình dù thỉnh thoảng mảng bên dưới phải được cấp phát lại và sao chép.

## 3. `Dictionary<TKey, TValue>`

Dictionary lưu các cặp key → value và cho tốc độ tra cứu, thêm, xóa theo key ở mức **O(1)** (trung bình) - nhanh hơn rất nhiều so với việc tìm một phần tử khớp trong `List<T>` khi bạn có một key tự nhiên.

```csharp
var ages = new Dictionary<string, int>
{
    ["Alice"] = 30,
    ["Bob"] = 25,
};

ages["Carol"] = 28; // thêm key mới
ages["Alice"] = 31; // ghi đè giá trị của key đã tồn tại

if (ages.TryGetValue("Bob", out var bobAge))
{
    Console.WriteLine(bobAge);
}
```

Ưu tiên `TryGetValue` thay vì truy cập bằng chỉ số (`ages["Missing"]`) bất cứ khi nào key có thể không tồn tại - truy cập bằng chỉ số ném `KeyNotFoundException`, còn `TryGetValue` chỉ trả về `false`. Dictionary không đảm bảo thứ tự khi duyệt; đừng bao giờ trông cậy vào thứ tự bạn nhận được từ `foreach`.

## 4. `HashSet<T>`

Set lưu các giá trị duy nhất, không có phần tử trùng lặp, và `Contains` có độ phức tạp **O(1)** (trung bình) - nhanh hơn nhiều so với `List<T>.Contains`, vốn phải quét qua từng phần tử.

```csharp
var uniqueTags = new HashSet<string> { "csharp", "dotnet" };
bool added = uniqueTags.Add("csharp"); // false - "csharp" đã tồn tại, set không đổi
```

`HashSet<T>` cũng hỗ trợ trực tiếp các phép toán tập hợp cổ điển:

```csharp
var setA = new HashSet<int> { 1, 2, 3 };
var setB = new HashSet<int> { 2, 3, 4 };

var intersection = new HashSet<int>(setA);
intersection.IntersectWith(setB); // {2, 3} - có trong cả hai

var union = new HashSet<int>(setA);
union.UnionWith(setB);            // {1, 2, 3, 4} - có trong ít nhất một

var difference = new HashSet<int>(setA);
difference.ExceptWith(setB);      // {1} - có trong setA nhưng không có trong setB
```

Dùng `HashSet<T>` bất cứ khi nào câu hỏi bạn cần trả lời là "phần tử này đã xuất hiện chưa?" hoặc "phần tử này có trong collection không?" - nó biến một lượt quét O(n) thành một lần tra cứu O(1).

## 5. `Queue<T>` (FIFO)

Queue xử lý các phần tử theo đúng thứ tự chúng đến - **F**irst **I**n, **F**irst **O**ut (vào trước, ra trước). Hãy hình dung một hàng người đang xếp hàng chờ.

```csharp
var ticketQueue = new Queue<string>();
ticketQueue.Enqueue("ticket-1");
ticketQueue.Enqueue("ticket-2");

ticketQueue.Peek();    // "ticket-1" - xem phần tử đầu mà không xóa nó
ticketQueue.Dequeue(); // "ticket-1" - xóa và trả về phần tử đầu
```

Ứng dụng thường gặp: hàng đợi công việc/task, duyệt theo chiều rộng (breadth-first traversal), bất cứ bài toán nào mô hình hóa "xử lý theo thứ tự đến trước."

## 6. `Stack<T>` (LIFO)

Stack xử lý các phần tử theo thứ tự ngược lại với lúc chúng đến - **L**ast **I**n, **F**irst **O**ut (vào sau, ra trước). Hãy hình dung một chồng đĩa.

```csharp
var undoStack = new Stack<string>();
undoStack.Push("type 'Hello'");
undoStack.Push("type ' World'");

undoStack.Peek(); // "type ' World'" - xem phần tử trên cùng mà không xóa nó
undoStack.Pop();  // "type ' World'" - xóa và trả về phần tử trên cùng
```

Ứng dụng thường gặp: lịch sử undo/redo, tính toán biểu thức, duyệt theo chiều sâu (depth-first traversal), backtracking, chính call stack của chương trình.

## 7. `LinkedList<T>`

`LinkedList<T>` là một danh sách liên kết đôi (doubly-linked list): mỗi node biết các node lân cận của nó. Việc chèn hoặc xóa một node mà bạn đã có tham chiếu tới là **O(1)**, bất kể nó ở đâu trong danh sách - trong khi `List<T>.Insert`/`Remove` ở giữa danh sách là O(n) vì phải dịch chuyển mọi phần tử phía sau.

```csharp
var timeline = new LinkedList<string>();
var noon = timeline.AddLast("12:00 lunch");
timeline.AddAfter(noon, "13:00 meeting"); // O(1) - đã có sẵn node
timeline.AddFirst("09:00 stand-up");
```

Đánh đổi: `LinkedList<T>` **không có truy cập theo chỉ số** (`list[5]` không tồn tại - muốn tới node thứ 5 phải đi qua 5 liên kết, O(n)), tốn bộ nhớ hơn cho mỗi phần tử (mỗi node lưu thêm hai con trỏ), và nhìn chung duyệt chậm hơn `List<T>` do tính cục bộ bộ nhớ đệm (cache locality) kém. Trong thực tế, `List<T>` gần như luôn là lựa chọn mặc định tốt hơn; chỉ dùng `LinkedList<T>` khi bạn thực sự cần chèn/xóa thường xuyên tại các vị trí tùy ý mà bạn đã biết trước.

## 8. Các interface của collection

Tất cả các collection generic đều implement một hệ thống interface dùng chung:

```
IEnumerable<T>  - duyệt được bằng foreach (chỉ đọc, chỉ đi tới)
    ↑
ICollection<T>  - + Count, Add, Remove, Contains
    ↑
IList<T>        - + truy cập theo chỉ số (this[int]), Insert, RemoveAt
```

`Dictionary<TKey,TValue>` và `HashSet<T>` implement `ICollection<T>` nhưng không implement `IList<T>` (không có chỉ số có ý nghĩa). `Queue<T>` và `Stack<T>` còn hẹp hơn nữa một cách chủ ý - chúng chỉ công bố các thao tác hợp lý cho truy cập kiểu FIFO/LIFO.

Viết một method dựa trên **interface hẹp nhất mà nó thực sự cần** giúp method đó hoạt động với mọi thứ thỏa mãn interface đó - một list, một mảng, một set, hay kết quả của một truy vấn LINQ:

```csharp
static void PrintAll(IEnumerable<string> items) => Console.WriteLine(string.Join(" | ", items));

PrintAll(names);                                   // một List<string>
PrintAll(fixedScores.Select(s => s.ToString()));   // kết quả truy vấn LINQ - cũng là IEnumerable<string>
```

Nếu `PrintAll` yêu cầu tham số kiểu `List<string>` thay vì `IEnumerable<string>`, lời gọi thứ hai sẽ không biên dịch được - một ràng buộc không cần thiết, vì method này chưa bao giờ dùng gì vượt quá những gì `IEnumerable<T>` đã cung cấp sẵn.

## 9. LINQ cơ bản

LINQ (Language Integrated Query) thêm các method kiểu truy vấn vào bất kỳ `IEnumerable<T>` nào - lọc, chiếu (project), sắp xếp, và tổng hợp mà không cần viết vòng lặp thủ công.

```csharp
record Product(string Name, decimal Price, string Category);

var products = new List<Product>
{
    new("Laptop", 1500m, "Electronics"),
    new("Mouse", 25m, "Electronics"),
    new("Desk", 300m, "Furniture"),
};

var result = products
    .Where(p => p.Category == "Electronics" && p.Price > 100)  // lọc
    .OrderBy(p => p.Price)                                      // sắp xếp tăng dần
    .Select(p => $"{p.Name} (${p.Price})")                      // chiếu sang hình dạng mới
    .ToList();                                                  // hiện thực hóa thành List<string>
```

Các thao tác LINQ thường dùng, phân theo mục đích:

| Mục đích              | Method                                                                   |
| --------------------- | ------------------------------------------------------------------------ |
| Lọc                   | `Where`                                                                  |
| Chiếu (đổi hình dạng) | `Select`                                                                 |
| Sắp xếp               | `OrderBy`, `OrderByDescending`, `ThenBy`                                 |
| Chọn một phần tử      | `First`, `FirstOrDefault`, `Single`, `SingleOrDefault`, `MinBy`, `MaxBy` |
| Kiểm tra              | `Any`, `All`, `Contains`                                                 |
| Tổng hợp              | `Count`, `Sum`, `Average`, `Min`, `Max`, `Aggregate`                     |
| Gom nhóm              | `GroupBy`                                                                |
| Hiện thực hóa         | `ToList`, `ToArray`, `ToDictionary`, `ToHashSet`                         |

`First` ném exception nếu không có phần tử nào khớp; `FirstOrDefault` trả về `default` (ví dụ `null` với kiểu tham chiếu) thay vì ném exception - ưu tiên `FirstOrDefault` bất cứ khi nào "không tìm thấy" là một kết quả bình thường, được dự tính trước, chứ không phải lỗi.

### Deferred execution (thực thi trì hoãn)

Hầu hết method LINQ (`Where`, `Select`, `OrderBy`, ...) là **lazy** (trì hoãn): việc xây dựng truy vấn không chạy nó. Truy vấn chỉ thực sự chạy khi bạn enumerate nó - bằng `foreach`, hoặc một lời gọi hiện thực hóa như `ToList()`/`ToArray()`/`Count()`.

```csharp
var query = products.Where(p => LogAndCheck(p)); // chưa chạy gì cả - chỉ mô tả truy vấn
Console.WriteLine("Query built, not yet run...");
var materialized = query.ToList();               // BÂY GIỜ predicate mới chạy cho từng phần tử
```

Điều này quan trọng trong thực tế: nếu bạn xây dựng một truy vấn một lần rồi enumerate nó hai lần, toàn bộ pipeline sẽ chạy hai lần; và nếu collection bên dưới thay đổi giữa lúc xây dựng truy vấn và lúc enumerate, lần enumerate sau sẽ thấy dữ liệu đã thay đổi. Gọi `.ToList()` một lần khi bạn cần một bản snapshot ổn định.

## Chọn đúng collection

| Nhu cầu                                                    | Dùng                      | Độ phức tạp thường gặp                               |
| ---------------------------------------------------------- | ------------------------- | ---------------------------------------------------- |
| Có thứ tự, truy cập theo chỉ số, thay đổi kích thước được  | `List<T>`                 | Add: O(1)\*, index: O(1), insert/remove ở giữa: O(n) |
| Kích thước cố định, truy cập chỉ số nhanh nhất             | Mảng (`T[]`)              | Index: O(1), không thay đổi kích thước được          |
| Tra cứu key → value                                        | `Dictionary<TKey,TValue>` | Get/Add/Remove: O(1)\*                               |
| Tính duy nhất / phép toán tập hợp                          | `HashSet<T>`              | Contains/Add/Remove: O(1)\*                          |
| Xử lý theo thứ tự đến trước                                | `Queue<T>`                | Enqueue/Dequeue: O(1)                                |
| Xử lý phần tử mới nhất trước                               | `Stack<T>`                | Push/Pop: O(1)                                       |
| Chèn/xóa thường xuyên tại vị trí đã biết, không cần chỉ số | `LinkedList<T>`           | Insert/remove tại một node: O(1), index: O(n)        |

\*O(1) cho dictionary/set/list-append là "amortized" (trung bình) - thỉnh thoảng một lần resize nội bộ tốn O(n), nhưng tính trung bình trên nhiều thao tác thì vẫn ra O(1).

## Lỗi thường gặp

- **Dùng `List<T>.Contains` trong vòng lặp chạy nhiều lần** trên một list lớn - mỗi lần gọi là O(n); nếu phải kiểm tra sự tồn tại nhiều lần, hãy dùng `HashSet<T>` thay thế.
- **Truy cập `Dictionary<TKey,TValue>` bằng chỉ số cho một key có thể không tồn tại** - `ages["Missing"]` ném `KeyNotFoundException`; hãy dùng `TryGetValue` hoặc kiểm tra `ContainsKey` trước.
- **Trông cậy vào thứ tự duyệt của dictionary/set** - thứ tự này không được đảm bảo và có thể thay đổi; nếu cần thứ tự, hãy sắp xếp tường minh hoặc dùng cấu trúc có thứ tự như `List<T>` hoặc `SortedDictionary<TKey,TValue>`.
- **Thay đổi một collection trong khi đang duyệt nó** bằng `foreach` sẽ ném `InvalidOperationException` - hãy gom các thay đổi cần thực hiện lại, rồi áp dụng sau vòng lặp (hoặc duyệt trên một bản sao bằng `.ToList()`).
- **Enumerate một truy vấn LINQ nhiều lần** mà không nhận ra rằng nó chạy lại toàn bộ pipeline mỗi lần - gọi `.ToList()` một lần nếu cần dùng lại kết quả.
- **Mặc định chọn `LinkedList<T>`** vì "nghe có vẻ hiệu quả" - với hầu hết khối lượng công việc, `List<T>` nhanh hơn trong thực tế nhờ tính cục bộ bộ nhớ đệm; chỉ dùng `LinkedList<T>` khi thực sự cần chèn/xóa O(1) tại các node đã biết trước.

## Bài tập

1. Cho một `List<int>` có phần tử trùng lặp, dùng `HashSet<int>` để lấy ra các giá trị duy nhất, rồi so sánh kết quả với `.Distinct()` sẵn có của LINQ.
2. Xây dựng một `Dictionary<string, List<string>>` gom nhóm một danh sách các tuple `(city, name)` theo thành phố, không dùng `GroupBy` - sau đó làm lại bằng `GroupBy` và so sánh.
3. Mô phỏng một hàng đợi in ấn đơn giản: `Enqueue` năm công việc, `Dequeue` từng cái một, in ra mỗi khi được xử lý.
4. Cài đặt kiểm tra "dấu ngoặc cân bằng" (`"(()())"` → true, `"(()"` → false) bằng `Stack<char>`.
5. Viết một truy vấn LINQ trên `List<Product>` trả về tên sản phẩm rẻ nhất trong mỗi category, dùng `GroupBy` kết hợp `Select`.

## Chạy thử project

```bash
cd lectures/02-csharp-collections/src
dotnet run
```

## Ghi chú

- Xem file [src/Program.cs](./src/Program.cs) để có ví dụ chạy được đầy đủ cho tất cả các phần ở trên, theo đúng thứ tự.
- Các khái niệm OOP được nhắc tới ở đây (như `record Product`) được trình bày chi tiết trong [03-csharp-oop](../03-csharp-oop/README.vi.md).
