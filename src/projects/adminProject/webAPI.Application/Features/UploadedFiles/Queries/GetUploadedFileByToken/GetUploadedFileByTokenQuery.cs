using AutoMapper;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities;
using MediatR;
using System.Net;
using webAPI.Application.Features.UploadedFiles.Dtos;
using webAPI.Application.Features.UploadedFiles.Rules;
using webAPI.Application.Services.Repositories;

namespace webAPI.Application.Features.UploadedFiles.Queries.GetUploadedFileByToken
{
    public class GetUploadedFileByTokenQuery : IRequest<CustomResponseDto<UploadedFileDto>>
    {
        public string Token { get; set; }

        public GetUploadedFileByTokenQuery()
        {
            Token = string.Empty;
        }

        public class GetUploadedFileByTokenQueryHandler : IRequestHandler<GetUploadedFileByTokenQuery, CustomResponseDto<UploadedFileDto>>
        {
            private readonly IUploadedFileRepository _uploadedFileRepository;
            private readonly IMapper _mapper;
            private readonly UploadedFileBusinessRules _uploadedFileBusinessRules;

            public GetUploadedFileByTokenQueryHandler(IUploadedFileRepository uploadedFileRepository, UploadedFileBusinessRules uploadedFileBusinessRules, IMapper mapper)
            {
                _uploadedFileRepository = uploadedFileRepository;
                _uploadedFileBusinessRules = uploadedFileBusinessRules;
                _mapper = mapper;
            }

            public async Task<CustomResponseDto<UploadedFileDto>> Handle(GetUploadedFileByTokenQuery request, CancellationToken cancellationToken)
            {
                UploadedFile? uploadedFile = await this._uploadedFileRepository.GetAsync(d => d.Token == request.Token);
                this._uploadedFileBusinessRules.FileShouldExistWhenRequested(uploadedFile);
                UploadedFileDto uploadedFileDto = _mapper.Map<UploadedFileDto>(uploadedFile);
                return CustomResponseDto<UploadedFileDto>.Success((int)HttpStatusCode.OK, uploadedFileDto, isSuccess: true);
            }
        }
    }
}
