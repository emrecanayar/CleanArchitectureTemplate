using Core.Domain.ComplexTypes.Enums;
using Core.Domain.Entities;
using Core.Domain.Entities.Base;

namespace Core.Application.Base
{
    public interface IConfirmationFunction<TDto>
        where TDto : IDto
    {
        void SendToConfirmation(TDto entityDto, User currentUser, string description, string transactionDescription, ConfirmationTypes confirmationTypes);
    }
}
