using EnterpriseApp.Application.Common;
using EnterpriseApp.Application.Common.Interfaces;
using EnterpriseApp.Application.Features.Auth.Dtos;
using FluentValidation;
using MediatR;

namespace EnterpriseApp.Application.Features.Auth.Commands;

/// <summary>
/// Command to refresh an access token.
/// </summary>
public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<LoginResponse>>;

/// <summary>
/// Validator for RefreshTokenCommand.
/// </summary>
public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}

/// <summary>
/// Handler for RefreshTokenCommand.
/// </summary>
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResponse>>
{
    private readonly IIdentityService _identityService;

    public RefreshTokenCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<LoginResponse>(result.Error!);
        }

        var authResult = result.Value;

        return Result.Success(new LoginResponse
        {
            UserId = authResult.UserId,
            Email = authResult.Email,
            FullName = authResult.FullName,
            AccessToken = authResult.AccessToken,
            RefreshToken = authResult.RefreshToken,
            ExpiresAt = authResult.ExpiresAt,
            Roles = authResult.Roles,
            Permissions = authResult.Permissions,
            RequiresTwoFactor = false
        });
    }
}
