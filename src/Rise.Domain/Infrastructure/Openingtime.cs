namespace Rise.Domain.Infrastructure;

public class Openingtime : Entity
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateOnly Date { get; set; }

}


