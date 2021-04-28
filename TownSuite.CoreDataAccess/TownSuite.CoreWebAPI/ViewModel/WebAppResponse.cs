using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TownSuite.CoreWebAPI.ViewModel
{
    public class WebAppResponse
    {
           
        public bool IsSuucess { get; set; }
        public string Error { get; set; }
        public string ErrorDEsciption { get; set; }
    }
}
