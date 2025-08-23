namespace JoG {

    /// <summary>提供对象的基本信息（如名称、描述等）。</summary>
    public interface IInformationProvider {

        /// <summary>对象名称</summary>
        string Name { get; }

        /// <summary>对象描述</summary>
        string Description { get; }

        /// <summary>根据键获取信息</summary>
        string GetProperty(string key);
    }
}