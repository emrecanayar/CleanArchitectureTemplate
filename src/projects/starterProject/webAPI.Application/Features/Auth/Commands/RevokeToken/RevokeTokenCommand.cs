﻿using Application.Features.Auth.Rules;
using Application.Services.AuthService;
using AutoMapper;
using Core.Application.ResponseTypes.Concrete;
using MediatR;
using System.Net;

namespace Application.Features.Auth.Commands.RevokeToken;

public class RevokeTokenCommand : IRequest<CustomResponseDto<RevokedTokenResponse>>
{
    public string Token { get; set; }
    public string IpAddress { get; set; }

    public RevokeTokenCommand()
    {
        Token = string.Empty;
        IpAddress = string.Empty;
    }

    public RevokeTokenCommand(string token, string ipAddress)
    {
        Token = token;
        IpAddress = ipAddress;
    }

    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, CustomResponseDto<RevokedTokenResponse>>
    {
        private readonly IAuthService _authService;
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IMapper _mapper;

        public RevokeTokenCommandHandler(IAuthService authService, AuthBusinessRules authBusinessRules, IMapper mapper)
        {
            _authService = authService;
            _authBusinessRules = authBusinessRules;
            _mapper = mapper;
        }

        public async Task<CustomResponseDto<RevokedTokenResponse>> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            Core.Domain.Entities.RefreshToken? refreshToken = await _authService.GetRefreshTokenByToken(request.Token);
            await _authBusinessRules.RefreshTokenShouldBeExists(refreshToken);
            await _authBusinessRules.RefreshTokenShouldBeActive(refreshToken!);

            await _authService.RevokeRefreshToken(token: refreshToken!, request.IpAddress, reason: "Revoked without replacement");

            RevokedTokenResponse revokedTokenResponse = _mapper.Map<RevokedTokenResponse>(refreshToken);
            return CustomResponseDto<RevokedTokenResponse>.Success((int)HttpStatusCode.OK, revokedTokenResponse, true);
        }
    }
}
