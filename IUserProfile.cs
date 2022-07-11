using Orleans;
using Sphere.Shared.Models;

namespace Sphere.Interfaces;

public interface IUserProfile : IGrainWithIntegerKey
{
    Task<UserProfileState> GetProfile();
    Task<UserProfileState> Update(UserProfileState updatedProfile);
}
