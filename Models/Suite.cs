public class Suite
{
    public int Id { get; set; }
    public required string Name { get; set; } 
    public int Capacity { get; set; } = 1;
    public required decimal Price { get; set; }

    public int MotelId { get; set; }
    public required Motel Motel { get; set; }

    public int TypeSuiteId { get; set; }
    public required TypeSuite TypeSuite { get; set; }
}
