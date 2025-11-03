namespace SmagerUp.Core.API.Models
{
    public class ApiResponse<T>
    {
        public bool Ok { get; set; }
        public string Msg { get; set; } = string.Empty;
        public T? Data { get; set; }

        // Success response
        public static ApiResponse<T> Success(T data, string msg = "")
        {
            return new ApiResponse<T> { Ok = true, Msg = msg, Data = data };
        }

        // Failure response
        public static ApiResponse<T> Fail(string msg, T? data = default)
        {
            return new ApiResponse<T> { Ok = false, Msg = msg, Data = data };
        }
    }
}
