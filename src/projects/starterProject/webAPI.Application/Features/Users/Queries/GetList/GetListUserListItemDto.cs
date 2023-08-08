using Core.Application.Dtos;
using static Core.Domain.ComplexTypes.Enums;

namespace Application.Features.Users.Queries.GetList;

public class GetListUserListItemDto : IDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public RecordStatu Status { get; set; }

    public GetListUserListItemDto()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
    }

    public GetListUserListItemDto(int id, string firstName, string lastName, string email, RecordStatu status)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Status = status;
    }
}
