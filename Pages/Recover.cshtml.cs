using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Nexxus15.Pages
{
    public class RecoverModel : PageModel
    {

        public void OnGet()
        {
            TempData["erro"] = "";
        }
        public void OnPost(string email)
        {
            if (email != null)
            {                
                string nome = "";
                string senha = "";
                string ativo = "";
                string completo = "";
                byte sta = Dados.Verifica_email(email, ref nome, ref senha, ref ativo, ref completo);
                if (nome == "")
                {
                    TempData["erro"] = "E-Mail informado para reenvio de senha não existe no sistema";
                } else {
                    senha = Funcoes.Descriptografa(senha, true);
                    string texto = "";
                    texto += "<head>";
                    texto += "<style type=\"text/css\">";
                    texto += "h3 { color: red; } ";
                    texto += ".geral { font-family: 'Verdana'; font-size: 16px; width: 75%; text-align: center; border: 0.5px solid #cdcece; box-shadow: 10px 15px 5px rgba(0,0,0,.5); margin: 10px auto; } ";
                    texto += ".imagem { width: auto; height: auto; background-color: #283d61; padding: 15px 10px; } ";
                    texto += ".linha { margin: 5px auto; } ";
                    texto += "</style>";
                    texto += "</head>";

                    texto += "<body>";
                    texto += "<div class=\"geral\">";

                    texto += "<div class=\"imagem\">";
                    texto += "<br />";
                    texto += "<a href=\"http://www.tipsoft.com.br\">";
                    texto += "<img src=\"http://www.tipsoft.com.br/images/logo.png\"/>";
                    texto += "</a>";
                    texto += "<br /><br />";
                    texto += "</div>";
                    texto += "<br /><hr />";

                    texto += "<div>";
                    texto += "<h3><strong>Recuperação de Senha de Usuário</strong></h3>";
                    texto += "<h4>" + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToShortTimeString() + " hr</h4>";
                    texto += "</div>";
                    texto += "<br /><hr />";
                    texto += "<p><strong>Olá " + nome + " segue sua senha para acesso a TipSoft Soluções, obrigado !</strong></p>";
                    texto += "<br />";
                    texto += "<div class=\"linha\">";
                    texto += "<p>Nome do usuário: " + completo + "</p>";
                    texto += "<p>E-mail: " + email + "</p>";
                    texto += "<p>Senha: " + senha + "</p>";
                    texto += "<p>Ativo: " + ativo + "</p>";
                    texto += "<a href=\"http://www.tipsoft.com.br\">";
                    texto += "<p><strong>www.tipsoft.com.br" + "</strong></p>";
                    texto += "</a>";
                    texto += "</div>";

                    texto += "</div>";
                    texto += "</body>";
                    byte ret = Funcoes.Envia_email("", email, "", "Solicitação de reenvio de senha TipSoft Soluções", texto);
                    if (ret == 0)
                    {
                        TempData["erro"] = "Senha solicitada pelo usuário foi enviada com Sucesso !";
                    } else {
                        TempData["erro"] = "Erro no envio de e-mail solicitado com senha de acesso !";
                    }
                }
            }

        }
    }
}
