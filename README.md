# ProjectOrleansServer - 分布式微服务架构

这是一个基于Microsoft Orleans构建的分布式微服务系统，包含认证权限管理和酒店业务管理两个独立的服务，展示了如何使用Orleans框架构建可扩展的微服务架构。

## 项目架构

### 项目结构
```
ProjectOrleansServer/
├── ProjectOrleansServer/          # 认证和权限管理服务
│   ├── Models/                    # 认证和权限数据模型
│   ├── Interfaces/                # 认证Grain接口定义
│   └── Grains/                    # 认证Grain实现
├── ProjectHotel/                  # 酒店业务管理服务
│   ├── Models/                    # 酒店业务数据模型
│   ├── Interfaces/                # 酒店Grain接口定义
│   └── Grains/                    # 酒店Grain实现
├── DB/                           # 数据库管理工具
└── ProjectOrleansServer.sln      # 解决方案文件
```

### 核心组件

#### 1. 认证和权限管理服务 (ProjectOrleansServer)
- **User**: 用户信息模型
- **Role**: 角色信息模型
- **Permission**: 权限信息模型
- **IAuthGrain**: 用户认证Grain接口
- **IPermissionGrain**: 权限管理Grain接口
- **AuthGrain**: 实现用户登录、注册、令牌管理
- **PermissionGrain**: 实现权限检查、角色管理

#### 2. 酒店业务管理服务 (ProjectHotel)
- **Hotel**: 酒店信息模型
- **Room**: 房间信息模型  
- **Reservation**: 预订信息模型
- **IHotelGrain**: 酒店Grain接口，管理单个酒店的信息和房间
- **IReservationGrain**: 预订Grain接口，管理预订的生命周期
- **IHotelManagementGrain**: 酒店管理Grain接口，管理所有酒店

#### 3. 数据库层 (DB)
- **HotelDbContext**: Entity Framework数据库上下文
- 支持SQL Server数据库
- 包含完整的实体配置和索引

## 功能特性

### 认证和权限管理
- 用户注册和登录
- JWT令牌管理
- 角色和权限管理
- 权限检查和控制
- 用户信息管理

### 酒店业务管理
- 创建、更新、删除酒店信息
- 按名称搜索酒店
- 按星级筛选酒店
- 获取酒店统计信息
- 房间管理（添加、更新、删除、查询可用房间）
- 预订管理（创建、确认、入住/退房、取消）

### 微服务架构
- 独立的认证服务
- 独立的业务服务
- 服务间通信
- 权限验证集成

## 技术栈

- **.NET 9.0**: 最新版本的.NET框架
- **Microsoft Orleans 9.2.1**: 分布式Actor模型框架
- **Entity Framework Core 9.0**: ORM框架
- **SQL Server**: 数据库
- **OrleansDashboard**: 监控和调试工具
- **gRPC**: 高性能RPC框架
- **Protocol Buffers**: 高效的序列化格式
- **ASP.NET Core**: Web API框架

## 快速开始

### 1. 环境要求
- .NET 9.0 SDK
- SQL Server (LocalDB或完整版)
- Visual Studio 2022 或 VS Code

### 2. 数据库设置
```bash
# 进入DB项目目录
cd DB

# 运行数据库初始化
dotnet run
```

### 3. 启动认证服务
```bash
# 进入认证服务项目目录
cd ProjectOrleansServer

# 启动认证服务Silo
dotnet run
```

### 4. 启动酒店业务服务
```bash
# 进入酒店业务服务项目目录
cd ProjectHotel

# 启动酒店业务服务Silo
dotnet run
```

### 5. 启动gRPC客户端示例
```bash
# 进入gRPC客户端项目目录
cd GrpcClient

# 启动gRPC客户端
dotnet run
```

### 6. 访问监控面板
- 认证服务监控: http://localhost:10010
- 酒店业务服务监控: http://localhost:10011

### 7. 访问HTTP API
- 认证服务API: https://localhost:5001
- 酒店业务服务API: https://localhost:5002

## 使用示例

### 认证和权限管理示例
```csharp
// 连接到认证服务
var authClient = new ClientBuilder()
    .UseLocalhostClustering()
    .Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "AuthCluster";
        options.ServiceId = "AuthService";
    })
    .Build();
await authClient.Connect();

// 用户登录
var authGrain = authClient.GetGrain<IAuthGrain>("auth");
var loginResponse = await authGrain.LoginAsync(new LoginRequest
{
    Username = "hotelmanager",
    Password = "manager123"
});

// 检查权限
var permissionGrain = authClient.GetGrain<IPermissionGrain>("permission");
var hasPermission = await permissionGrain.CheckPermissionAsync(new PermissionCheckRequest
{
    UserId = loginResponse.User.Id,
    Resource = "Hotel",
    Action = "Read"
});
```

### 酒店业务管理示例
```csharp
// 连接到酒店业务服务
var hotelClient = new ClientBuilder()
    .UseLocalhostClustering()
    .Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "HotelCluster";
        options.ServiceId = "HotelService";
    })
    .Build();
await hotelClient.Connect();

// 获取酒店管理Grain
var hotelManagement = hotelClient.GetGrain<IHotelManagementGrain>("management");

// 获取所有酒店
var hotels = await hotelManagement.GetAllHotelsAsync();

// 创建新酒店
var newHotel = new Hotel
{
    Name = "新酒店",
    Address = "新地址",
    StarRating = 4
};
var hotelId = await hotelManagement.CreateHotelAsync(newHotel);

// 操作特定酒店
var hotelGrain = hotelClient.GetGrain<IHotelGrain>(hotelId);
await hotelGrain.AddRoomAsync(new Room
{
    RoomNumber = "101",
    RoomType = "标准间",
    Price = 299.00m
});
```

### gRPC API示例
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
foreach (var hotel in hotelsResponse.Hotels)
{
    Console.WriteLine($"酒店: {hotel.Name}");
}
```

## 架构优势

### 1. 可扩展性
- 基于Actor模型的分布式架构
- 支持水平扩展
- 自动负载均衡

### 2. 高可用性
- 内置故障恢复机制
- 状态持久化
- 集群容错

### 3. 开发效率
- 简单的编程模型
- 强类型接口
- 内置监控和调试工具

### 4. 性能
- 内存中的Actor状态
- 异步消息传递
- 批量操作支持

## 监控和调试

### OrleansDashboard
- 实时监控Grain状态
- 性能指标
- 错误日志
- 集群拓扑

### 日志记录
- 结构化日志
- 不同级别的日志记录
- 性能跟踪

## 扩展建议

### 1. 持久化存储
- 集成Azure Storage
- 支持Redis缓存
- 实现自定义存储提供程序

### 2. 消息传递
- 集成Service Bus
- 实现事件驱动架构
- 支持消息队列

### 3. 安全
- 身份验证和授权
- API密钥管理
- 数据加密

### 4. 微服务集成
- 与其他微服务通信
- API网关集成
- 服务发现

## 贡献指南

1. Fork项目
2. 创建功能分支
3. 提交更改
4. 创建Pull Request

## 许可证

MIT License