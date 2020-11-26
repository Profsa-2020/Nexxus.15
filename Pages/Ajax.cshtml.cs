using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Nexxus15.Pages
{
    public class AjaxModel : PageModel
    {
        public void OnGet(int est)
        {

            TempData["cid_v"] = Dados.Lista_Cid(est);

        }
    }
}
