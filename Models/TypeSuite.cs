public class TypeSuite
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public List<Suite> Suites { get; set; } = [];
}
