using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorage.Wrappers
{
    public class Response<T>
    {
        public Response()
        {
        }
        public Response(T data, string message = null)
        {
            Succeed = true;
            Message = message;
            Data = data;
        }
        public Response(string message, object errors = null)
        {
            Succeed = false;
            Message = message;
            Errors = errors;
        }
        public bool Succeed { get; set; }
        public string Message { get; set; }
        public object Errors { get; set; }
        public T Data { get; set; }
    }
}
