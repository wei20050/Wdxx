using System;

namespace CardReading.Core
{
    public class CardReaderInfoAttribute : Attribute
    {

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="description">描述</param>
        /// <param name="isComPortRequired">是否需要打开com口</param>
        public CardReaderInfoAttribute(string name, string description, bool isComPortRequired)
        {
            Name = name;
            Description = description;
            IsComPortRequired = isComPortRequired;
        }

        /// <summary>
        ///     读卡器名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     读卡器详细信息
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     是否需要设置Com端口
        /// </summary>
        public bool IsComPortRequired { get; }
        
        /// <summary>
        /// 读卡器类型
        /// </summary>
        public Type Type { get; set; }
    }
}