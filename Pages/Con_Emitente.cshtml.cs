using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Nexxus15.Pages
{
    public class Con_EmitenteModel : PageModel
    {
        public void OnGet()
        {
            if (Funcoes.wkschausu == 0)
            {
                Response.Redirect("Login", true);
            }
            TempData["dad"] = Dados.Lista_Emi();
        }
    }
}
