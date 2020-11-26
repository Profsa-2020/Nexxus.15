using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Nexxus15.Pages
{
    public class LoginModel : PageModel
    {
        public void OnGet(string email, string senha)
        {
            TempData["erro"] = "";
        }

        public void OnPost(string email, string senha)
        {
            if (email != null && senha != null)
            {
                int chave = 0;
                string nome = "";
                string completo = "";
                string ativo = "";
                int admin = 0;
                int emite = 0;
                int caixa = 0;
                byte sta = Dados.Valida_login(email, senha, ref chave, ref nome, ref completo, ref ativo, ref admin, ref emite, ref caixa);
                if (sta != 0)
                {
                    TempData["erro"] = "E-mail e/ou senha para acesso ao sistema não está cadastrado";
                }
                else if (ativo != "Sim")
                {
                    TempData["erro"] = "Usuário informado para acesso não está ativo no sistema";
                }
                else if (emite == 0)
                {
                    TempData["erro"] = "Usuário informado não tem permissão para emissão de NFC-e";
                } else {
                    Funcoes.wkschausu = chave;
                    Funcoes.wksnomusu = nome;
                    Funcoes.wksnomcom = completo;
                    Funcoes.wksemausu = email;
                    Response.Redirect("Menu01", true);
                }
            }
        }

    }
}
