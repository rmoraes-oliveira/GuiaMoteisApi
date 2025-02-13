using System;

public class Reservation
{
    public int Id { get; set; }
    public required DateTime CheckIn { get; set; }
    public required DateTime Checkout { get; set; }
    public required decimal Total { get; set; }

    public int CustomerId { get; set; }
    public required Customer Customer { get; set; }

    public int SuiteId { get; set; }
    public required Suite Suite { get; set; }
}
