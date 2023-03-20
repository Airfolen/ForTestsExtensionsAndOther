// See https://aka.ms/new-console-template for more information

Order order = queueService.GetOrder();

switch (order.GetType())
{
    case (OnlineOrder).
}
if (order.GetType().Name == "OnlineOrder")
    AddOnlineOrder((OnlineOrder)order, dbConnection);
else 
    AddOfflineOrder((OfflineOrder)order, dbConnection);
