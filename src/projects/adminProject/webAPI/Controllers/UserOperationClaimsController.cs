using Core.Application.Requests;
using Core.Application.Responses;
using Core.Application.ResponseTypes.Concrete;
using Microsoft.AspNetCore.Mvc;
using webAPI.Application.Features.UserOperationClaims.Commands.Create;
using webAPI.Application.Features.UserOperationClaims.Commands.Delete;
using webAPI.Application.Features.UserOperationClaims.Commands.Update;
using webAPI.Application.Features.UserOperationClaims.Queries.GetById;
using webAPI.Application.Features.UserOperationClaims.Queries.GetList;
using webAPI.Controllers.Base;

namespace webAPI.Controllers
{
    public class UserOperationClaimsController : BaseController
    {
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById([FromRoute] GetByIdUserOperationClaimQuery getByIdUserOperationClaimQuery)
        {
            CustomResponseDto<GetByIdUserOperationClaimResponse> result = await Mediator.Send(getByIdUserOperationClaimQuery);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
        {
            GetListUserOperationClaimQuery getListUserOperationClaimQuery = new() { PageRequest = pageRequest };
            CustomResponseDto<GetPagedListResponse<GetListUserOperationClaimListItemDto>> result = await Mediator.Send(getListUserOperationClaimQuery);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateUserOperationClaimCommand createUserOperationClaimCommand)
        {
            CustomResponseDto<CreatedUserOperationClaimResponse> result = await Mediator.Send(createUserOperationClaimCommand);
            return Created(uri: string.Empty, result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserOperationClaimCommand updateUserOperationClaimCommand)
        {
            CustomResponseDto<UpdatedUserOperationClaimResponse> result = await Mediator.Send(updateUserOperationClaimCommand);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteUserOperationClaimCommand deleteUserOperationClaimCommand)
        {
            CustomResponseDto<DeletedUserOperationClaimResponse> result = await Mediator.Send(deleteUserOperationClaimCommand);
            return Ok(result);
        }
    }
}
