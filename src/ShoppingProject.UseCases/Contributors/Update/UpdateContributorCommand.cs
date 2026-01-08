using ShoppingProject.Core.ContributorAggregate;

namespace ShoppingProject.UseCases.Contributors.Update;

public record UpdateContributorCommand(ContributorId ContributorId, ContributorName NewName) : ICommand<Result<ContributorDto>>;
