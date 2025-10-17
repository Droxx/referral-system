namespace ReferralService.Data.Models;

// In production I would expect these enums to be defined in a shared library
// to avoid duplication between Data and Contracts projects.
public enum RentalState
{
    InProgress,
    Finished
}