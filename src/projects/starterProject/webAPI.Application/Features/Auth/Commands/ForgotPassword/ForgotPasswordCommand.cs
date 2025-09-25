using System.Net;
using System.Text.Json.Serialization;
using Application.Features.Auth.Rules;
using Application.Services.AuthenticatorService;
using Application.Services.AuthService;
using Application.Services.Repositories;
using Application.Services.UsersService;
using AutoMapper;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Constants;
using Core.Domain.Entities;
using Core.Mailing;
using MediatR;
using MimeKit;

namespace Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommand : IRequest<CustomResponseDto<bool>>
{
    public string Email { get; set; }

    [JsonIgnore]
    public string? IpAddress { get; set; }

    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, CustomResponseDto<bool>>
    {
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IMapper _mapper;
        private readonly IAuthenticatorService _authenticatorService;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IMailService _mailService;

        public ForgotPasswordCommandHandler(
            IMapper mapper,
            IUserService userService,
            IAuthService authService,
            AuthBusinessRules authBusinessRules,
            IAuthenticatorService authenticatorService,
            IUserRepository userRepository,
            IMailService mailService)
        {
            _mapper = mapper;
            _userService = userService;
            _authService = authService;
            _userRepository = userRepository;
            _authBusinessRules = authBusinessRules;
            _authenticatorService = authenticatorService;
            _mailService = mailService;
        }

        public async Task<CustomResponseDto<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userService.GetAsync(
               predicate: u => u.Email == request.Email,
               enableTracking: false,
               cancellationToken: cancellationToken);

            await _authBusinessRules.UserShouldBeExistsWhenSelected(user);
            string password = TempPasswordCreator();
            Core.Security.Hashing.HashingHelper.CreatePasswordHash(password, passwordHash: out byte[] passwordHash, passwordSalt: out byte[] passwordSalt);
            user!.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _userRepository.UpdateAsync(user);
            try
            {
                var fullname = user.FirstName + " " + user.LastName;
                var tolist = new List<MailboxAddress>
                        {
                            new MailboxAddress($"{fullname}", $"{user.Email}"),
                        };
                Mail mail = new Mail()
                {
                    ToList = tolist,
                    Subject = $"Şifre Sıfırlama Talebi - EduWee",
                    HtmlBody = StaticParameter.ForgotPasswordMailTemplate.Replace("{Username}", fullname).Replace("{Password}", password),
                };
                await _mailService.SendEmailAsync(mail);
            }
            catch (Exception)
            {
            }

            return CustomResponseDto<bool>.Success((int)HttpStatusCode.OK, true, true);
        }

        public string TempPasswordCreator(int uzunluk = 8)
        {
            const string karakterler = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
            var rastgele = new Random();
            return new string(Enumerable.Repeat(karakterler, uzunluk)
                .Select(s => s[rastgele.Next(s.Length)]).ToArray());
        }
    }
}