using ScheduleYourLife;

Console.WriteLine("=== SCHEDULE YOUR LIFE - SQLITE DATABASE TEST ===");
Console.WriteLine();

// 1. Khởi tạo kết nối CSDL và tự động tạo file schedule.db nếu chưa có
using (var db = new AppDbContext())
{
    db.Database.EnsureCreated();

    // 2. Kiểm tra nếu CSDL chưa có dữ liệu thì thêm mới
    if (!db.TaskItems.Any())
    {
        Console.WriteLine("[INFO] Chưa có dữ liệu. Đang khởi tạo dữ liệu ban đầu vào CSDL...");

        db.TaskItems.AddRange(
            new TaskItems
            {
                Title = "Nộp báo cáo đồ án cho giảng viên",
                AssignedQuadrant = Quadrant.Q1_doFirst,
                AiReasoning = "Deadline trong ngày và ảnh hưởng điểm số."
            },
            new TaskItems
            {
                Title = "Học C# và .NET 8.0 mỗi ngày 1 tiếng",
                AssignedQuadrant = Quadrant.Q2_schedule,
                AiReasoning = "Mục tiêu dài hạn, phát triển kỹ năng."
            }
        );

        // Lưu thay đổi vào file SQLite
        db.SaveChanges();
        Console.WriteLine("[SUCCESS] Đã lưu dữ liệu vào CSDL thành công!\n");
    }
    else
    {
        Console.WriteLine("[INFO] Đã tìm thấy dữ liệu có sẵn trong file SQLite!\n");
    }

    // 3. Đọc danh sách Task từ file SQLite ra màn hình
    var allTasks = db.TaskItems.ToList();
    Console.WriteLine($"--- DANH SÁCH TASK TỪ DATABASE ({allTasks.Count} tasks) ---");

    foreach (var task in allTasks)
    {
        Console.WriteLine($"[ID: {task.Id}] {task.Title}");
        Console.WriteLine($"  -> Phân loại : {task.AssignedQuadrant}");
        Console.WriteLine($"  -> Trạng thái: {(task.IsCompleted ? "Hoàn thành" : "Chưa làm")}");
        Console.WriteLine($"  -> Ngày tạo  : {task.CreatedAt:dd/MM/yyyy HH:mm}");
        Console.WriteLine("--------------------------------------------------");
    }
}

Console.WriteLine("\nBấm phím bất kỳ để thoát...");
Console.ReadKey();