using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Nexxus15.Pages
{
    public class Man_EmitenteModel : PageModel
    {
        public void OnGet(string ope, string cod)
        {
            TempData["ope"] = (ope == null ? "" : ope);
            TempData["cod"] = (cod == null ? "" : cod);
            TempData["est_l"] = Dados.Lista_Est();
            TempData["cod_v"] = Dados.Ultimo_Emi();
            if (Funcoes.wkschausu == 0)
            {
                Response.Redirect("Login", true);
            }
        }
        public void OnPost(string ope, string cod)
        {
            TempData["ope"] = ope;
            TempData["cod"] = cod;
            string sal = Request.Form["sal"];
            TempData["est_l"] = Dados.Lista_Est();
            TempData["cod_v"] = Dados.Ultimo_Emi();
            if (Funcoes.wkschausu == 0)
            {
                Response.Redirect("Login", true);
            }
        }
        public string OnGetCarrega_Cid(string est)
        {
            String cid = "";

            return cid;
        }
    }
}