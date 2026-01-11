using System.Security.Claims;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.UseCases.Users.Interfaces;
using ShoppingProject.UseCases.Users.Specifications;
using ShoppingProject.Core.Interfaces;
using ShoppingProject.UseCases.Users.DTOs;

namespace ShoppingProject.UseCases.Users.Commands.ResetPassword;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Unit> { 
    private readonly  IRepository<ApplicationUser> _userRepository; 
    public ResetPasswordHandler( 
        IRepository<ApplicationUser> userRepository) { 
        _userRepository = userRepository; 
    } 

    public async ValueTask<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken) { 
            var user = await _userRepository.FirstOrDefaultAsync(
            new UserByEmailSpec(request.Email), cancellationToken);

        if (user == null) throw new UnauthorizedAccessException("Kullanıcı bulunamadı."); 
        await _userRepository.UpdateAsync(user, cancellationToken); 
        return Unit.Value; 
    } 
}

