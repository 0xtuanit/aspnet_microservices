namespace Contracts.Sagas.OrderManager;

public interface ISagaOrderManager<in TInput, out TOuput> where TInput : class
    where TOuput : class
{
    public TOuput CreateOrder(TInput input);
    public TOuput RollBackOrder(string username, string documentNo, long orderId);
}