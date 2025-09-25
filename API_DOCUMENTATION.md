# API 文档

## 概述

本项目提供了基于gRPC和Protocol Buffers的HTTP API，支持认证权限管理和酒店业务管理功能。

## 服务端点

### 认证服务 (ProjectOrleansServer)
- **地址**: https://localhost:5001
- **监控面板**: http://localhost:10010

### 酒店业务服务 (ProjectHotel)
- **地址**: https://localhost:5002
- **监控面板**: http://localhost:10011

## 认证服务 API

### AuthService

#### 用户登录
```protobuf
rpc Login(LoginRequest) returns (LoginResponse);
```

**请求**:
```protobuf
message LoginRequest {
  string username = 1;
  string password = 2;
  bool remember_me = 3;
}
```

**响应**:
```protobuf
message LoginResponse {
  bool success = 1;
  string token = 2;
  string refresh_token = 3;
  int64 expires_at = 4;
  User user = 5;
  repeated string roles = 6;
  repeated string permissions = 7;
  string error_message = 8;
}
```

#### 用户注册
```protobuf
rpc Register(RegisterRequest) returns (LoginResponse);
```

**请求**:
```protobuf
message RegisterRequest {
  string username = 1;
  string email = 2;
  string password = 3;
  string confirm_password = 4;
  string first_name = 5;
  string last_name = 6;
}
```

#### 刷新令牌
```protobuf
rpc RefreshToken(RefreshTokenRequest) returns (LoginResponse);
```

#### 用户登出
```protobuf
rpc Logout(LogoutRequest) returns (LogoutResponse);
```

#### 验证令牌
```protobuf
rpc ValidateToken(ValidateTokenRequest) returns (User);
```

#### 获取用户信息
```protobuf
rpc GetUser(GetUserRequest) returns (User);
```

#### 更新用户信息
```protobuf
rpc UpdateUser(UpdateUserRequest) returns (UpdateUserResponse);
```

#### 更改密码
```protobuf
rpc ChangePassword(ChangePasswordRequest) returns (ChangePasswordResponse);
```

### PermissionService

#### 检查权限
```protobuf
rpc CheckPermission(PermissionCheckRequest) returns (PermissionCheckResponse);
```

**请求**:
```protobuf
message PermissionCheckRequest {
  int32 user_id = 1;
  string resource = 2;
  string action = 3;
}
```

**响应**:
```protobuf
message PermissionCheckResponse {
  bool has_permission = 1;
  string reason = 2;
}
```

#### 获取用户权限
```protobuf
rpc GetUserPermissions(GetUserPermissionsRequest) returns (GetUserPermissionsResponse);
```

#### 获取用户角色
```protobuf
rpc GetUserRoles(GetUserRolesRequest) returns (GetUserRolesResponse);
```

#### 角色和权限管理
```protobuf
rpc AssignRoleToUser(AssignRoleToUserRequest) returns (AssignRoleToUserResponse);
rpc RemoveRoleFromUser(RemoveRoleFromUserRequest) returns (RemoveRoleFromUserResponse);
rpc AssignPermissionToRole(AssignPermissionToRoleRequest) returns (AssignPermissionToRoleResponse);
rpc RemovePermissionFromRole(RemovePermissionFromRoleRequest) returns (RemovePermissionFromRoleResponse);
rpc CreateRole(CreateRoleRequest) returns (CreateRoleResponse);
rpc CreatePermission(CreatePermissionRequest) returns (CreatePermissionResponse);
rpc GetAllRoles(GetAllRolesRequest) returns (GetAllRolesResponse);
rpc GetAllPermissions(GetAllPermissionsRequest) returns (GetAllPermissionsResponse);
```

## 酒店业务服务 API

### HotelService

#### 获取酒店信息
```protobuf
rpc GetHotel(GetHotelRequest) returns (GetHotelResponse);
```

**请求**:
```protobuf
message GetHotelRequest {
  int32 hotel_id = 1;
}
```

**响应**:
```protobuf
message GetHotelResponse {
  Hotel hotel = 1;
}
```

#### 创建或更新酒店
```protobuf
rpc CreateOrUpdateHotel(CreateOrUpdateHotelRequest) returns (CreateOrUpdateHotelResponse);
```

#### 删除酒店
```protobuf
rpc DeleteHotel(DeleteHotelRequest) returns (DeleteHotelResponse);
```

#### 房间管理
```protobuf
rpc GetRooms(GetRoomsRequest) returns (GetRoomsResponse);
rpc AddRoom(AddRoomRequest) returns (AddRoomResponse);
rpc UpdateRoom(UpdateRoomRequest) returns (UpdateRoomResponse);
rpc DeleteRoom(DeleteRoomRequest) returns (DeleteRoomResponse);
rpc GetAvailableRooms(GetAvailableRoomsRequest) returns (GetAvailableRoomsResponse);
```

