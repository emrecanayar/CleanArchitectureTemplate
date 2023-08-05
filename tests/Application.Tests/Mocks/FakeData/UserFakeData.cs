using Core.Domain.Entities;
using Core.Test.Application.FakeData;
using System;
using System.Collections.Generic;
using static Core.Domain.ComplexTypes.Enums;

namespace Application.Tests.Mocks.FakeData;

public class UserFakeData : BaseFakeData<User, Guid>
{
    public override List<User> CreateFakeData()
    {
        List<User> data =
            new()
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Emre Can",
                    LastName = "Ayar",
                    Email = "example@email.com",
                    PasswordHash = new byte[] { },
                    PasswordSalt = new byte[] { },
                    Status = RecordStatu.Active,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Uğur Can",
                    LastName = "Balcı",
                    Email = "example2@email.com",
                    PasswordHash = new byte[] { },
                    PasswordSalt = new byte[] { },
                    Status = RecordStatu.Active,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                }
            };
        return data;
    }
}
