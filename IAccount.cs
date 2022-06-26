using System.Net;
using Orleans;

namespace Sphere.Interfaces;

public interface IAccount : IGrainWithStringKey
{
    public string Email { get; set; }
    public CreatedWith CreatedWith { get; set; }
    public string UserName { get; set; }
    public long AccountId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string AccountDisplayName { get; set; }
    public TimeZoneInfo TimeZoneInfo { get; set; }
    public DateOnly BirthDate { get; set; }
    public IPAddress CreationIP { get; set; }
    public string PhoneNumber { get; set; }
}

public enum CreatedWith
{
    Unknown = 0,
    Web,
    Mobile,
}
