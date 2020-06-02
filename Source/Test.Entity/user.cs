namespace Test.Entity
{
    public class user
    {
        [System.Data.Objects.DataClasses.EdmScalarProperty(EntityKeyProperty = true, IsNullable = false)]
        public string id { get; set; }
        public string name { get; set; }
    }
}
