using System;
using System.Collections.Generic;
using Core.Domain.ComplexTypes.Enums;
using Core.Domain.Entities;
using Core.Test.Application.FakeData;

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
                    Id = Guid.Parse("e16d144a-8684-4f28-8d24-e816a560dfb3"),
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
                    Id = Guid.Parse("b55c4530-45e8-4391-83a7-70273067edab"),
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
