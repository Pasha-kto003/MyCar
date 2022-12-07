using ModelsApi;

namespace MyCar.Server.DB
{
    public partial class Status
    {
        public static explicit operator StatusApi(Status status)
        {
            return new StatusApi
            {
                ID = status.Id,
                StatusName = status.StatusName
            };
        }

        public static explicit operator Status(StatusApi status)
        {
            return new Status
            {
                Id = status.ID,
                StatusName = status.StatusName
            };
        }
    }
}
