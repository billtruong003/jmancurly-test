# Tóm tắt thiết lập hệ thống đa người chơi với Photon Fusion 2

## 1. Cấu trúc dự án

Dự án được thiết kế theo mô hình **Multi-Scene** với 2 scene chính:

### MainMenu Scene (Index 0)
- Chứa giao diện người dùng cho việc tạo và tham gia phòng
- Bao gồm các panel: MainMenuPanel, HostPanel, JoinPanel
- Không chứa player hoặc gameplay elements

### Gameplay Scene (Index 1)
- Chứa môi trường chơi game và UI trong game
- Bao gồm GamePanel với thông tin mạng
- Chứa các điểm spawn và gameplay elements

## 2. Các thành phần chính

### NetworkRunnerManager
- **Singleton** quản lý kết nối Fusion và điều phối game
- Duy trì qua các scene transition bằng `DontDestroyOnLoad`
- Quản lý việc host/join game và tạo/phá hủy player
- Thu thập và cung cấp thông tin mạng (ping, bandwidth, etc.)

### GameplayManager
- **Singleton** quản lý các điểm spawn và gameplay logic
- Chỉ tồn tại trong Gameplay Scene
- Có thể tự động tạo điểm spawn với công cụ editor
- Làm cầu nối giữa NetworkRunnerManager và UIManager

### UIManager
- Quản lý giao diện người dùng và chuyển đổi giữa các panel
- Hiển thị thông tin mạng và trạng thái game
- Xử lý input từ người dùng và truyền cho NetworkRunnerManager
- Tự động đăng ký với GameplayManager khi được load

## 3. Luồng chơi game

1. **Khởi động game**:
   - MainMenu Scene được load
   - NetworkRunnerManager được tạo và duy trì suốt quá trình
   - UIManager hiển thị MainMenuPanel

2. **Tạo hoặc tham gia game**:
   - Người chơi nhập tên phòng và chọn Host hoặc Join
   - NetworkRunnerManager khởi tạo kết nối Fusion
   - Hệ thống chuyển sang Gameplay Scene

3. **Trong game**:
   - NetworkRunnerManager tạo player ở các điểm spawn
   - GameplayManager quản lý gameplay logic
   - UIManager hiển thị thông tin mạng và trạng thái game

4. **Kết thúc game**:
   - Người chơi nhấn Disconnect
   - NetworkRunnerManager đóng kết nối Fusion
   - Hệ thống quay lại MainMenu Scene

## 4. Cách thiết lập dự án mới

1. **Tạo Scene**:
   - Tạo MainMenu Scene và Gameplay Scene
   - Thêm cả hai vào Build Settings (MainMenu index 0, Gameplay index 1)

2. **Thiết lập NetworkRunnerManager**:
   - Tạo Empty GameObject và thêm NetworkRunnerManager script
   - Thiết lập tham chiếu đến Player Prefab
   - Thiết lập Scene Indexes đúng

3. **Thiết lập UI**:
   - Tạo Canvas và các UI Panels theo hướng dẫn UI_SETUP_GUIDE.md
   - Thêm UIManager script và liên kết các tham chiếu UI

4. **Thiết lập GameplayManager**:
   - Trong Gameplay Scene, tạo GameplayManager
   - Sử dụng các công cụ tự động tạo điểm spawn
   - Liên kết với UIManager

5. **Kiểm tra**:
   - Chạy game từ MainMenu Scene
   - Kiểm tra luồng Host/Join và scene transitions
   - Xác nhận thông tin mạng hiển thị chính xác

## 5. Mẹo sử dụng

### Sử dụng thông tin mạng
- **Advanced Stats Toggle** cho phép xem thông tin mạng chi tiết
- Monitoring ping và bandwidth có thể giúp phát hiện vấn đề kết nối

### Spawn Points
- Sử dụng công cụ **Generate Random Spawn Points** để tạo điểm spawn nhanh chóng
- Điều chỉnh tham số sinh điểm để phù hợp với map của bạn

### Scene Management
- Đảm bảo thiết lập đúng scene indexes trong NetworkRunnerManager
- Nếu thêm scene mới, cập nhật lại indexes và build settings

### Coroutine Initialization
- Hệ thống sử dụng coroutines để đảm bảo các Manager được kết nối đúng thứ tự
- Timeouts được thiết lập để tránh hang khi có lỗi khởi tạo

## 6. Mở rộng hệ thống

### Thêm game modes mới
- Tạo enum GameMode và thêm logic xử lý trong NetworkRunnerManager
- Cập nhật UI để cho phép người chơi chọn game mode

### Thêm maps mới
- Tạo scene mới cho từng map
- Cập nhật NetworkRunnerManager để hỗ trợ nhiều maps
- Thêm UI cho phép người chơi chọn map

### Thêm thông tin người chơi
- Mở rộng UIManager để hiển thị danh sách người chơi
- Thêm player stats và leaderboard