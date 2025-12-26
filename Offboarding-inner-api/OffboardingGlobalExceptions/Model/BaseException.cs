namespace Offboarding_inner_api.OffboardingGlobalExceptions.Model
{
    public class BaseException
    {
        public int StatusCode { get; set; }

        public string Message { get; set; } = string.Empty;

        public string StackTrace { get; set; } = string.Empty;

        public string? InnerException { get; set; }
    }
}
