using Core.Application.Responses;
using Core.Domain.ComplexTypes.Enums;

namespace Application.Features.Users.Commands.Create;

public class CreatedUserResponse : IResponse
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public RecordStatu Status { get; set; }

    public CreatedUserResponse()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
    }

    public CreatedUserResponse(Guid id, string firstName, string lastName, string email, RecordStatu status)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Status = status;
    }
}
