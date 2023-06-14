namespace BotFrameworkApp
{
    public class MemoryChatState : IChatState
    {
        private string? _state;

        public MemoryChatState()
        {
            _state = null;
        }

        public Task<string?> LoadAsync()
        {
            return Task.FromResult(_state);
        }

        public Task SaveAsync(string state)
        {
            _state = state;
            return Task.CompletedTask;
        }
    }
}
