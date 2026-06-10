using Enterprise.Configuration.Abstractions;
using Enterprise.Configuration.Core;
using Enterprise.Configuration.Extensions;
using Enterprise.Configuration.DI;
using static Enterprise.Configuration.Abstractions.OptionsInterfaces;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace TestConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 模拟命令行参数（实际 = args）
            string[] runArgs = ["--Ai:Timeout=60", "--Db:MaxPool=100"];

            // 1. 构造内存临时配置（优先级最高）
            var memoryData = new Dictionary<string, string?>
            {
                { "App:RunMode", "Production" },
                { "Ai:Debug", "false" }
            };

            // 2. 链式构建多配置源
            IConfigRoot config = new ConfigBuilder()
                .AddJsonFile("json1.json", optional: false, reloadOnChange: true)   // 1. 基础Json
                .AddEnvironmentVariables(prefix: "AI_")                                 // 2. 环境变量
                .AddCommandLine(runArgs)                                                 // 3. 命令行
                .AddMemory(memoryData)                                                   // 4. 内存（最高优先级）
                .Build();

            // 3. DI 注册
            var services = new ServiceCollection();
            services.AddEnterpriseConfig(config);
            services.Configure<List<AiAgentOptions>>("AiAgent");
            var provider = services.BuildServiceProvider();

            // 4. 取值测试
            Console.WriteLine($"App运行模式：{config["App:RunMode"]}");
            Console.WriteLine($"AI超时时间：{config["AiAgent:Timeout"]}");
            Console.WriteLine($"数据库连接池：{config["Db:MaxPool"]}");

            // 5. 读取实体配置
            var aiOpts = provider.GetRequiredService<IOptions<List<AiAgentOptions>>>().Value;
            Console.WriteLine($"AI端点：{aiOpts[0].Endpoint}");

            Console.ReadLine();
        }

        /// <summary>AI智能体配置实体</summary>
        public class AiAgentOptions
        {
            [Required(ErrorMessage = "AI ApiKey 不能为空")]
            public string ApiKey { get; set; } = string.Empty;

            [Url(ErrorMessage = "Endpoint 地址格式错误")]
            public string Endpoint { get; set; } = string.Empty;

            [Range(1, 300, ErrorMessage = "超时时间必须在 1~300 之间")]
            public int Timeout { get; set; }
        }
    }
}
