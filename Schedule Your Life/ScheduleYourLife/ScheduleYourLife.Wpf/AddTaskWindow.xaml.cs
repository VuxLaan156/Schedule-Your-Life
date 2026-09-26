using System;
using System.Windows;
using ScheduleYourLife; // Namespace chứa enum Quadrant

namespace ScheduleYourLife.Wpf
{
    public partial class AddTaskWindow : Window
    {
        public string TaskTitle { get; private set; } = string.Empty;
        public Quadrant SelectedQuadrant { get; private set; } = Quadrant.Q2_schedule;

        public AddTaskWindow()
        {
            InitializeComponent();

            // Lấy danh sách enum Quadrant đổ vào ComboBox
            CboQuadrant.ItemsSource = Enum.GetValues(typeof(Quadrant));
            CboQuadrant.SelectedIndex = 1; // Mặc định chọn Quadrant 2 (Lên kế hoạch)
        }

        // Sự kiện khi bấm nút "Lưu Task"
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra tính bắt buộc của Tên công việc
            if (string.IsNullOrWhiteSpace(TxtTitle.Text))
            {
                MessageBox.Show("Vui lòng nhập tên công việc!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtTitle.Focus();
                return;
            }

            TaskTitle = TxtTitle.Text.Trim();

            // Lấy Ma trận Eisenhower được chọn từ ComboBox
            if (CboQuadrant.SelectedItem is Quadrant selected)
            {
                SelectedQuadrant = selected;
            }

            // Trả về true để thông báo cho MainWindow biết người dùng đã bấm "Lưu"
            DialogResult = true;
        }
    }
}