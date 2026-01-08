using ShoppingProject.Core.ContributorAggregate;

namespace ShoppingProject.UseCases.Contributors;
public record ContributorDto(ContributorId Id, ContributorName Name, PhoneNumber PhoneNumber);
