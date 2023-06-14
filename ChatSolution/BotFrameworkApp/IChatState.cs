namespace BotFrameworkApp
{
    public interface IChatState
    {
        Task<string?> LoadAsync();

        Task SaveAsync(string state);
    }
}
