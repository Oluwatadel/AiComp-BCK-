namespace AiComp.Application.DTOs
{
    public class BaseResponse<T> where T : class
    {

        public bool Status { get; private set; }
        public string? Message { get; private set; }
        public T? Data { get; private set; }

        public void SetValues(string message, bool status, T data)
        {
            Message = message;
            Status = status;
            Data = data;
        }
    }
}
