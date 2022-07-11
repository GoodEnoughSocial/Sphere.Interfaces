using Orleans;
using Sphere.Shared.Models;

namespace Sphere.Interfaces;

public interface IAccount : IGrainWithStringKey
{
    Task<AccountState> RegisterAccount(AccountState account);
    Task<AccountState> GetAccountState();
    Task<AccountState> UpdateAccount(AccountState account);
}
