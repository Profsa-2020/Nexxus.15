using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Nexxus15.Pages
{
    public class Man_PagtoModel : PageModel
    {
        public void OnGet(string ope, string cod)
        {
            string cod_v = "";
            string des_v = "";
            string obs_v = "";
            TempData["ope"] = (ope == null ? "" : ope);
            TempData["cod"] = (cod == null ? "" : cod);
            TempData["cha"] = Dados.Ultimo_Pag();
            TempData["dad"] = Dados.Lista_Pag();
            if (Funcoes.wkschausu == 0)
            {
                Response.Redirect("Login", true);
            }
            if (ope == "2" || ope == "3")
            {
                byte sta = Dados.Ler_Pagto(cod, ref cod_v, ref des_v, ref obs_v);
                TempData["num_v"] = cod;
                TempData["cod_v"] = cod_v;
                TempData["des_v"] = des_v;
                TempData["obs_v"] = obs_v;
            }
        }
        public void OnPost(string ope, string cod)
        {
            byte ret = 0;
            string sql = "";
            TempData["ope"] = ope;
            TempData["cod"] = cod;
            string sal = Request.Form["sal"];
            string cod_v = Request.Form["cod_v"];
            string des_v = Request.Form["des_v"];
            string obs_v = Request.Form["obs_v"];
            if (ope == "1")
            {
                sql  = "Insert into CadFormaPagto (";
                sql += "codigo, ";
                sql += "descricao, ";
                sql += "observacoes ";
                sql += ") values ( ";
                sql += "'" + cod_v + "',";
                sql += "'" + des_v + "',";
                sql += "'" + obs_v + "')";
                ret = Dados.ComandoSql(sql);
            }
            if (ope == "2")
            {
                sql = "Update CadFormaPagto set ";
                sql += "codigo = '" + cod_v + "',";
                sql += "descricao = '" + des_v + "',";
                sql += "observacoes = '" + obs_v + "' ";
                sql += "where Pk = " + cod;
                ret = Dados.ComandoSql(sql);
            }
            if (ope == "3")
            {
                sql = "Delete from CadFormaPagto where Pk = " + cod;
                ret = Dados.ComandoSql(sql);
            }
            TempData["cha"] = Dados.Ultimo_Pag();
            TempData["dad"] = Dados.Lista_Pag();
        }

    }
}
