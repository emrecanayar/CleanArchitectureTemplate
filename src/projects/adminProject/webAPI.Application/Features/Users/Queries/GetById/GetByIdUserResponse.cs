using Core.Application.Responses;
using Core.Domain.ComplexTypes.Enums;

namespace webAPI.Application.Features.Users.Queries.GetById
{
    public class GetByIdUserResponse : IResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public AuthenticatorType AuthenticatorType { get; set; }
        public CultureType CultureType { get; set; }
        public RecordStatu Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public GetByIdUserResponse()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
        }

        public GetByIdUserResponse(Guid id, string firstName, string lastName, string email, AuthenticatorType authenticatorType, CultureType cultureType, RecordStatu status, DateTime createdDate) : this()
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            AuthenticatorType = authenticatorType;
            CultureType = cultureType;
            Status = status;
            CreatedDate = createdDate;
        }
    }
}
