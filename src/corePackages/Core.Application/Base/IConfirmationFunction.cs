using Core.Domain.Entities;
using Core.Domain.Entities.Base;
using static Core.Domain.ComplexTypes.Enums;

namespace Core.Application.Base
{
    public interface IConfirmationFunction<TDto> where TDto : IDto
    {
        void SendToConfirmation(TDto entityDto, User currentUser, string description, string transactionDescription, ConfirmationTypes confirmationTypes);
    }
}