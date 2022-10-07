using ModelsApi;

namespace MyCar.Server.DB
{
    public partial class Model
    {
        public static explicit operator ModelApi(Model model)
        {
            return new ModelApi
            {
                ID = model.Id,
                ModelName = model.ModelName,
                MarkId = model.MarkId
            };
        }

        public static explicit operator Model(ModelApi model)
        {
            return new Model
            {
                Id = model.ID,
                ModelName = model.ModelName,
                MarkId = model.MarkId
            };
        }
    }
}
