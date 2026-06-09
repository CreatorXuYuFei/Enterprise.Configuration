# 项目介绍
Enterprise.Configuration 是一款面向 .NET 生态 打造的企业级通用配置增强库，基于微软官方 Microsoft.Extensions.Configuration 体系深度扩展，专为中大型企业项目设计，解决原生配置框架在多环境管理、配置热更新、配置加密、强类型绑定、分布式配置、配置校验等场景下的短板。
本库完全兼容 .NET 原生配置 API，无侵入式改造，可无缝接入 控制台应用、ASP.NET Core、Worker Service、Windows 服务 等所有 .NET 项目。

# 核心特性
✅ 全兼容原生配置：完全基于微软官方 Configuration 构建，原有代码无需大幅改造即可迁移
✅ 多环境配置隔离：内置 Development/Staging/Production 多环境自动切换规则，支持自定义环境
✅ 配置热更新：监听配置文件变更，自动重载配置，无需重启应用
✅ 配置项加密：支持 JSON/XML 配置文件敏感字段加密，适配数据库连接串、密钥等隐私配置
✅ 增强型强类型绑定：优化实体绑定逻辑，支持嵌套实体、数组、枚举、默认值填充
✅ 多配置源聚合：统一整合 JSON、XML、INI、环境变量、命令行、内存配置、远程配置源
✅ 配置合法性校验：内置数据校验规则，启动时自动检测必填项、数值范围、格式合法性
✅ 分层配置合并：支持全局配置 + 项目私有配置 + 本地开发配置三级合并策略
✅ 轻量无依赖：除 .NET 官方基础包外，无第三方重型依赖，体积小巧、性能高效

# 运行环境
支持框架：.NET 6 / .NET 7 / .NET 8（LTS 长期支持版本）
兼容平台：Windows / Linux /macOS/ Docker
基础依赖：Microsoft.Extensions.Configuration 系列官方包

# 安装方式
方式 1：NuGet 包安装（推荐）
Package Manager
powershell
Install-Package Enterprise.Configuration
.NET CLI
bash
运行
dotnet add package Enterprise.Configuration
方式 2：源码编译引用
克隆本仓库
bash
运行
git clone https://github.com/CreatorXuYuFei/Enterprise.Configuration.git
cd Enterprise.Configuration
编译项目
bash
运行
dotnet build -c Release
在你的业务项目中引用编译后的 dll 或项目文件

# 快速上手
参考单元测试用例：Enterprise.Configuration.Tests

# 项目目录结构
plaintext
Enterprise.Configuration/
├── src/
│   ├── Enterprise.Configuration/   # 核心类库源码
│   │   ├── Builders/              # 配置构建器
│   │   ├── Providers/              # 各类配置源实现
│   │   ├── Encryption/             # 配置加密模块
│   │   ├── Validation/             # 配置校验模块
│   │   └── Extensions/             # 扩展方法
├── tests/
│   ├── Enterprise.Configuration.Tests/  # 单元测试项目
├── samples/                        # 示例项目
│   ├── ConsoleDemo/                # 控制台使用示例
│   └── WebApiDemo/                 # ASP.NET Core 使用示例
├── LICENSE                         # 开源协议
└── README.md                       # 项目说明文档

# 执行全量单元测试
dotnet test

# 代码规范
遵循 .NET 官方编码规范
新增功能必须补充对应单元测试
提交描述清晰说明改动内容

# 开源协议
本项目基于 MIT License 开源，详见 LICENSE 文件。
可自由用于个人、商业项目，无需额外授权。

# 👤 作者 & 仓库地址
作者：CreatorXuYuFei
GitHub：https://github.com/CreatorXuYuFei/Enterprise.Configuration


