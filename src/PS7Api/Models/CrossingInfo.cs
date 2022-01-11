using System.ComponentModel.DataAnnotations.Schema;
using PS7Api.Controllers;

namespace PS7Api.Models;

public class CrossingInfo
{
    public int Id { get; set; }
    public int NbPassengers { get; set; }
    public int TypeId { get; set; }
    public TypePassenger? Type { get; set; }
    public DateTime EntryTollTime { get; set; }
    public DateTime? ExitTollTime { get; set; }
    public int EntryTollId { get; set; }
    public TollOffice EntryToll { get; set; }
    public int? ExitTollId { get; set; }
    public TollOffice? ExitToll { get; set; }
    [NotMapped]
    public bool Valid => ExitTollId != null;
    public Transport Transport { get; set; }
}

public abstract class TypePassenger
{
    public int Id { get; set; }
}

public class Human : TypePassenger
{
    public HumanEnum Type { get; set; }
}

public enum HumanEnum
{
    Tourist,
    Professional
}

public class Merchendise : TypePassenger
{
    public string TypeVehicle { get; set; }
    public string TypeMerchendise { get; set; }
    public string QuantityMerchendise { get; set; }
}

public enum Transport
{
    Boat, Ship, Airplace, Car, Train, Truck
}