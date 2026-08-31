using Bodokado.Application.Common.Interfaces;

namespace Bodokado.Application.Common.Auth.Services;

public class GenericRoleContext : IRoleContext
{
    public string Role { get; }
    public GenericRoleContext(string role) => Role = role;
}
