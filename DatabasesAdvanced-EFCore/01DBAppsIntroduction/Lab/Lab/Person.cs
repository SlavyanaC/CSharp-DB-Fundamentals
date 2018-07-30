namespace Lab
{
    public class Person
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string JobTitle { get; set; }

        public override string ToString()
        {
            var personDate = $"{this.FirstName} {this.LastName}";
            return personDate;
        }
    }
}
