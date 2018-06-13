using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCoreManageLinux.Web
{
    public class ResponseModel
    {
        public bool IsSuccess { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public object Object { get; set; }
    }
}
