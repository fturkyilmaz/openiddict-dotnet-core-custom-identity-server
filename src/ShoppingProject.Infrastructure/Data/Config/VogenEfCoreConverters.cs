using ShoppingProject.Core.ContributorAggregate;
using Vogen;

namespace ShoppingProject.Infrastructure.Data.Config;

[EfCoreConverter<ContributorId>]
[EfCoreConverter<ContributorName>]
internal partial class VogenEfCoreConverters;
