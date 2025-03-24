# Hướng dẫn thiết lập Game Panel với thông tin mạng

## 1. Tổng quan

Game Panel là UI được hiển thị trong quá trình chơi game, chứa các thông tin về phòng, kết nối mạng, và các điều khiển trong game. Hướng dẫn này sẽ chỉ cho bạn cách thiết lập Game Panel với đầy đủ thông tin mạng như tên phòng, ping, lưu lượng mạng, và số người chơi.

## 2. Thiết lập Canvas và Game Panel

1. **Tạo Canvas trong GameplayScene**:
   - Trong Hierarchy: Create > UI > Canvas
   - Đặt tên "GameplayCanvas"
   - Đặt Canvas Scaler: UI Scale Mode = "Scale With Screen Size", Reference Resolution = 1920x1080

2. **Tạo Game Panel chính**:
   - Trong Canvas: Create > UI > Panel
   - Đặt tên "GamePanel"
   - Thiết lập Anchor: Top-Stretch
   - Thiết lập kích thước và vị trí để tạo thanh thông tin ở phía trên màn hình

## 3. Thêm thông tin phòng và mạng

1. **Tạo Room Info Panel**:
   - Trong GamePanel: Create > UI > Panel
   - Đặt tên "RoomInfoPanel"
   - Đặt ở góc trên bên trái
   - Kích thước khoảng 350x200
   - Background Color: Màu tối, Alpha = 0.7

2. **Thêm Room Name Text**:
   - Trong RoomInfoPanel: Create > UI > Text - TextMeshPro
   - Đặt tên "_roomNameText"
   - Font Size: 24
   - Text: "Room: ..."
   - Alignment: Middle Left
   - Margin Left: 10px

3. **Thêm Player Count Text**:
   - Trong RoomInfoPanel: Create > UI > Text - TextMeshPro
   - Đặt tên "_playerCountText"
   - Font Size: 20
   - Text: "Players: 1/4"
   - Alignment: Middle Left
   - Margin Left: 10px
   - Position Y: Dưới Room Name

4. **Thêm Ping Text**:
   - Trong RoomInfoPanel: Create > UI > Text - TextMeshPro
   - Đặt tên "_pingText"
   - Font Size: 20
   - Text: "Ping: 0 ms"
   - Alignment: Middle Left
   - Margin Left: 10px
   - Position Y: Dưới Player Count

5. **Thêm FPS Text**:
   - Trong RoomInfoPanel: Create > UI > Text - TextMeshPro
   - Đặt tên "_fpsText"
   - Font Size: 20
   - Text: "FPS: 60"
   - Alignment: Middle Left
   - Margin Left: 10px
   - Position Y: Dưới Ping

## 4. Thêm thông tin mạng nâng cao

1. **Tạo Advanced Stats Toggle**:
   - Trong RoomInfoPanel: Create > UI > Toggle
   - Đặt tên "_showAdvancedStatsToggle"
   - Label Text: "Show Network Stats"
   - Position: Dưới FPS Text

2. **Tạo Advanced Stats Panel**:
   - Trong RoomInfoPanel: Create > UI > Panel
   - Đặt tên "_advancedStatsPanel"
   - Kích thước khoảng 350x250
   - Position: Dưới Toggle
   - Background Color: Màu tối, Alpha = 0.5
   - Mặc định: SetActive(false)

3. **Thêm Network Stats Text**:
   - Trong AdvancedStatsPanel: Create > UI > Text - TextMeshPro
   - Đặt tên "_networkStatsText"
   - Font Size: 16
   - Font Style: Monospace
   - Text: "Network Stats..."
   - Alignment: Middle Left
   - Margin Left: 10px

## 5. Thêm nút Disconnect

1. **Tạo Disconnect Button**:
   - Trong GamePanel: Create > UI > Button
   - Đặt tên "_disconnectButton"
   - Đặt ở góc trên bên phải của GamePanel
   - Text: "Disconnect"
   - Kích thước: 160x50
   - Colors: Normal Color = Red với Alpha = 0.8

## 6. Thêm Status Text

1. **Tạo Status Text**:
   - Trong Canvas (không phải trong GamePanel): Create > UI > Text - TextMeshPro
   - Đặt tên "_statusText"
   - Anchor: Bottom-Stretch
   - Kích thước: Height = 40
   - Position Y = 20
   - Font Size: 24
   - Text: "" (rỗng)
   - Alignment: Middle Center
   - Font Style: Bold
   - Color: Yellow hoặc White

## 7. Kết nối với UIManager

1. **Thêm UIManager vào Scene**:
   - Create > Empty GameObject
   - Đặt tên "UIManager"
   - Thêm component UIManager script

2. **Kéo thả các tham chiếu**:
   - Kéo GamePanel vào trường _gamePanel
   - Kéo các text và button vào các trường tương ứng:
     * _roomNameText
     * _playerCountText
     * _pingText
     * _fpsText
     * _networkStatsText
     * _showAdvancedStatsToggle
     * _advancedStatsPanel
     * _disconnectButton
     * _statusText

3. **Kiểm tra _advanced_statsPanel**:
   - Đảm bảo mặc định nó đã bị ẩn (SetActive = false)
   - Toggle sẽ điều khiển hiển thị của panel này

## 8. Kết nối UIManager với GameplayManager

1. **Đảm bảo GameplayManager có tham chiếu**:
   - Kéo UIManager vào trường _uiManager của GameplayManager
   - Hoặc UIManager sẽ tự đăng ký thông qua code

## Kết quả cuối cùng

Khi thiết lập hoàn tất, GamePanel trong quá trình chơi sẽ hiển thị:
- Tên phòng đang chơi
- Số lượng người chơi hiện tại và tối đa
- Ping (độ trễ) hiện tại
- FPS (khung hình mỗi giây)
- Tùy chọn xem thông tin mạng chi tiết
- Nút Disconnect để rời khỏi trò chơi

UIManager sẽ tự động cập nhật các thông tin này mỗi 0.5 giây, sử dụng dữ liệu từ NetworkRunnerManager.

## Lưu ý quan trọng

- **Đặt tên chính xác**: Đảm bảo các đối tượng UI có tên chính xác như trong UIManager script
- **Phần Canvas**: Trong Gameplay Scene, chỉ cần GamePanel và các thành phần liên quan
- **Tương thích**: Thiết kế UI để hoạt động tốt ở nhiều độ phân giải khác nhau 