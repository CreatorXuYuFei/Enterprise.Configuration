
安装微软依赖包：
Install-Package Microsoft.Extensions.DependencyInjection -Version 9.0.0
Install-Package Microsoft.Extensions.Options -Version 9.0.0

// 1. 系统内置命名空间
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// 2. 微软 DI & Options（ServiceCollection / IOptions 来源）
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// 3. 框架 - 核心接口 & 实现
using Enterprise.Configuration.Abstractions;
using Enterprise.Configuration.Core;

// 4. 配置框架 - 配置源扩展（AddJsonFile/AddCommandLine 等）
using Enterprise.Configuration.Extensions;

// 5. 配置框架 - DI 扩展（AddEnterpriseConfig / Configure<T> ）
using Enterprise.Configuration.DI;