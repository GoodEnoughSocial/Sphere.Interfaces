using Orleans;

namespace Sphere.Interfaces;

public interface IUserProfile : IGrainWithIntegerKey
{
    public Description Description { get; set; }
    public string AvatarMediaUrl { get; set; }
    public string HeaderMediaUrl { get; set; }
}

[Serializable]
public record Description(string Bio, string Website, string Location);
