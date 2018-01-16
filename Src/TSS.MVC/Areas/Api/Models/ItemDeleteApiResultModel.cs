using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TSS.MVC.Areas.Api.Models
{
    public class ItemDeleteApiResultModel
    {
        public int BankKey { get; set; }
        public string ItemKeys { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
