using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ScheduleYourLife;

namespace ScheduleYourLife.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DpSelectedDate.SelectedDate = DateTime.Today;
            InitializeDatabase();
            LoadData();
            DpSelectedDate.SelectedDateChanged += (s, e) => LoadData();
        }

        private void InitializeDatabase()
        {
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
            }
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                var allTasks = db.TaskItems.ToList();

                // 1. Đổ dữ liệu xuống Task List bên dưới
                LstTasks.ItemsSource = allTasks;

                // 2. Chia nhỏ Task vào các phiên (45p Làm / 15p Nghỉ)
                LstMorningShift.ItemsSource = FormatShiftTasks(allTasks.Where(t => t.Id % 4 == 1).ToList(), 8);   // Ca Sáng từ 8h
                LstNoonShift.ItemsSource = FormatShiftTasks(allTasks.Where(t => t.Id % 4 == 2).ToList(), 12);   // Ca Trưa từ 12h
                LstAfternoonShift.ItemsSource = FormatShiftTasks(allTasks.Where(t => t.Id % 4 == 3).ToList(), 13); // Ca Chiều từ 13h
                LstEveningShift.ItemsSource = FormatShiftTasks(allTasks.Where(t => t.Id % 4 == 0).ToList(), 18);   // Ca Tối từ 18h
            }
        }

        // Định dạng chia khung giờ 45p làm / 15p nghỉ
        private List<TaskItems> FormatShiftTasks(List<TaskItems> tasks, int startHour)
        {
            var formattedList = new List<TaskItems>();
            DateTime timeCursor = DateTime.Today.AddHours(startHour);

            foreach (var task in tasks)
            {
                DateTime workEnd = timeCursor.AddMinutes(45);
                DateTime restEnd = workEnd.AddMinutes(15);

                formattedList.Add(new TaskItems
                {
                    Id = task.Id,
                    Title = $"[{timeCursor:HH:mm}-{workEnd:HH:mm}] {task.Title} (☕ Nghỉ đến {restEnd:HH:mm})"
                });

                timeCursor = restEnd;
            }
            return formattedList;
        }

        // Xử lý nút: ➕ Thêm Task Mới
        private void BtnAddTask_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddTaskWindow { Owner = this };
            if (dialog.ShowDialog() == true)
            {
                using (var db = new AppDbContext())
                {
                    var newTask = new TaskItems
                    {
                        Title = dialog.TaskTitle,
                        AssignedQuadrant = dialog.SelectedQuadrant,
                        CreatedAt = DpSelectedDate.SelectedDate ?? DateTime.Now,
                        AiReasoning = "Thêm từ giao diện WPF."
                    };

                    db.TaskItems.Add(newTask);
                    db.SaveChanges();
                }
                LoadData();
            }
        }

        // Xử lý sự kiện khi tích chọn/bỏ tích CheckBox ở danh sách công việc
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.DataContext is TaskItems task)
            {
                using (var db = new AppDbContext())
                {
                    var dbTask = db.TaskItems.Find(task.Id);
                    if (dbTask != null)
                    {
                        dbTask.IsCompleted = cb.IsChecked ?? false;
                        db.SaveChanges(); // Cập nhật trạng thái completed vào SQLite
                    }
                }
            }
        }

        // Xử lý nút: 🗑️ Loại bỏ Task đã xong
        private void BtnClearCompleted_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var completedTasks = db.TaskItems.Where(t => t.IsCompleted).ToList();
                if (!completedTasks.Any())
                {
                    MessageBox.Show("Không có công việc nào đã hoàn thành để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var confirm = MessageBox.Show($"Bạn có chắc muốn xóa vĩnh viễn {completedTasks.Count} công việc đã hoàn thành khỏi Database?",
                                              "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (confirm == MessageBoxResult.Yes)
                {
                    db.TaskItems.RemoveRange(completedTasks); // Xóa khỏi CSDL
                    db.SaveChanges();
                    LoadData();
                }
            }
        }

        // Xử lý nút: ❓ Help
        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ứng dụng tự động lập lịch theo phiên Pomodoro 45p làm việc và 15p nghỉ ngơi.", "Help");
        }

        // Xử lý nút: Gửi câu hỏi cho AI Chatbot
        private void BtnSendAi_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtAiInput.Text)) return;
            LstAiChat.Items.Add($"👤 Bạn: {TxtAiInput.Text}");
            LstAiChat.Items.Add("🤖 AI: Tôi gợi ý bạn nên nghỉ ngơi 15 phút sau mỗi phiên 45 phút để duy trì năng lượng!");
            TxtAiInput.Clear();
        }
    }
}