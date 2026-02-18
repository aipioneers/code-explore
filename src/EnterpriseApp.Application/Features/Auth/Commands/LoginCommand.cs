using EnterpriseApp.Application.Common;
using EnterpriseApp.Application.Common.Interfaces;
using EnterpriseApp.Application.Features.Auth.Dtos;
using FluentValidation;
using MediatR;

namespace EnterpriseApp.Application.Features.Auth.Commands;

/// <summary>
/// Command to authenticate a user.
/// </summary>
public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;

/// <summary>
/// Validator for LoginCommand.
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.");
    }
}

/// <summary>
/// Handler for LoginCommand.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.AuthenticateAsync(request.Email, request.Password, cancellationToken);

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
            RequiresTwoFactor = authResult.RequiresTwoFactor
        });
    }
}
