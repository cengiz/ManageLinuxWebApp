using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCoreManageLinux.Web
{
    public class MyTree
    {
        public string key { get; set; }
        public string title { get; set; }
        public bool lazy { get; set; }
        public bool folder { get; set; }
        public bool checkbox { get; set; }
        public string icon { get; set; }
        public MyTree children { get; set; }
    }
}
