using Enterprise.Configuration.Abstractions;
using Enterprise.Configuration.Core;
using Enterprise.Configuration.Extensions;
using System.ComponentModel.DataAnnotations;

namespace TestWebApplication1
{
    public class Program
    {
        ///<summary>
        ///数据库连接配置模型
        ///对应appsettings.json中connectionStrings节点下的每个子节点
        ///</summary>
        public class DbConnectionConfig
        {
            ///<summary>
            ///连接名（如dbconn、algorithm）
            ///</summary>
            public string Name { get; set; }

            ///<summary>
            ///连接字符串
            ///</summary>
            public string ConnectionString { get; set; }

            ///<summary>
            ///数据库提供者名称
            ///</summary>
            public string ProviderName { get; set; }
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

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 模拟命令行参数（实际 = args）
            string[] runArgs = ["--Ai:Timeout=60", "--Db:MaxPool=100"];

            // 构造内存临时配置（优先级最高）
            var memoryData = new Dictionary<string, string?>
            {
                { "App:RunMode", "Production" },
                { "Ai:Debug", "false" }
            };

            // Add services to the container.
            string FilePath = string.Empty;
            //读取程序根目录，默认在根目录下查找
            string baseFile = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "appsettings.json";
            FilePath = FilePath == "" ? baseFile.Replace("\\", "/") : FilePath;
            var _configuration = new ConfigBuilder()
                .AddJsonFile(FilePath, optional: false, reloadOnChange: true)   //基础Json
                .AddEnvironmentVariables("AI_")
                .AddCommandLine(runArgs)
                .AddMemory(memoryData)
                .Build();

            //初始化静态工具（启动阶段仅赋值，无绑定）
            ConfigHelper.Init(_configuration);
            //读取实体配置
            var aiOpts = ConfigHelper.GetConfigList<DbConnectionConfig>("ConnectionStrings");

            // 注入到官方总线
            builder.Services.AddSingleton(_configuration);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
