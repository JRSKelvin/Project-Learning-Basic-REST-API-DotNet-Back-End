namespace Project_Learning_Basic_REST_API_DotNet_Back_End.Models.Common
{
    public class SuccessResponse<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = "Request Successfully";
        public T? Data { get; set; }
        public SuccessResponse(int statusCode, string message, T data)
        {
            StatusCode = statusCode;
            if (!string.IsNullOrEmpty(message)) Message = message;
            Data = data;
        }
    }
}
