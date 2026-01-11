using System.Security.Claims;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.UseCases.Users.Interfaces;
using ShoppingProject.UseCases.Users.Specifications;
using ShoppingProject.Core.Interfaces;
using ShoppingProject.UseCases.Users.DTOs;

namespace ShoppingProject.UseCases.Users.Commands.VerifyEmail;    
    
public class VerifyEmailHandler : IRequestHandler<VerifyEmailCommand, Unit> { 
    private readonly  IRepository<ApplicationUser> _userRepository; 
    public VerifyEmailHandler( IRepository<ApplicationUser> userRepository) { _userRepository = userRepository; } 
    public async ValueTask<Unit> Handle(VerifyEmailCommand request, CancellationToken cancellationToken) { 
        var user = await _userRepository.GetByIdAsync(Guid.Parse(request.UserId), cancellationToken); 
        if (user == null ) throw new UnauthorizedAccessException("Kullanıcı bulunamadı."); 
        await _userRepository.UpdateAsync(user, cancellationToken); 
        return Unit.Value; 
    } 
}