#### 酒店查询
```protobuf
rpc GetAllHotels(GetAllHotelsRequest) returns (GetAllHotelsResponse);
rpc SearchHotels(SearchHotelsRequest) returns (SearchHotelsResponse);
rpc GetHotelsByStarRating(GetHotelsByStarRatingRequest) returns (GetHotelsByStarRatingResponse);
rpc GetHotelStatistics(GetHotelStatisticsRequest) returns (GetHotelStatisticsResponse);
rpc CreateHotel(CreateHotelRequest) returns (CreateHotelResponse);
```

### ReservationService

#### 预订管理
```protobuf
rpc GetReservation(GetReservationRequest) returns (GetReservationResponse);
rpc CreateReservation(CreateReservationRequest) returns (CreateReservationResponse);
rpc UpdateReservationStatus(UpdateReservationStatusRequest) returns (UpdateReservationStatusResponse);
rpc CancelReservation(CancelReservationRequest) returns (CancelReservationResponse);
rpc ConfirmReservation(ConfirmReservationRequest) returns (ConfirmReservationResponse);
rpc CheckIn(CheckInRequest) returns (CheckInResponse);
rpc CheckOut(CheckOutRequest) returns (CheckOutResponse);
```

## 数据模型

### 用户模型
```protobuf
message User {
  int32 id = 1;
  string username = 2;
  string email = 3;
  string first_name = 4;
  string last_name = 5;
  bool is_active = 6;
  int64 created_at = 7;
  int64 updated_at = 8;
  int64 last_login_at = 9;
}
```

### 酒店模型
```protobuf
message Hotel {
  int32 id = 1;
  string name = 2;
  string address = 3;
  string phone = 4;
  int32 star_rating = 5;
  int64 created_at = 6;
  int64 updated_at = 7;
}
```

### 房间模型
```protobuf
message Room {
  int32 id = 1;
  int32 hotel_id = 2;
  string room_number = 3;
  string room_type = 4;
  double price = 5;
  bool is_available = 6;
  int64 created_at = 7;
  int64 updated_at = 8;
}
```

### 预订模型
```protobuf
message Reservation {
  int32 id = 1;
  int32 hotel_id = 2;
  int32 room_id = 3;
  string guest_name = 4;
  string guest_phone = 5;
  int64 check_in_date = 6;
  int64 check_out_date = 7;
  double total_amount = 8;
  ReservationStatus status = 9;
  int64 created_at = 10;
  int64 updated_at = 11;
}

enum ReservationStatus {
  PENDING = 0;
  CONFIRMED = 1;
  CHECKED_IN = 2;
  CHECKED_OUT = 3;
  CANCELLED = 4;
}
```

## 使用示例

### C# 客户端示例

```csharp
// 创建gRPC通道
using var authChannel = GrpcChannel.ForAddress("https://localhost:5001");
using var hotelChannel = GrpcChannel.ForAddress("https://localhost:5002");

// 创建客户端
var authClient = new AuthService.AuthServiceClient(authChannel);
var hotelClient = new HotelService.HotelServiceClient(hotelChannel);

// 用户登录
var loginRequest = new LoginRequest
{
    Username = "hotelmanager",
    Password = "manager123"
};
var loginResponse = await authClient.LoginAsync(loginRequest);

// 获取所有酒店
var hotelsResponse = await hotelClient.GetAllHotelsAsync(new GetAllHotelsRequest());
```

### JavaScript 客户端示例

```javascript
// 使用 grpc-web 客户端
const { AuthServiceClient } = require('./generated/auth_service_grpc_web_pb');
const { HotelServiceClient } = require('./generated/hotel_service_grpc_web_pb');

const authClient = new AuthServiceClient('https://localhost:5001');
const hotelClient = new HotelServiceClient('https://localhost:5002');

// 用户登录
const loginRequest = new LoginRequest();
loginRequest.setUsername('hotelmanager');
loginRequest.setPassword('manager123');

authClient.login(loginRequest, {}, (err, response) => {
    if (err) {
        console.error('登录失败:', err);
    } else {
        console.log('登录成功:', response.getUser().getUsername());
    }
});
```

## 错误处理

所有API都返回标准的gRPC状态码：
- `OK (0)`: 成功
- `INVALID_ARGUMENT (3)`: 无效参数
- `UNAUTHENTICATED (16)`: 未认证
- `PERMISSION_DENIED (7)`: 权限不足
- `NOT_FOUND (5)`: 资源未找到
- `INTERNAL (13)`: 内部服务器错误

## 认证和授权

1. 客户端首先调用认证服务进行登录
2. 认证服务返回JWT令牌和用户信息
3. 客户端在后续请求中携带JWT令牌
4. 业务服务验证令牌并检查权限
5. 权限验证通过后执行业务逻辑

## 性能优化

- 使用Protocol Buffers进行高效序列化
- 支持gRPC流式传输
- 内置连接池和负载均衡
- 支持HTTP/2多路复用
- 自动压缩和缓存

## 监控和调试

- Orleans Dashboard提供实时监控
- gRPC反射支持API探索
- 结构化日志记录
- 性能指标收集
- 健康检查端点
