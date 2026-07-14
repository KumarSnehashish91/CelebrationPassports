using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Passports.DTOs;
using CelebrationPassports.Application.Passports.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using FluentValidation;

namespace CelebrationPassports.Application.Passports.Services;

public class PassportService : IPassportService
{
    private readonly IPassportRepository _passportRepository;
    private readonly IGenericRepository<PassportPerson> _passportPersonRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IPassportAccessGuard _accessGuard;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreatePassportRequest> _createValidator;

    public PassportService(
        IPassportRepository passportRepository,
        IGenericRepository<PassportPerson> passportPersonRepository,
        IUserProfileRepository userProfileRepository,
        IPassportAccessGuard accessGuard,
        IUnitOfWork unitOfWork,
        IValidator<CreatePassportRequest> createValidator)
    {
        _passportRepository = passportRepository;
        _passportPersonRepository = passportPersonRepository;
        _userProfileRepository = userProfileRepository;
        _accessGuard = accessGuard;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
    }

    public async Task<PassportDetailDto> CreateAsync(Guid userId, CreatePassportRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var profile = await _userProfileRepository.GetUserProfileByIdAsync(userId);
        var ownerName = profile is null
            ? "Unknown"
            : string.IsNullOrWhiteSpace(profile.DisplayName)
                ? $"{profile.FirstName} {profile.LastName}".Trim()
                : profile.DisplayName;

        var passport = new Passport
        {
            Id = Guid.NewGuid(),
            OwnerUserId = userId,
            Title = request.Title,
            Status = PassportStatus.Active,
            CreatedOn = DateTime.UtcNow
        };

        await _passportRepository.AddAsync(passport);

        var owner = new PassportPerson
        {
            Id = Guid.NewGuid(),
            PassportId = passport.Id,
            UserId = userId,
            Name = ownerName,
            Role = PassportPersonRole.Owner
        };

        await _passportPersonRepository.AddAsync(owner);

        await _unitOfWork.SaveChangesAsync();

        return new PassportDetailDto
        {
            Id = passport.Id,
            Title = passport.Title,
            Status = passport.Status,
            OwnerUserId = passport.OwnerUserId,
            CreatedOn = passport.CreatedOn,
            People = new List<PassportPersonDto>
            {
                new()
                {
                    Id = owner.Id,
                    UserId = owner.UserId,
                    Name = owner.Name,
                    Role = owner.Role
                }
            }
        };
    }

    public async Task<IReadOnlyList<PassportSummaryDto>> GetMineAsync(Guid userId)
    {
        var passports = await _passportRepository.GetForUserAsync(userId);

        return passports.Select(p => new PassportSummaryDto
        {
            Id = p.Id,
            Title = p.Title,
            Status = p.Status,
            PeopleCount = p.People.Count,
            IsOwner = p.OwnerUserId == userId
        }).ToList();
    }

    public async Task<PassportDetailDto> GetByIdAsync(Guid userId, Guid passportId)
    {
        await _accessGuard.EnsureMemberAsync(userId, passportId);

        var passport = await _passportRepository.GetByIdWithPeopleAsync(passportId)
            ?? throw new NotFoundException("Passport not found.");

        return new PassportDetailDto
        {
            Id = passport.Id,
            Title = passport.Title,
            Status = passport.Status,
            OwnerUserId = passport.OwnerUserId,
            CreatedOn = passport.CreatedOn,
            People = passport.People.Select(p => new PassportPersonDto
            {
                Id = p.Id,
                UserId = p.UserId,
                Name = p.Name,
                Role = p.Role
            }).ToList()
        };
    }
}
