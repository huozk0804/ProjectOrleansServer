using Grpc.Net.Client;
using ProjectOrleansServer.Protos;
using ProjectHotel.Protos;

namespace GrpcClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("=== gRPC客户端示例 ===");
            
            // 创建gRPC通道
            using var authChannel = GrpcChannel.ForAddress("https://localhost:5001");
            using var hotelChannel = GrpcChannel.ForAddress("https://localhost:5002");
            
            // 创建客户端
            var authClient = new AuthService.AuthServiceClient(authChannel);
            var permissionClient = new PermissionService.PermissionServiceClient(authChannel);
            var hotelClient = new HotelService.HotelServiceClient(hotelChannel);
            var reservationClient = new ReservationService.ReservationServiceClient(hotelChannel);
            
            try
            {
                // 1. 用户登录
                Console.WriteLine("\n=== 用户登录 ===");
                var loginRequest = new LoginRequest
                {
                    Username = "hotelmanager",
                    Password = "manager123"
                };
                
                var loginResponse = await authClient.LoginAsync(loginRequest);
                if (loginResponse.Success)
                {
                    Console.WriteLine($"登录成功! 用户: {loginResponse.User.Username}");
                    Console.WriteLine($"角色: {string.Join(", ", loginResponse.Roles)}");
                    Console.WriteLine($"权限: {string.Join(", ", loginResponse.Permissions)}");
                }
                else
                {
                    Console.WriteLine($"登录失败: {loginResponse.ErrorMessage}");
                    return;
                }
                
                // 2. 检查权限
                Console.WriteLine("\n=== 权限检查 ===");
                var permissionCheckRequest = new PermissionCheckRequest
                {
                    UserId = loginResponse.User.Id,
                    Resource = "Hotel",
                    Action = "Read"
                };
                
                var permissionCheck = await permissionClient.CheckPermissionAsync(permissionCheckRequest);
                Console.WriteLine($"权限检查结果: {permissionCheck.HasPermission} - {permissionCheck.Reason}");
                
                // 3. 获取所有酒店
                Console.WriteLine("\n=== 获取所有酒店 ===");
                var getAllHotelsRequest = new GetAllHotelsRequest();
                var hotelsResponse = await hotelClient.GetAllHotelsAsync(getAllHotelsRequest);
                
                foreach (var hotel in hotelsResponse.Hotels)
                {
                    Console.WriteLine($"ID: {hotel.Id}, 名称: {hotel.Name}, 星级: {hotel.StarRating}");
                }
                
                // 4. 搜索酒店
                Console.WriteLine("\n=== 搜索酒店 ===");
                var searchRequest = new SearchHotelsRequest
                {
                    SearchTerm = "北京"
                };
                var searchResponse = await hotelClient.SearchHotelsAsync(searchRequest);
                
                foreach (var hotel in searchResponse.Hotels)
                {
                    Console.WriteLine($"找到酒店: {hotel.Name}");
                }
                
                // 5. 获取酒店统计信息
                Console.WriteLine("\n=== 酒店统计信息 ===");
                var statisticsRequest = new GetHotelStatisticsRequest();
                var statisticsResponse = await hotelClient.GetHotelStatisticsAsync(statisticsRequest);
                var stats = statisticsResponse.Statistics;
                
                Console.WriteLine($"总酒店数: {stats.TotalHotels}");
                Console.WriteLine($"总房间数: {stats.TotalRooms}");
                Console.WriteLine($"可用房间数: {stats.AvailableRooms}");
                Console.WriteLine($"总预订数: {stats.TotalReservations}");
                
                // 6. 创建新酒店
                Console.WriteLine("\n=== 创建新酒店 ===");
                var createHotelRequest = new CreateHotelRequest
                {
                    Hotel = new Hotel
                    {
                        Name = "深圳凯悦酒店",
                        Address = "深圳市南山区科技园",
                        Phone = "0755-88888888",
                        StarRating = 4
                    }
                };
                
                var createHotelResponse = await hotelClient.CreateHotelAsync(createHotelRequest);
                if (createHotelResponse.HotelId > 0)
                {
                    Console.WriteLine($"新酒店创建成功，ID: {createHotelResponse.HotelId}");
                    
                    // 7. 为酒店添加房间
                    Console.WriteLine("\n=== 添加房间 ===");
                    var addRoomRequest = new AddRoomRequest
                    {
                        HotelId = createHotelResponse.HotelId,
                        Room = new Room
                        {
                            RoomNumber = "101",
                            RoomType = "标准间",
                            Price = 299.00,
                            IsAvailable = true
                        }
                    };
                    
                    var addRoomResponse = await hotelClient.AddRoomAsync(addRoomRequest);
                    if (addRoomResponse.Success)
                    {
                        Console.WriteLine("房间添加成功");
                    }
                    
                    // 8. 创建预订
                    Console.WriteLine("\n=== 创建预订 ===");
                    var createReservationRequest = new CreateReservationRequest
                    {
                        Reservation = new Reservation
                        {
                            HotelId = createHotelResponse.HotelId,
                            RoomId = 1,
                            GuestName = "张三",
                            GuestPhone = "13800138000",
                            CheckInDate = ((DateTimeOffset)DateTime.Today.AddDays(1)).ToUnixTimeSeconds(),
                            CheckOutDate = ((DateTimeOffset)DateTime.Today.AddDays(3)).ToUnixTimeSeconds(),
                            TotalAmount = 598.00,
                            Status = ReservationStatus.Pending
                        }
                    };
                    
                    var createReservationResponse = await reservationClient.CreateReservationAsync(createReservationRequest);
                    if (createReservationResponse.Success)
                    {
                        Console.WriteLine("预订创建成功");
                        
                        // 9. 确认预订
                        Console.WriteLine("\n=== 确认预订 ===");
                        var confirmReservationRequest = new ConfirmReservationRequest
                        {
                            ReservationId = 1
                        };
                        
                        var confirmReservationResponse = await reservationClient.ConfirmReservationAsync(confirmReservationRequest);
                        if (confirmReservationResponse.Success)
                        {
                            Console.WriteLine("预订已确认");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
            }
            
            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }
    }
}
