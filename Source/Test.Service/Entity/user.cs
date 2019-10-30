// ReSharper disable InconsistentNaming
namespace Test.Service.Entity
{
    public class User
    {
        [System.Data.Objects.DataClasses.EdmScalarProperty(EntityKeyProperty = true, IsNullable = false)]
        public int? id { get; set; }
        public string name { get; set; }
    }
}
