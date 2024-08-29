namespace IpDeputyApi.Exceptions
{
    [Serializable]
    public class HttpException : Exception
    {
        public int StatusCode { private set; get; }
        public object? ResponseData { private set; get; }

        public HttpException()
        { 
        }

        public HttpException(string message) : base(message)
        { 
        }

        public HttpException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpException(string message, int statusCode, object responseData) : base(message)
        {
            StatusCode = statusCode;
            ResponseData = responseData;
        }

        public HttpException(string message, Exception innerException) : base(message, innerException)
        { 
        }

        public HttpException(string message, Exception innerException, int statusCode) : base(message, innerException)
        {
            StatusCode = statusCode;
        }

        public HttpException(string message, Exception innerException, int statusCode, object responseData) : base(message, innerException)
        {
            StatusCode = statusCode;
            ResponseData = responseData;
        }
    }
}
