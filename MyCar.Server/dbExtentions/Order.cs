using ModelsApi;

namespace MyCar.Server.DB
{
    public partial class Order
    {
        public static explicit operator OrderApi(Order order)
        {
            return new OrderApi
            {
                ID = order.Id,
                ActionTypeId = order.ActionTypeId,
                ClientId = order.ClientId,
                DateOfOrder = order.DateOfOrder,
                StatusId = order.StatusId
            };
        }

        public static explicit operator Order(OrderApi order)
        {
            return new Order
            {
                Id = order.ID,
                ActionTypeId = order.ActionTypeId,
                ClientId = order.ClientId,
                DateOfOrder = order.DateOfOrder,
                StatusId = order.StatusId
            };
        }
    }
}
