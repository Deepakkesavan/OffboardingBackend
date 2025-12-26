using Offboarding_inner_api.OffboardingGlobalExceptions.Model;

namespace Offboarding_inner_api.OffboardingGlobalExceptions.Exceptions
{
    public class ErrorResponseException : Exception
    {
        public BaseException Error { get; }

        public ErrorResponseException(BaseException error) : base(error.Message)
        {
            Error = error;
        }
    }
}
