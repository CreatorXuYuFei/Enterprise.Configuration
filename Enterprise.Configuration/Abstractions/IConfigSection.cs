namespace Enterprise.Configuration.Abstractions
{
    /// <summary>配置分段节点</summary>
    public interface IConfigSection : IConfigRoot
    {
        string Path { get; }

        //公有只读属性，获取所属根配置
        IConfigRoot Root { get; }
    }
}
