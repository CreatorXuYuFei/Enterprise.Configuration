# Enterprise.Configuration

## 企业级 .NET 配置组件 使用文档

**仓库地址**：[https://github.com/CreatorXuYuFei/Enterprise.Configuration](https://github.com/CreatorXuYuFei/Enterprise.Configuration)

**开源协议**：MIT

**组件定位**：基于微软官方 `Microsoft.Extensions.Configuration` 体系深度扩展的**企业级配置增强库**，完全兼容原生配置生态，补齐原生框架在集合绑定、多配置源管理、热更新、类型转换、配置校验等企业场景短板，支持 .NET 全主流版本，无重型第三方依赖。



***

## 目录



1. [项目简介](#1-项目简介)

2. [核心特性](#2-核心特性)

3. [运行环境 & 安装方式](#3-运行环境--安装方式)

4. [基础概念：配置源优先级](#4-基础概念配置源优先级)

5. [快速入门示例](#5-快速入门示例)

6. [核心功能使用教程](#6-核心功能使用教程)

7. [进阶用法](#7-进阶用法)

8. [核心 API 说明](#8-核心-api-说明)

9. [最佳实践](#9-最佳实践)

10. [常见问题 & 故障排查](#10-常见问题--故障排查)



***

## 1. 项目简介

微软原生 `Configuration` 功能基础，但在企业项目中存在诸多限制：



* 不友好支持 **数组 / 集合 / 嵌套对象集合** 绑定；

* 多配置源组合、优先级管理需要手动封装；

* 字符串与数字 / 布尔类型转换需自行处理；

* 缺少统一的配置校验、变更防抖监听；

`Enterprise.Configuration` 在**完全兼容原生语法**的前提下，做全维度增强：



* 原有业务代码几乎无需改动即可迁移；

* 原生不支持的**集合、嵌套集合、稀疏数组**完美解析；

* 一站式整合 JSON、环境变量、命令行、内存配置等多配置源；

* 内置类型自动转换、模型校验、热更新、配置变更监听能力。

适用场景：控制台程序、[ASP.NET](https://ASP.NET) Core WebAPI/ MVC、后台服务、桌面应用等所有 .NET 项目。



***

## 2. 核心特性

### 基础能力（兼容原生）



1. 无缝兼容微软 `IConfiguration` 生态，原生配置语法全部可用；

2. 支持 JSON 配置文件，开启 `reloadOnChange` 实现**配置热更新**；

3. 支持环境变量、命令行参数、内存临时配置三大常用配置源；

4. 配置键**大小写不敏感**，降低书写容错成本。

### 增强能力（组件独有）



1. ✅ **完整集合绑定**：支持 `List<T>`、数组、对象集合、嵌套集合、稀疏索引数组（核心增强）；

2. ✅ **自动类型转换**：字符串 ↔ 数字、字符串 ↔ 布尔（忽略大小写，支持 `true/false/TRUE/FALSE`）；

3. ✅ **强类型实体绑定**：统一扩展方法 `BindModel`，告别繁琐手动映射；

4. ✅ **配置模型校验**：结合 `DataAnnotations` 实现配置项合法性校验；

5. ✅ **配置变更监听 + 防抖**：避免配置频繁变动触发重复逻辑；

6. ✅ **多配置源优先级管控**：后添加的配置源自动覆盖前置配置，层级清晰；

7. ✅ **全量扁平配置导出**：一键获取所有合并后的配置键值对；

8. ✅ 轻量无依赖：仅依赖 .NET 官方基础包，体积小、性能高。



***

## 3. 运行环境 & 安装方式

### 3.1 支持环境



* 目标框架：`.NET 6 / .NET 7 / .NET 8`（全 LTS 长期支持版本）

* 运行平台：Windows / Linux /macOS/ Docker / 容器化部署

* 依赖：仅 .NET 官方 `Microsoft.Extensions.Configuration` 系列包，无第三方依赖

### 3.2 安装方式

#### 方式一：NuGet 安装（推荐正式项目）

##### .NET CLI



```
dotnet add package Enterprise.Configuration
```

##### 包管理器控制台（Visual Studio）



```
Install-Package Enterprise.Configuration
```

#### 方式二：源码引用（二次开发 / 自定义改造）



1. Clone 代码仓库：



```
git clone https://github.com/CreatorXuYuFei/Enterprise.Configuration.git
```



1. 在你的项目中添加**项目引用**：



```
<!-- 你的项目 .csproj -->

<ItemGroup>

 <ProjectReference Include="..Enterprise.ConfigurationEnterprise.Configuration.csproj" />

</ItemGroup>
```

> 重要：若使用 JSON 配置文件，右键配置文件 → 属性 → 
>
> **复制到输出目录**
>
>  = 
>
> `如果较新则复制`
>
> ，否则程序读取不到文件。



***

## 4. 基础概念：配置源优先级

组件采用 **「后加入优先覆盖」** 规则，优先级从 **低 → 高** 排序：



```
JSON 配置文件  <  环境变量  <  命令行参数  <  内存配置
```

> 示例：同一配置项，内存配置会覆盖命令行，命令行覆盖环境变量，最终以最高优先级配置为准。



***

## 5. 快速入门示例

### 5.1 命名空间引用

所有使用场景需引入以下命名空间：



```
using Enterprise.Configuration;

using Enterprise.Configuration.Abstractions;

using Enterprise.Configuration.Exceptions; // BindModel 扩展方法
```

### 5.2 控制台快速示例（极简入门）

#### 步骤 1：编写配置文件 `appsettings.json`



```
{

 "App": {

   "RunMode": "Development",

   "Debug": true

 },

 "Db": {

   "MaxPool": 50

 },

 "AiAgent": [

   {

     "ApiKey": "sk-123456",

     "Endpoint": "https://api.test.com",

     "Timeout": 60

   }

 ]

}
```

#### 步骤 2：完整代码



```
// 1. 模拟命令行参数

string[] runArgs = ["--App:RunMode=Test", "--Db:MaxPool=100"];

// 2. 内存配置（最高优先级）

var memoryData = new Dictionary<string, string?>

{

   { "App:RunMode", "Production" },

   { "App:Debug", "false" }

};

// 3. 链式构建多配置源

IConfigRoot configRoot = new ConfigBuilder()

   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)

   .AddCommandLine(runArgs)

   .AddMemory(memoryData)

   .Build();

// 4. 基础键值读取

Console.WriteLine($"运行模式：{configRoot["App:RunMode"]}");

Console.WriteLine($"数据库连接池：{configRoot["Db:MaxPool"]}");

// 5. 集合实体绑定（核心能力）

var agentList = configRoot.GetSection("AiAgent").BindModel<List<AiAgentOptions>>();

Console.WriteLine($"AI 接口地址：{agentList[0].Endpoint}");

// 实体定义

public class AiAgentOptions

{

   public string ApiKey { get; set; } = string.Empty;

   public string Endpoint { get; set; } = string.Empty;

   public int Timeout { get; set; }

}
```

### 5.3 [ASP.NET](https://ASP.NET) Core WebAPI 示例（核心业务场景）

遵循你的需求：**启动阶段仅初始化配置，不提前注册 Options，运行时被动触发解析**（规避原生 Options 集合绑定报错、多 DI 容器问题）。

#### 步骤 1：Program.cs（启动层，极简初始化）



```
using Enterprise.Configuration;

using Enterprise.Configuration.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// 1. 构建多配置源

string[] runArgs = ["--App:RunMode=WebTest"];

var memoryData = new Dictionary<string, string?>

{

   { "App:RunMode", "Production" }

};

IConfigRoot configRoot = new ConfigBuilder()

   .AddJsonFile("appsettings.json", false, true)

   .AddCommandLine(runArgs)

   .AddMemory(memoryData)

   .Build();

// 2. 仅注册全局配置根（启动阶段不做任何实体绑定）

builder.Services.AddSingleton(configRoot);

// 3. WebAPI 基础服务

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())

{

   app.UseSwagger();

   app.UseSwaggerUI();

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// 配置实体

public class AiAgentOptions

{

   public string ApiKey { get; set; } = string.Empty;

   public string Endpoint { get; set; } = string.Empty;

   public int Timeout { get; set; }

}
```

#### 步骤 2：控制器（被动触发解析，访问接口时才读取配置）



```
using Enterprise.Configuration.Abstractions;

using Enterprise.Configuration.Exceptions;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;

[ApiController]

[Route("api/config")]

public class ConfigController : ControllerBase

{

   private readonly IConfigRoot _configRoot;

   // 仅注入配置根，无提前绑定

   public ConfigController(IConfigRoot configRoot)

   {

       _configRoot = configRoot;

   }

   /// <summary>被动读取集合配置</summary>

   [HttpGet("ai-agents")]

   public IActionResult GetAiAgents()

   {

       // 访问接口时才解析集合（被动触发）

       var list = _configRoot.GetSection("AiAgent")

           .BindModel<List<AiAgentOptions>>();

       return Ok(list);

   }

   /// <summary>被动读取普通单实体</summary>

   [HttpGet("app")]

   public IActionResult GetAppConfig()

   {

       var appConfig = _configRoot.GetSection("App").BindModel<AppConfig>();

       return Ok(appConfig);

   }

}

// 普通配置实体

public class AppConfig

{

   public string RunMode { get; set; } = string.Empty;

   public bool Debug { get; set; }

}
```



***

## 6. 核心功能使用教程

### 6.1 基础键值读取

通过 `IConfigRoot` 直接按 `节点:子节点` 格式读取原始字符串：



```
// 读取单个键

string? runMode = configRoot["App:RunMode"];

// 安全读取（防止 null）

if (configRoot.TryGetValue("Db:MaxPool", out string? poolStr))

{

   int maxPool = int.Parse(poolStr);

}
```

### 6.2 普通实体绑定（非集合）

使用扩展方法 `BindModel<T>` 绑定单个实体，兼容 `DataAnnotations` 校验：



```
// 定义实体

public class DbConfig

{

   [Range(1, 200, ErrorMessage = "连接池大小必须 1~200")]

   public int MaxPool { get; set; }

}

// 绑定实体

var dbSection = configRoot.GetSection("Db");

DbConfig dbConfig = dbSection.BindModel<DbConfig>();
```

### 6.3 集合绑定（组件核心增强）

支持 **值类型集合、对象集合、嵌套集合、稀疏数组**，原生 `Options` 无法实现。

#### 6.3.1 简单值集合

配置文件：



```
"WhiteIps": [ "127.0.0.1", "192.168.1.1" ]
```

读取代码：



```
List<string> ipList = configRoot.GetSection("WhiteIps").BindModel<List<string>>();
```

#### 6.3.2 对象集合（最常用）

配置参考上文 `AiAgent` 节点，读取：



```
List<AiAgentOptions> agentList = configRoot.GetSection("AiAgent").BindModel<List<AiAgentOptions>>();
```

#### 6.3.3 嵌套集合

配置：



```
"GroupAgent": [

 {

   "GroupName": "Group1",

   "Agents": [ { "ApiKey": "xxx" } ]

 }

]
```

实体 + 读取：



```
public class GroupAgentItem

{

   public string GroupName { get; set; } = string.Empty;

   public List<AiAgentOptions> Agents { get; set; } = new();

}

// 嵌套集合绑定

var groupList = configRoot.GetSection("GroupAgent").BindModel<List<GroupAgentItem>>();
```

### 6.4 多配置源叠加（优先级演示）



```
// 1. JSON 默认值：App:Debug = true

// 2. 命令行：--App:Debug=true

// 3. 内存配置（最高优先级）：App:Debug = false

IConfigRoot configRoot = new ConfigBuilder()

   .AddJsonFile("appsettings.json", false, true)

   .AddCommandLine(new[] { "--App:Debug=true" })

   .AddMemory(new Dictionary<string, string?> { { "App:Debug", "false" } })

   .Build();

// 最终结果：false（内存配置生效）

Console.WriteLine(configRoot["App:Debug"]);
```

### 6.5 配置热更新

只要开启 `reloadOnChange: true`，修改 JSON 文件后**无需重启程序**，再次读取自动加载最新配置：



```
.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
```

### 6.6 配置变更监听

监听配置文件变更，配合防抖避免频繁触发逻辑：



```
// 全局配置重载事件

configRoot.Reloaded += () =>

{

   Console.WriteLine("配置已更新，执行重载逻辑");

   // 重新初始化客户端、刷新缓存等业务逻辑

};
```

### 6.7 模型数据校验

结合 .NET 原生 `DataAnnotations` 做配置合法性校验：



```
using System.ComponentModel.DataAnnotations;

public class AiAgentOptions

{

   [Required(ErrorMessage = "ApiKey 不能为空")]

   public string ApiKey { get; set; } = string.Empty;

   [Url(ErrorMessage = "接口地址格式错误")]

   public string Endpoint { get; set; } = string.Empty;

}

// 校验实体

var agent = configRoot.GetSection("AiAgent").BindModel<List<AiAgentOptions>>()[0];

var validationContext = new ValidationContext(agent);

Validator.ValidateObject(agent, validationContext, true);
```



***

## 7. 进阶用法

### 7.1 静态工具类封装（零 DI 注入，全局被动读取）

适合不想到处注入 `IConfigRoot` 的场景，任意位置直接调用：



```
public static class ConfigHelper

{

   private static IConfigRoot? _configRoot;

   // 程序启动时初始化一次

   public static void Init(IConfigRoot configRoot)

   {

       _configRoot = configRoot ?? throw new ArgumentNullException(nameof(configRoot));

   }

   // 读取单实体

   public static T GetConfig<T>(string section) where T : class, new()

   {

       if (_configRoot == null) throw new InvalidOperationException("配置未初始化");

       return _configRoot.GetSection(section).BindModel<T>();

   }

   // 读取集合

   public static List<T> GetList<T>(string section) where T : class, new()

   {

       return _configRoot!.GetSection(section).BindModel<List<T>>();

   }

   // 手动强制重载所有配置

   public static void Reload() => _configRoot?.ReloadAll();

}
```

**初始化（Program.cs）**



```
ConfigHelper.Init(configRoot);
```

**业务代码调用（零注入）**



```
var agents = ConfigHelper.GetList<AiAgentOptions>("AiAgent");
```

### 7.2 手动重载全量配置

代码中主动触发所有配置源重新加载：



```
configRoot.ReloadAll();
```

### 7.3 导出全量扁平配置

获取所有合并后的配置键值对（用于日志、调试、同步）：



```
Dictionary<string, string?> allConfig = configRoot.GetAllFlattenData();

foreach (var kv in allConfig)

{

   Console.WriteLine($"{kv.Key} = {kv.Value}");

}
```



***

## 8. 核心 API 说明

### 8.1 核心接口 & 类



| 类 / 接口                | 作用                                    |
| --------------------- | ------------------------------------- |
| `ConfigBuilder`       | 配置构建器，链式添加各类配置源                       |
| `IConfigRoot`         | 配置根节点，提供键值读取、分区、重载、全量导出               |
| `IConfigSection`      | 配置分区节点，对应 JSON 中的一个节点                 |
| `ModelBindExtensions` | 静态扩展类，核心方法 `BindModel<T>` 实现实体 / 集合绑定 |
| `JsonHelper`          | JSON 扁平化、反向重建、类型转换工具（内部核心）            |

### 8.2 常用核心方法

#### ConfigBuilder 链式方法



```
// 添加 JSON 文件

AddJsonFile(string path, bool optional, bool reloadOnChange)

// 添加环境变量（支持前缀过滤）

AddEnvironmentVariables(string prefix = null)

// 添加命令行参数

AddCommandLine(string[] args)

// 添加内存临时配置（最高优先级）

AddMemory(Dictionary<string, string?> data)

// 构建最终配置根

IConfigRoot Build()
```

#### IConfigRoot 方法



```
// 按键读取配置

string? this[string key]

// 安全读取

bool TryGetValue(string key, out string? value)

// 获取配置分区

IConfigSection GetSection(string path)

// 手动重载所有配置

void ReloadAll()

// 导出全量扁平配置

Dictionary<string, string?> GetAllFlattenData()

// 配置重载事件

event Action? Reloaded
```

#### 扩展方法（核心）



```
// 分区绑定为实体/集合（万能绑定方法）

public static TOptions BindModel<TOptions>(this IConfigSection section)

   where TOptions : class, new()
```



***

## 9. 最佳实践



1. **配置源顺序规范**

   固定顺序：`JSON文件 → 环境变量 → 命令行 → 内存配置`，符合企业运维习惯。

2. **WebAPI 推荐用法**

   启动仅注册 `IConfigRoot` 为单例，**禁止手动新建独立**`ServiceCollection`，避免多 DI 容器隔离报错；所有实体绑定延迟到控制器 / 服务内部**被动触发**。

3. **文件属性配置**

   JSON 配置文件务必设置「复制到输出目录」，否则部署后读取失败。

4. **热更新使用场景**

   生产环境开启 `reloadOnChange=true`，无需重启服务即可更新配置。

5. **版本管理**

   迭代包版本遵循**语义化版本**：`主版本.次版本.修订号`，重大变更升级主版本。

6. **集合绑定规范**

   优先使用 `BindModel<List<T>>`，不要尝试用微软原生 `IOptions<List<T>>`（原生框架不支持）。



***

## 10. 常见问题 & 故障排查

### 问题 1：WebAPI 报错 `No service for type IOptions<List<T>>`

**原因**：



1. 手动 `new ServiceCollection()` 创建独立 DI 容器，和 WebAPI 宿主容器隔离；

2. 原生 `Options` 框架**不支持直接绑定 List**。

**解决方案**：



* 移除手动创建的 `ServiceCollection`，统一使用 `builder.Services`；

* 放弃 `services.Configure<List<T>>`，改用组件自带的 `BindModel` 被动解析集合。

### 问题 2：读取不到 JSON 配置

**原因**：



1. 配置文件未设置「复制到输出目录」；

2. 文件路径错误、文件名大小写不匹配；

3. `optional=false` 但文件不存在。

**解决方案**：检查文件属性、文件路径。

### 问题 3：集合绑定返回空数组

**原因**：配置节点路径书写错误（如 `AiAgent` 写成 `Ai`）。

**解决方案**：核对 `GetSection(节点名)` 与 JSON 节点完全一致（组件大小写不敏感，但路径层级必须正确）。

### 问题 4：字符串布尔值解析失败

**解决方案**：组件内置 `JsonStringBoolConverter`，支持 `true/false/TRUE/FALSE`，直接使用字符串格式即可。

### 问题 5：修改 JSON 后配置不更新

**原因**：`AddJsonFile` 未开启 `reloadOnChange: true`。

**解决方案**：开启热更新参数。

### 问题 6：稀疏数组解析异常

**说明**：组件已原生支持稀疏索引（跳过部分数组下标），无需额外处理。



***

## 11. 开源信息



* 仓库地址：[https://github.com/CreatorXuYuFei/Enterprise.Configuration](https://github.com/CreatorXuYuFei/Enterprise.Configuration)

* 开源协议：MIT（可免费用于个人、商业项目）

* 技术支持：提交 Issue / Pull Request 进行反馈与迭代
