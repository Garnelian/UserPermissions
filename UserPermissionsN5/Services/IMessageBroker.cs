namespace UserPermissionsN5.Services
{
    public interface IMessageBroker
    {
        Task SendMessageAsync(Guid id, string operation);
    }

}
