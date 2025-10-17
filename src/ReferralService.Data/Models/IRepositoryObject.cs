namespace ReferralService.Data.Models;

/// <summary>
/// Interface for repository objects with a Reference property.
///
/// Ensures consistent behaviour in the repository layer.
/// </summary>
public interface IRepositoryObject
{
    Guid Id { get; set; }
}