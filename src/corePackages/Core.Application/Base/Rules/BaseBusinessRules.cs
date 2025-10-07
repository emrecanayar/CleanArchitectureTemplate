using System.Linq.Expressions;
using Core.Application.Base.Constants;
using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exceptions.Types;
using Core.Domain.Entities.Base;
using Core.Persistence.Repositories;

namespace Core.Application.Base.Rules
{
    public class BaseBusinessRules<TEntity, TEntityId> : BaseBusinessRules
       where TEntity : Entity<TEntityId>
       where TEntityId : struct, IEquatable<TEntityId>
    {
        private readonly IAsyncRepository<TEntity, TEntityId> _asyncRepository;

        public BaseBusinessRules(IAsyncRepository<TEntity, TEntityId> asyncRepository)
        {
            _asyncRepository = asyncRepository;
        }

        public async Task BaseIdShouldExistWhenSelected(TEntityId id)
        {
            TEntity? baseData = await _asyncRepository.GetAsync(x => x.Id.Equals(id), enableTracking: false);
            if (baseData is null)
            {
                throw new BusinessException(typeof(TEntity).Name + BaseMessages.EntityDoesNotExist);
            }
        }

        public Task BaseShouldBeExist(TEntity? baseData)
        {
            if (baseData is null)
            {
                throw new NotFoundException(typeof(TEntity).Name + BaseMessages.EntityDoesNotExist);
            }

            return Task.CompletedTask;
        }

        public LambdaExpression GetIncludeLambda(Type entityType, string include)
        {
            string[] navigationProperties = include.Split('.');
            ParameterExpression parameterExpression = Expression.Parameter(entityType);
            Expression expression = parameterExpression;

            foreach (string navigationProperty in navigationProperties)
            {
                expression = Expression.PropertyOrField(expression, navigationProperty);
            }

            return Expression.Lambda(expression, parameterExpression);
        }
    }
}
