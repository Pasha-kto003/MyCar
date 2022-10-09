using ModelsApi;

namespace MyCar.Server.DB
{
    public partial class Passport
    {
        public static explicit operator PassportApi(Passport passport)
        {
            return new PassportApi
            {
                ID = passport.Id,
                Seria = passport.Seria,
                Number = passport.Number,
                DateEnd = passport.DateEnd,
                DateStart = passport.DateStart,
                FirstName = passport.FirstName,
                LastName = passport.LastName,
                Patronimyc = passport.Patronymic
            };
        }
        public static explicit operator Passport(PassportApi passport)
        {
            return new Passport
            {
                Id = passport.ID,
                Seria = passport.Seria,
                Number = passport.Number,
                DateEnd = passport.DateEnd,
                DateStart = passport.DateStart,
                FirstName = passport.FirstName,
                LastName = passport.LastName,
                Patronymic = passport.Patronimyc
            };
        }
    }
}
