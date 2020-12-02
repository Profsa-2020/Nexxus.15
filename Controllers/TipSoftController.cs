using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nexxus16.Models;
using System.Data;

namespace Nexxus16.Controllers
{
    public class TipSoftController : Controller
    {
        public string Carrega_Cid(string est)    // para o Ajax não pode STATIC a action (rotina)
        {
            string cid = "<option value=\"**\" selected=\"selected\">Selecione a cidade ...</option>";
            if (est != "**") {
                DataTable dados = DadosModel.Lista_Cid(est);
                foreach (DataRow linha in ((System.Data.DataTable)dados).Rows)
                {
                    cid += "<option value=\"" + linha["Pk"] + "\">" + linha["Nome"] + "</ option >";
                }
            }
            return cid;
        }
        public string Digito_Cnpj(string cgc)    
        {
            if (cgc.Length != 14) { return "1"; }
            string sta = "0"; int num = 0; int pos = 5;
            for(int ind = 0; ind < (cgc.Length - 2); ind++)
            {
                num = num + Convert.ToInt32(cgc.Substring(ind, 1)) * pos;
                pos = pos - 1;
                if (pos == 1) { pos = 9; }
            }
            int res1 = 11 - num % 11;
            if (res1 == 10 || res1 == 11) { res1 = 0; }
            num = 0; pos = 6;
            for (int ind = 0; ind < (cgc.Length - 2); ind++)
            {
                num = num + Convert.ToInt32(cgc.Substring(ind, 1)) * pos;
                pos = pos - 1;
                if (pos == 1) { pos = 9; }
            }
            num = num + res1 * 2;
            int res2 = 11 - num % 11;
            if (res2 == 10 || res2 == 11) { res2 = 0; }
            if ((res1.ToString() + res2.ToString()) != cgc.Substring(12,2))
            {
                sta = "2";
            }
            return sta;
        }

    }
}
