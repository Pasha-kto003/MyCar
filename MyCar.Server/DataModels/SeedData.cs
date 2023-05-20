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

            if (!context.Units.Any())
            {
                var units = new[]
                {
                    new Unit { UnitName = "Л.С." },
                    new Unit { UnitName = "Л." },
                    new Unit { UnitName = "Тонн" },
                    new Unit { UnitName = "Метры" },
                    new Unit { UnitName = "Сантиметры" },
                    new Unit { UnitName = "Цвет" },
                    new Unit { UnitName = "км/ч" },
                    new Unit { UnitName = "Сек" },
                    new Unit { UnitName = "Шт." },
                    new Unit { UnitName = "М." },
                    new Unit { UnitName = "кВт/ч" },
                    new Unit { UnitName = "Нм" },
                };

                context.Units.AddRange(units);
                context.SaveChanges();
            }

            if (!context.Characteristics.Any())
            {
                var characteristics = new[]
                {
                    new Characteristic { CharacteristicName = "Мощность", UnitId = context.Units.First(s=>s.UnitName == "Л.С.").Id },
                    new Characteristic { CharacteristicName = "Объем", UnitId = context.Units.First(s=>s.UnitName == "Л.").Id },
                    new Characteristic { CharacteristicName = "Вес", UnitId = context.Units.First(s=>s.UnitName == "Тонн").Id },
                    new Characteristic { CharacteristicName = "Высота 2", UnitId = context.Units.First(s=>s.UnitName == "Метры").Id },
                    new Characteristic { CharacteristicName = "Колесная база", UnitId = context.Units.First(s=>s.UnitName == "Сантиметры").Id },
                    new Characteristic { CharacteristicName = "Цвет кузова", UnitId = context.Units.First(s=>s.UnitName == "Цвет").Id },
                    new Characteristic { CharacteristicName = "Макс. Скорость", UnitId = context.Units.First(s=>s.UnitName == "км/ч").Id },
                    new Characteristic { CharacteristicName = "0-100", UnitId = context.Units.First(s=>s.UnitName == "Сек").Id },
                    new Characteristic { CharacteristicName = "Кол-во цилиндров", UnitId = context.Units.First(s=>s.UnitName == "Шт.").Id },
                    new Characteristic { CharacteristicName = "Длина", UnitId = context.Units.First(s=>s.UnitName == "М.").Id },
                    new Characteristic { CharacteristicName = "Вместимость топливного бака", UnitId = context.Units.First(s=>s.UnitName == "Л.").Id },
                    new Characteristic { CharacteristicName = "Мощность электродвигателя", UnitId = context.Units.First(s=>s.UnitName == "кВт/ч").Id },
                    new Characteristic { CharacteristicName = "Крутящий момент", UnitId = context.Units.First(s=>s.UnitName == "Нм").Id },
                };

                context.Characteristics.AddRange(characteristics);
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
