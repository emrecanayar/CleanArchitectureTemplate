﻿using Core.Domain.Entities;
using Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IOperationClaimRepository : IAsyncRepository<OperationClaim, Guid>, IRepository<OperationClaim, Guid> { }
