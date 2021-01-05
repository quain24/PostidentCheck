namespace Postident.Application.Common.Models
{
    public class CommandResult<TResult>
    {
        private readonly bool _isValid;
        private readonly TResult _result;
        private readonly string _message;

        public static CommandResult<TResult> ValidResult(TResult result, string message) => new(true, result, message);

        public static CommandResult<TResult> InvalidResult(TResult result, string message) => new(false, result, message);

        private CommandResult(bool isValid, TResult result, string message)
        {
            _isValid = isValid;
            _result = result;
            _message = message ?? string.Empty;
        }
    }
}