using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceTracker.Core.Models
{
    public class Result<T> : Result
    {
        private object data;

        public T Data { get;}
        public Result(T data, bool isError = false, string errorMessage = ""):base(isError, errorMessage)
        {
            Data = data;
        }

        public Result(object data)
        {
            this.data = data;
        }
    }

    public class Result
    {
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }

        public Result(bool isError = false, string errorMessage = "")
        {
            ErrorMessage = errorMessage;
            IsError = isError;
        }
    }
}
