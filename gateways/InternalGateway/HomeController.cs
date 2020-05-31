using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace InternalGateway
{
    public class HomeController : AbpController
    {
        public void Index()
        {
            Redirect("/swagger/index.html");
        }
    }
}
