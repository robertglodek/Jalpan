using Jalpan.Time;
using Microsoft.Extensions.Logging;
using Taskly.Services.Identity.Application.Events;
using Taskly.Services.Identity.Application.Services;
using Taskly.Services.Identity.Core.Entities;
using Taskly.Services.Identity.Core.Exceptions;
using Taskly.Services.Identity.Domain.ValueObjects;

namespace Taskly.Services.Identity.Application.Commands.Handlers;

internal sealed class SignUpHandler(IUserRepository userRepository, IDateTime dateTime, IPasswordService passwordService, ILogger<SignUpHandler> logger) : ICommandHandler<SignUp, Empty>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IDateTime _dateTime = dateTime;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly ILogger<SignUpHandler> _logger = logger;

    public async Task<Empty> HandleAsync(SignUp command, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetAsync(command.Email);
        if (user is not null)
        {
            _logger.LogError("Email already in use: {Email}", command.Email);
            throw new EmailInUseException(command.Email);
        }

        var role = Role.From(command.Role);
        var password = _passwordService.Hash(command.Password);

        user = new User(command.UserId, command.Email, password, role, _dateTime.Now);
        await _userRepository.AddAsync(user);

        _logger.LogInformation("Created an account for the user with id: {UserId}", user.Id);
        await _messageBroker.PublishAsync(new SignedUp(user.Id, user.Email, user.Role));
    }
}
