namespace Tset.Entity
{
    public class user
    {
        [System.Data.Objects.DataClasses.EdmScalarProperty(EntityKeyProperty = true, IsNullable = false)]
        public int? id { get; set; }
        public string name { get; set; }
    }
}
