using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCoreManageLinux.Web.Models
{
    public class ShellCommandViewModel
    {
        public string ShellCommand { get; set; }
        public string Params { get; set; }
        public string Result { get; set; }
    }
}
