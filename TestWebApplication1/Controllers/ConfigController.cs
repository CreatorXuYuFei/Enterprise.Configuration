using Enterprise.Configuration.Abstractions;
using Enterprise.Configuration.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace TestWebApplication1
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigController : ControllerBase
    {
        private readonly IConfigRoot _configRoot;

        // 仅注入全局配置根，启动阶段无绑定
        public ConfigController(IConfigRoot configRoot)
        {
            _configRoot = configRoot;
        }

        /// <summary>被动读取：普通单个实体</summary>
        [HttpGet("app")]
        public IActionResult GetAppConfig()
        {
            // 【被动触发】访问接口时才绑定实体
            var section = _configRoot.GetSection("ConnectionStrings");
            var appConfig = section.BindModel<List<Program.DbConnectionConfig>>();
            return Ok(appConfig);
        }
    }
}