using ModelsApi;

namespace MyCar.Server.DB
{
    public partial class ActionType
    {
        public static explicit operator ActionTypeApi(ActionType actionType)
        {
            return new ActionTypeApi
            {
                ID = actionType.Id,
                ActionTypeName = actionType.ActionTypeName
            };
        }

        public static explicit operator ActionType(ActionTypeApi actionType)
        {
            return new ActionType
            {
                Id = actionType.ID,
                ActionTypeName = actionType.ActionTypeName
            };
        }
    }
}
