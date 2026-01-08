using ShoppingProject.Core.ContributorAggregate;

namespace ShoppingProject.UseCases.Contributors.Delete;

public record DeleteContributorCommand(ContributorId ContributorId) : ICommand<Result>;
