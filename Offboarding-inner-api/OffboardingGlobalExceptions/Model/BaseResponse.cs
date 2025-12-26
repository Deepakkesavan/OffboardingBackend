namespace Offboarding_inner_api.OffboardingGlobalExceptions.Model
{
    public class BaseResponse<T>
    {
        public List<string> Warnings { get; set; } = new();

        public List<BaseException> Errors { get; set; } = new();

        public T? Result { get; set; }
    }
}
