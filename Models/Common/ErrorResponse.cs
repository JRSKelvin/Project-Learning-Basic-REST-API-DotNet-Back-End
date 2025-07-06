namespace Project_Learning_Basic_REST_API_DotNet_Back_End.Models.Common
{
    public class ErrorResponse
    {
        public string Title { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public ErrorResponse(int statusCode, string title, string message)
        {
            StatusCode = statusCode;
            if (!string.IsNullOrEmpty(title)) Title = title;
            if (!string.IsNullOrEmpty(message)) Message = message;
        }
    }
}
