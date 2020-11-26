using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Nexxus15
{
    public class ProcessaController : Controller
    {
        public IActionResult Carrega_Sid(string est)
        {
            return View();
        }

    }
}
