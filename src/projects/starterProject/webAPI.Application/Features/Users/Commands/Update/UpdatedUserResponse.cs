using Core.Application.Responses;
using Core.Domain.ComplexTypes.Enums;

namespace Application.Features.Users.Commands.Update;

public class UpdatedUserResponse : IResponse
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public RecordStatu Status { get; set; }

    public UpdatedUserResponse()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
    }

    public UpdatedUserResponse(Guid id, string firstName, string lastName, string email, RecordStatu status)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Status = status;
    }
}
