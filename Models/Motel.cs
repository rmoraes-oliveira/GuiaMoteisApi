
public class Motel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string Phone { get; set; }
    
    public List<Suite> Suites { get; set; } = [];
}
