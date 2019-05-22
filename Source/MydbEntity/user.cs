using System.Runtime.Serialization;

namespace Tset.Entity
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class user
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string name { get; set; }
    }
}
