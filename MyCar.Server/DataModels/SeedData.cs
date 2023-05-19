using MyCar.Server.Controllers.Jwt;
using MyCar.Server.DB;
using System.Security.Cryptography;
using System.Text.Json;

namespace MyCar.Server.DataModels
{
    public static class SeedData
    {
        public static void Initialize(MyCar_DBContext context)
        {
            if (!context.Statuses.Any())
            {
                var statuses = new[]
                {
                    new Status { StatusName = "Новый" },
                    new Status { StatusName = "Завершен" },
                    new Status { StatusName = "Отменен" }
                };

                context.Statuses.AddRange(statuses);
                context.SaveChanges();
            }

            if (!context.ActionTypes.Any())
            {
                var actionTypes = new[]
                {
                    new ActionType { ActionTypeName = "Поступление" },
                    new ActionType { ActionTypeName = "Продажа" },
                    new ActionType { ActionTypeName = "Списание" }
                };

                context.ActionTypes.AddRange(actionTypes);
                context.SaveChanges();
            }

            if (!context.UserTypes.Any())
            {
                var userTypes = new[]
                {
                    new UserType { TypeName = "Сотрудник" },
                    new UserType { TypeName = "Клиент" },
                    new UserType { TypeName = "Администратор" }
                };

                context.UserTypes.AddRange(userTypes);
                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                User user = new User();
                Passport passport = new Passport();
                passport.Id = context.Passports.Count() + 1;
                context.Passports.Add(passport);
                context.SaveChanges();
                CreatePasswordHash("1234", out byte[] passwordHash, out byte[] passwordSalt);
                user.UserName = "Admin";
                user.Email = "Admin@mail";
                user.PasswordHash = passwordHash;
                user.SaltHash = passwordSalt;
                user.UserTypeId = context.UserTypes.FirstOrDefault(s => s.TypeName == "Администратор").Id;
                user.PassportId = passport.Id;

                context.Users.Add(user);
                context.SaveChanges();
            }
        }
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
