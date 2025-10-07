using System;
using System.Collections.Generic;
using Core.Domain.Entities;
using Core.Test.Application.FakeData;

namespace Application.Tests.Mocks.FakeData
{
    public class OperationClaimFakeData : BaseFakeData<OperationClaim, Guid>
    {
        public override List<OperationClaim> CreateFakeData()
        {
            List<OperationClaim> data = new()
            {
                new OperationClaim
                {
                    Id = Guid.Parse("a128588a-ae6b-40d6-96e7-4bdf54ddf1d4"),
                    Name = "Admin",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                },
                new OperationClaim
                {
                    Id = Guid.Parse("ccec1dad-7d0b-42ac-9b7c-6b71f3a11124"),
                    Name = "User",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                },
            };

            return data;
        }
    }
}
