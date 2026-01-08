using ShoppingProject.Core.ContributorAggregate;

namespace ShoppingProject.UseCases.Contributors.Get;

public record GetContributorQuery(ContributorId ContributorId) : IQuery<Result<ContributorDto>>;
