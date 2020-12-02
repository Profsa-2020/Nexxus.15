using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Configuration;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nexxus16.Models;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Nexxus16.Controllers
{
    public class HomeController : Controller
    {
        [ViewData]
        public static string wksnomusu { get; set; }
        public static string wksemausu { get; set; }

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string email, string senha)
        {
            TempData["erro"] = "";
            int chave = 0; string nome = ""; string ativo = ""; byte emite = 0;
            if (email != null && senha != null) {
                senha = Criptografa(senha, true);
                byte ret = DadosModel.Valida_Login(email, senha, ref chave, ref nome, ref ativo, ref emite);
                if (nome == "")
                {
                    TempData["erro"] = "E-mail e/ou senha para acesso ao sistema não está cadastrado";
                } else if (ativo != "Sim")
                {
                    TempData["erro"] = "Usuário informado para acesso não está ativo no sistema";
                } else if (emite == 0)
                {
                    TempData["erro"] = "Usuário informado não tem permissão para emissão de NFC-e";
                }
            }
            if (chave > 0)
            {
                wksnomusu = nome; 
                wksemausu = email;
                Response.Redirect("/Home/Menu01", true);
            }
            return View("Index");
        }

        public IActionResult Recover(string email)
        {
            if (email != "" && email != null)
            {
                TempData["erro"] = "";
                int chave = 0; string senha = ""; string ativo = "";  string nome = "";  string completo = "";
                byte ret = DadosModel.Verifica_Email(email, ref chave, ref senha, ref ativo, ref nome, ref completo);
                if (chave == 0)
                {
                    TempData["erro"] = "E-Mail informado para reenvio de senha não existe no sistema";
                }
                if (chave >= 1)
                {
                    senha = Descriptografa(senha, true);
                    string erro = "";
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
                    byte sta = Envia_email("", email, "", "Solicitação de reenvio de senha TipSoft Soluções", texto, ref erro);
                    if (sta == 0)
                    {
                        TempData["erro"] = "Senha solicitada pelo usuário foi enviada com Sucesso !";
                    }
                    else
                    {
                        TempData["erro"] = "Erro no envio de e-mail solicitado com senha de acesso !";
                    }
                }
            }

            return View("Recover");
        }

        public IActionResult Menu01()
        {
            TempData["nom"] = wksnomusu;
            TempData["ema"] = wksemausu;
            if (wksnomusu == null || wksemausu == null)
            {
                Response.Redirect("/Home/Index", true);
            }
            return View("Menu01");
        }

        public IActionResult Man_Pagto(string ope, string cod)
        {
            string sql = ""; 
            ViewBag.ope_p = ope;
            ViewBag.cod_p = cod;
            TempData["nom"] = wksnomusu;
            TempData["ema"] = wksemausu;
            if (wksnomusu == null || wksemausu == null)
            {
                Response.Redirect("/Home/Index", true);
            }
            TempData["num_v"] = DadosModel.Ultimo_Pag();
            TempData["ope_p"] = ope; TempData["cod_p"] = cod;
            if (Request.Method == "GET") {
                if (ope != "1")
                {
                    string cha = "";
                    string des = "";
                    string obs = "";
                    byte ret = DadosModel.Ler_Pagto(cod, ref cha, ref des, ref obs);
                    ViewBag.cod_v = cha;
                    ViewBag.des_v = des;
                    ViewBag.obs_v = obs;
                }
            }
            if (Request.Method == "POST") {
                string sal = Request.Form["sal"];
                if (ope == "1")
                {
                    sql = "Insert into CadFormaPagto (";
                    sql += "codigo, ";
                    sql += "descricao, ";
                    sql += "observacoes ";
                    sql += ") values ( ";
                    sql += "'" + Request.Form["cod_v"] + "',";
                    sql += "'" + Request.Form["des_v"] + "',";
                    sql += "'" + Request.Form["obs_v"] + "')";
                    byte ret = DadosModel.ComandoSql(sql);
                }
                if (ope == "2")
                {
                    sql = "Update CadFormaPagto set ";
                    sql += "codigo = '" + Request.Form["cod_v"] + "',";
                    sql += "descricao = '" + Request.Form["des_v"] + "',";
                    sql += "observacoes = '" + Request.Form["obs_v"] + "' ";
                    sql += "where Pk = " + cod;
                    byte ret = DadosModel.ComandoSql(sql);
                }
                if (ope == "3")
                {
                    sql = "Delete from CadFormaPagto where Pk = " + cod;
                    byte ret = DadosModel.ComandoSql(sql);
                    if (ret == 99) {
                        TempData["erro"] = DadosModel.wkserroco;
                    }
                }
                ope = "1"; cod = "0";
            }
            TempData["dad"] = DadosModel.Lista_Pag();
            return View();
        }

        public IActionResult Con_Emitente(string ope, string cod)
        {
            ViewBag.ope_p = ope;
            ViewBag.cod_p = cod;
            TempData["erro"] = "";
            TempData["nom"] = wksnomusu;
            TempData["ema"] = wksemausu;
            if (wksnomusu == null || wksemausu == null)
            {
                Response.Redirect("/Home/Index", true);
            }
            TempData["dad"] = DadosModel.Lista_Emi();
            return View("Con_Emitente");
        }

        public IActionResult Man_Emitente(string ope, string cod)
        {
            string sql = "";
            TempData["erro"] = "";
            TempData["nom"] = wksnomusu;
            TempData["ema"] = wksemausu;
            TempData["est_l"] = DadosModel.Lista_Est();
            TempData["num_v"] = DadosModel.Ultimo_Emi();
            TempData["ope_p"] = ope; TempData["cod_p"] = cod;
            if (wksnomusu == null || wksemausu == null)
            {
                Response.Redirect("/Home/Index", true);
            }
            if (Directory.Exists(DadosModel.wkscamweb + "\\imagens\\") == false)
            {
                Directory.CreateDirectory(DadosModel.wkscamweb + "/imagens/");
            }
            if (Request.Method == "GET")
            {
                if (ope != "1")
                {
                    Dictionary<string, string> campos = new Dictionary<string, string>();
                    byte ret = DadosModel.Ler_Emitente(cod, ref campos);
                    TempData["raz"] = campos["raz"];
                    TempData["fan"] = campos["fan"];
                    TempData["cep"] = campos["cep"];
                    TempData["end"] = campos["end"];
                    TempData["nro"] = campos["nro"];
                    TempData["com"] = campos["com"];
                    TempData["bai"] = campos["bai"];
                    TempData["est"] = campos["est"];
                    TempData["cid"] = campos["cid"];
                    TempData["ati"] = campos["ati"];
                    TempData["tel"] = campos["tel"];
                    TempData["cel"] = campos["cel"];
                    TempData["ema"] = campos["ema"];
                    TempData["cnp"] = campos["cnp"];
                    TempData["ies"] = campos["ies"];
                    TempData["imu"] = campos["imu"];
                    TempData["obs"] = campos["obs"];
                    TempData["cid_l"] = DadosModel.Lista_Cid(campos["est"]);
                }
            }
            if (Request.Method == "POST")
            {
                string sal = Request.Form["sal"];
                if (ope == "1" && sal != null)
                {
                    sql = "Insert into CadEmitentes (";
                    sql += "FkCidades, ";
                    sql += "FkEstados, ";
                    sql += "FkPaises, ";
                    sql += "Tipo, ";
                    sql += "Inscricao, ";
                    sql += "InscricaoEstadual, ";
                    sql += "InscricaoMunicipal, ";
                    sql += "RazaoSocial, ";
                    sql += "NomeFantasia, ";
                    sql += "Endereco, ";
                    sql += "Numero, ";
                    sql += "Complemento, ";
                    sql += "Bairro, ";
                    sql += "Cep, ";
                    sql += "Telefone, ";
                    sql += "Celular, ";
                    sql += "Email, ";
                    sql += "Ativo, ";
                    sql += "ControlaEstoquePreco, ";
                    sql += "Observacoes ";
                    sql += ") values ( ";
                    sql += "'" + Request.Form["cid_v"] + "',";
                    sql += "'" + Request.Form["est_v"] + "',";
                    sql += "'" + "256" + "',";
                    sql += "'" + "CNPJ" + "',";
                    sql += "'" + Somente_Nro(Request.Form["cnp_v"]) + "',";
                    sql += "'" + Somente_Nro(Request.Form["ies_v"]) + "',";
                    sql += "'" + Somente_Nro(Request.Form["imu_v"]) + "',";
                    sql += "'" + Request.Form["raz_v"] + "',";
                    sql += "'" + Request.Form["fan_v"] + "',";
                    sql += "'" + Request.Form["end_v"] + "',";
                    sql += "'" + Somente_Nro(Request.Form["nro_v"]) + "',";
                    sql += "'" + Request.Form["com_v"] + "',";
                    sql += "'" + Request.Form["bai_v"] + "',";
                    sql += "'" + Somente_Nro(Request.Form["cep_v"]) + "',";
                    sql += "'" + Somente_Nro(Request.Form["tel_v"]) + "',";
                    sql += "'" + Somente_Nro(Request.Form["cel_v"]) + "',";
                    sql += "'" + Request.Form["ema_v"] + "',";
                    sql += "'" + Request.Form["ati_v"] + "',";
                    sql += "'" + "Não" + "',";
                    sql += "'" + Request.Form["obs_v"] + "')";
                    byte ret = DadosModel.ComandoSql(sql);
                    if (ret == 99)
                    {
                        TempData["erro"] = DadosModel.wkserroco;
                    }
                    if (Request.Form.Files.Count > 0)
                    {
                        if (Request.Form.Files["arq-log"] != null) {
                            long tam = Request.Form.Files["arq-log"].Length;
                            string[] ext = Request.Form.Files["arq-log"].FileName.Split(".");
                            string nom = Request.Form.Files["arq-log"].FileName;
                            string cnpj = Somente_Nro(Request.Form["cnp_v"]).ToString();
                            WebClient myWebClient = new WebClient();
                            string cam = DadosModel.wkscamweb + "/imagens/" + "Log_" + cnpj + "." + ext[1];
                            using (var fileStream = new FileStream(cam, FileMode.Create, FileAccess.Write))
                            {
                                Request.Form.Files["arq-log"].CopyTo(fileStream);
                            }
                        }
                        if (Request.Form.Files["arq-cer"] != null)
                        {
                            long tam = Request.Form.Files["arq-cer"].Length;
                            string[] ext = Request.Form.Files["arq-cer"].FileName.Split(".");
                            string nom = Request.Form.Files["arq-cer"].FileName;
                            string cnpj = Somente_Nro(Request.Form["cnp_v"]).ToString();
                            WebClient myWebClient = new WebClient();
                            string cam = DadosModel.wkscamweb + "/imagens/" + "Cer_" + cnpj + "." + ext[1];
                            using (var fileStream = new FileStream(cam, FileMode.Create, FileAccess.Write))
                            {
                                Request.Form.Files["arq-cer"].CopyTo(fileStream);
                            }
                        }
                    }
                    TempData["num_v"] = DadosModel.Ultimo_Emi();
                }
                if (ope == "2" && sal != null)
                {
                    sql = "Update CadEmitentes set ";
                    sql += "Fkcidades = '" + Request.Form["cid_v"] + "',";
                    sql += "Fkestados = '" + Request.Form["est_v"] + "',";
                    sql += "Inscricao = '" + Somente_Nro(Request.Form["cnp_v"]) + "',";
                    sql += "InscricaoEstadual = '" + Somente_Nro(Request.Form["ies_v"]) + "',";
                    sql += "InscricaoMunicipal = '" + Somente_Nro(Request.Form["imu_v"]) + "',";
                    sql += "RazaoSocial = '" + Request.Form["raz_v"] + "',";
                    sql += "NomeFantasia = '" + Request.Form["fan_v"] + "',";
                    sql += "Endereco = '" + Request.Form["end_v"] + "',";
                    sql += "Numero = '" + Somente_Nro(Request.Form["nro_v"]) + "',";
                    sql += "Complemento = '" + Request.Form["com_v"] + "',";
                    sql += "Bairro = '" + Request.Form["bai_v"] + "',";
                    sql += "Cep = '" + Somente_Nro(Request.Form["cep_v"]) + "',";
                    sql += "Telefone = '" + Somente_Nro(Request.Form["tel_v"]) + "',";
                    sql += "Celular = '" + Somente_Nro(Request.Form["cel_v"]) + "',";
                    sql += "Email = '" + Request.Form["ema_v"] + "',";
                    sql += "Ativo = '" + Request.Form["ati_v"] + "',";
                    sql += "Observacoes = '" + Request.Form["obs_v"] + "' ";
                    sql += "where Pk = " + cod;                    
                    byte ret = DadosModel.ComandoSql(sql);
                    if (ret == 99)
                    {
                        TempData["erro"] = DadosModel.wkserroco;
                    }
                    if (Request.Form.Files.Count > 0)
                    {
                        if (Request.Form.Files["arq-log"] != null)
                        {
                            long tam = Request.Form.Files["arq-log"].Length;
                            string[] ext = Request.Form.Files["arq-log"].FileName.Split(".");
                            string nom = Request.Form.Files["arq-log"].FileName;
                            string cnpj = Somente_Nro(Request.Form["cnp_v"]).ToString();
                            WebClient myWebClient = new WebClient();
                            string cam = DadosModel.wkscamweb + "/imagens/" + "Log_" + cnpj + "." + ext[1];
                            using (var fileStream = new FileStream(cam, FileMode.Create, FileAccess.Write))
                            {
                                Request.Form.Files["arq-log"].CopyTo(fileStream);
                            }
                        }
                        if (Request.Form.Files["arq-cer"] != null)
                        {
                            long tam = Request.Form.Files["arq-cer"].Length;
                            string[] ext = Request.Form.Files["arq-cer"].FileName.Split(".");
                            string nom = Request.Form.Files["arq-cer"].FileName;
                            string cnpj = Somente_Nro(Request.Form["cnp_v"]).ToString();
                            WebClient myWebClient = new WebClient();
                            string cam = DadosModel.wkscamweb + "/imagens/" + "Cer_" + cnpj + "." + ext[1];
                            using (var fileStream = new FileStream(cam, FileMode.Create, FileAccess.Write))
                            {
                                Request.Form.Files["arq-cer"].CopyTo(fileStream);
                            }
                        }
                    }
                }
                if (ope == "3" && sal != null)
                {
                    sql = "Delete from CadEmitentes where Pk = " + cod;
                    byte ret = DadosModel.ComandoSql(sql);
                    if (ret == 99)
                    {
                        TempData["erro"] = DadosModel.wkserroco;
                    }
                }
                ope = "1"; cod = "0"; TempData["ope_p"] = "1";
            }
            return View();
        }
        public IActionResult Con_Produto(string ope, string cod)
        {
            ViewBag.ope_p = ope;
            ViewBag.cod_p = cod;
            TempData["erro"] = "";
            TempData["nom"] = wksnomusu;
            TempData["ema"] = wksemausu;
            if (wksnomusu == null || wksemausu == null)
            {
                Response.Redirect("/Home/Index", true);
            }
            TempData["dad"] = DadosModel.Lista_Pro();
            return View("Con_Produto");
        }
        public IActionResult Man_Produto(string ope, string cod)
        {
            string sql = "";
            TempData["erro"] = "";
            TempData["nom"] = wksnomusu;
            TempData["ema"] = wksemausu;
            TempData["uni_l"] = DadosModel.Lista_Uni();
            TempData["sit_l"] = DadosModel.Lista_Cst();
            TempData["cfo_l"] = DadosModel.Lista_Cfo();
            TempData["ncm_l"] = DadosModel.Lista_Ncm();
            TempData["gru_l"] = DadosModel.Lista_Gru();
            TempData["num_v"] = DadosModel.Ultimo_Pro();
            TempData["ope_p"] = ope; TempData["cod_p"] = cod;
            if (wksnomusu == null || wksemausu == null)
            {
                Response.Redirect("/Home/Index", true);
            }
            if (Request.Method == "GET")
            {
                if (ope != "1")
                {
                    Dictionary<string, string> campos = new Dictionary<string, string>();
                    byte ret = DadosModel.Ler_Produto(cod, ref campos);
                    TempData["des"] = campos["des"];
                    TempData["cod"] = campos["cod"];
                    TempData["ean"] = campos["ean"];
                    TempData["ncm"] = campos["ncm"];
                    TempData["cfo"] = campos["cfo"];
                    TempData["uni"] = campos["uni"];
                    TempData["ati"] = campos["ati"];
                    TempData["pre"] = campos["pre"];
                    TempData["cst"] = campos["cst"];
                    TempData["gru"] = campos["gru"];
                    TempData["obs"] = campos["obs"];
                    TempData["uni_l"] = DadosModel.Lista_Uni();
                    TempData["sit_l"] = DadosModel.Lista_Cst();
                    TempData["cfo_l"] = DadosModel.Lista_Cfo();
                    TempData["ncm_l"] = DadosModel.Lista_Ncm();
                }
            }
            if (Request.Method == "POST")
            {
                string sal = Request.Form["sal"];
                if (ope == "1" && sal != null)
                {
                    sql = "Insert into CadProdutos (";
                    sql += "FkCadEmitentes, ";
                    sql += "FkCadUnidade, ";
                    sql += "FkCadNcm, ";
                    sql += "FkCadCfop, ";
                    sql += "FkCadCstIcms, ";
                    sql += "FkGruposCadProdutos, ";
                    sql += "Descricao, ";
                    sql += "Codigo, ";
                    sql += "CodigoEan, ";
                    sql += "Ativo, ";
                    sql += "VrVenda, ";
                    sql += "Observacoes ";
                    sql += ") values ( ";
                    sql += "'" + "1" + "',";
                    sql += "'" + Request.Form["uni_v"] + "',";
                    sql += "'" + Request.Form["ncm_v"] + "',";
                    sql += "'" + Request.Form["cfo_v"] + "',";
                    sql += "'" + Request.Form["cst_v"] + "',";
                    sql += "'" + Request.Form["gru_v"] + "',";
                    sql += "'" + Request.Form["des_v"] + "',";
                    sql += "'" + Request.Form["cod_v"] + "',";
                    sql += "'" + Request.Form["ean_v"] + "',";
                    sql += "'" + Request.Form["ati_v"] + "',";
                    sql += "'" + Request.Form["pre_v"] + "',";
                    sql += "'" + Request.Form["obs_v"] + "')";
                    byte ret = DadosModel.ComandoSql(sql);
                    TempData["num_v"] = DadosModel.Ultimo_Pro();
                    if (ret == 99)
                    {
                        TempData["erro"] = DadosModel.wkserroco;
                    }
                }
                if (ope == "2" && sal != null)
                {
                    sql = "Update CadProdutos set ";
                    sql += "FkCadNcm = '" + Request.Form["ncm_v"] + "',";
                    sql += "FkCadCfop = '" + Request.Form["cfo_v"] + "',";
                    sql += "FkCadUnidade = '" + Request.Form["uni_v"] + "',";
                    sql += "FkCadCstIcms = '" + Request.Form["cst_v"] + "',";
                    sql += "FkGruposCadProdutos = '" + Request.Form["gru_v"] + "',";
                    sql += "Descricao = '" + Request.Form["des_v"] + "',";
                    sql += "Codigo= '" + Request.Form["cod_v"] + "',";
                    sql += "CodigoEan = '" + Request.Form["ean_v"] + "',";
                    sql += "Ativo = '" + Request.Form["ati_v"] + "',";
                    sql += "VrVenda = '" + Request.Form["pre_v"] + "',";
                    sql += "Observacoes = '" + Request.Form["obs_v"] + "' ";
                    sql += "where Pk = " + cod;
                    byte ret = DadosModel.ComandoSql(sql);
                    if (ret == 99)
                    {
                        TempData["erro"] = DadosModel.wkserroco;
                    }
                }
                if (ope == "3" && sal != null)
                {
                    sql = "Delete from CadProdutos where Pk = " + cod;
                    byte ret = DadosModel.ComandoSql(sql);
                    if (ret == 99)
                    {
                        TempData["erro"] = DadosModel.wkserroco;
                    }
                }
                ope = "1"; cod = "0"; TempData["ope_p"] = "1";
            }
            return View();

        }
        public IActionResult Con_Cliente(string ope, string cod)
        {
            ViewBag.ope_p = ope;
            ViewBag.cod_p = cod;
            TempData["erro"] = "";
            TempData["nom"] = wksnomusu;
            TempData["ema"] = wksemausu;
            if (wksnomusu == null || wksemausu == null)
            {
                Response.Redirect("/Home/Index", true);
            }
            TempData["dad"] = DadosModel.Lista_Cli();
            return View("Con_Cliente");
        }
        public IActionResult Man_Cliente(string ope, string cod)
        {
            string sql = "";
            TempData["erro"] = "";
            TempData["nom"] = wksnomusu;
            TempData["ema"] = wksemausu;
            TempData["est_l"] = DadosModel.Lista_Est();
            TempData["num_v"] = DadosModel.Ultimo_Cli();
            TempData["ope_p"] = ope; TempData["cod_p"] = cod;
            if (wksnomusu == null || wksemausu == null)
            {
                Response.Redirect("/Home/Index", true);
            }
            if (Request.Method == "GET")
            {
                if (ope != "1")
                {
                    Dictionary<string, string> campos = new Dictionary<string, string>();
                    byte ret = DadosModel.Ler_Cliente(cod, ref campos);
                    TempData["raz"] = campos["raz"];
                    TempData["fan"] = campos["fan"];
                    TempData["cep"] = campos["cep"];
                    TempData["end"] = campos["end"];
                    TempData["nro"] = campos["nro"];
                    TempData["com"] = campos["com"];
                    TempData["bai"] = campos["bai"];
                    TempData["est"] = campos["est"];
                    TempData["cid"] = campos["cid"];
                    TempData["ati"] = campos["ati"];
                    TempData["tel"] = campos["tel"];
                    TempData["cel"] = campos["cel"];
                    TempData["ema"] = campos["ema"];
                    TempData["cnp"] = campos["cnp"];
                    TempData["ies"] = campos["ies"];
                    TempData["imu"] = campos["imu"];
                    TempData["tip"] = campos["tip"];
                    TempData["con"] = campos["con"];
                    TempData["obs"] = campos["obs"];
                    TempData["cid_l"] = DadosModel.Lista_Cid(campos["est"]);
                }
            }
            if (Request.Method == "POST")
            {
                string sal = Request.Form["sal"];
                if (ope == "1" && sal != null)
                {
                    sql = "Insert into CadClientesFornecedores (";
                    sql += "FkCidades, ";
                    sql += "FkEstados, ";
                    sql += "FkPaises, ";
                    sql += "Tipo, ";
                    sql += "Inscricao, ";
                    sql += "InscricaoEstadual, ";
                    sql += "InscricaoMunicipal, ";
                    sql += "RazaoSocial, ";
                    sql += "NomeFantasia, ";
                    sql += "Endereco, ";
                    sql += "Numero, ";
                    sql += "Complemento, ";
                    sql += "Bairro, ";
                    sql += "Cep, ";
                    sql += "Telefone, ";
                    sql += "Celular, ";
                    sql += "Email, ";
                    sql += "Ativo, ";
                    sql += "Contato, ";
                    sql += "Observacoes ";
                    sql += ") values ( ";
                    sql += "'" + Request.Form["cid_v"] + "',";
                    sql += "'" + Request.Form["est_v"] + "',";
                    sql += "'" + "256" + "',";
                    sql += "'" + Request.Form["tip_v"] + "',";
                    sql += "'" + Somente_Nro(Request.Form["cnp_v"]) + "',";
                    sql += "'" + Somente_Nro(Request.Form["ies_v"]) + "',";
                    sql += "'" + Somente_Nro(Request.Form["imu_v"]) + "',";
                    sql += "'" + Request.Form["raz_v"] + "',";
                    sql += "'" + Request.Form["fan_v"] + "',";
                    sql += "'" + Request.Form["end_v"] + "',";
                    sql += "'" + Somente_Nro(Request.Form["nro_v"]) + "',";
                    sql += "'" + Request.Form["com_v"] + "',";
                    sql += "'" + Request.Form["bai_v"] + "',";
                    sql += "'" + Somente_Nro(Request.Form["cep_v"]) + "',";
                    sql += "'" + Somente_Nro(Request.Form["tel_v"]) + "',";
                    sql += "'" + Somente_Nro(Request.Form["cel_v"]) + "',";
                    sql += "'" + Request.Form["ema_v"] + "',";
                    sql += "'" + Request.Form["ati_v"] + "',";
                    sql += "'" + Request.Form["con_v"] + "',";
                    sql += "'" + Request.Form["obs_v"] + "')";
                    byte ret = DadosModel.ComandoSql(sql);
                    if (ret == 99)
                    {
                        TempData["erro"] = DadosModel.wkserroco;
                    }
                    TempData["num_v"] = DadosModel.Ultimo_Cli();
                }
                if (ope == "2" && sal != null)
                {
                    sql = "Update CadClientesFornecedores set ";
                    sql += "Fkcidades = '" + Request.Form["cid_v"] + "',";
                    sql += "Fkestados = '" + Request.Form["est_v"] + "',";
                    sql += "Tipo = '" + Request.Form["tip_v"] + "',";
                    sql += "Inscricao = '" + Somente_Nro(Request.Form["cnp_v"]) + "',";
                    sql += "InscricaoEstadual = '" + Somente_Nro(Request.Form["ies_v"]) + "',";
                    sql += "InscricaoMunicipal = '" + Somente_Nro(Request.Form["imu_v"]) + "',";
                    sql += "RazaoSocial = '" + Request.Form["raz_v"] + "',";
                    sql += "NomeFantasia = '" + Request.Form["fan_v"] + "',";
                    sql += "Endereco = '" + Request.Form["end_v"] + "',";
                    sql += "Numero = '" + Somente_Nro(Request.Form["nro_v"]) + "',";
                    sql += "Complemento = '" + Request.Form["com_v"] + "',";
                    sql += "Bairro = '" + Request.Form["bai_v"] + "',";
                    sql += "Cep = '" + Somente_Nro(Request.Form["cep_v"]) + "',";
                    sql += "Telefone = '" + Somente_Nro(Request.Form["tel_v"]) + "',";
                    sql += "Celular = '" + Somente_Nro(Request.Form["cel_v"]) + "',";
                    sql += "Email = '" + Request.Form["ema_v"] + "',";
                    sql += "Ativo = '" + Request.Form["ati_v"] + "',";
                    sql += "Contato = '" + Request.Form["con_v"] + "',";
                    sql += "Observacoes = '" + Request.Form["obs_v"] + "' ";
                    sql += "where Pk = " + cod;
                    byte ret = DadosModel.ComandoSql(sql);
                    if (ret == 99)
                    {
                        TempData["erro"] = DadosModel.wkserroco;
                    }
                }
                if (ope == "3" && sal != null)
                {
                    sql = "Delete from CadClientesFornecedores where Pk = " + cod;
                    byte ret = DadosModel.ComandoSql(sql);
                    if (ret == 99)
                    {
                        TempData["erro"] = DadosModel.wkserroco;
                    }
                }
                ope = "1"; cod = "0"; TempData["ope_p"] = "1";
            }
            return View("Man_Cliente");
        }
        public IActionResult Con_Usuario(string ope, string cod)
        {
            ViewBag.ope_p = ope;
            ViewBag.cod_p = cod;
            TempData["erro"] = "";
            TempData["nom"] = wksnomusu;
            TempData["ema"] = wksemausu;
            if (wksnomusu == null || wksemausu == null)
            {
                Response.Redirect("/Home/Index", true);
            }
            TempData["dad"] = DadosModel.Lista_Usu();
            return View("Con_Usuario");
        }
        public IActionResult Man_Usuario(string ope, string cod)
        {
            string sql = "";
            TempData["erro"] = "";
            TempData["nom"] = wksnomusu;
            TempData["ema"] = wksemausu;
            TempData["emi_l"] = DadosModel.Nomes_Emi();
            TempData["num_v"] = DadosModel.Ultimo_Usu();
            TempData["ope_p"] = ope; TempData["cod_p"] = cod;
            if (wksnomusu == null || wksemausu == null)
            {
                Response.Redirect("/Home/Index", true);
            }
            if (Request.Method == "GET")
            {
                if (ope != "1")
                {
                    Dictionary<string, string> campos = new Dictionary<string, string>();
                    byte ret = DadosModel.Ler_Usuario(cod, ref campos);
                    TempData["nme"] = campos["nme"];
                    TempData["com"] = campos["com"];
                    TempData["ati"] = campos["ati"];
                    TempData["obs"] = campos["obs"];
                    TempData["adm"] = campos["adm"];
                    TempData["nfc"] = campos["nfc"];
                    TempData["cai"] = campos["cai"];
                    TempData["end"] = campos["end"];
                    TempData["emi"] = campos["emi"];
                    TempData["ete"] = campos["ete"];
                    TempData["cha"] = campos["cha"];
                    TempData["sen"] = Descriptografa(campos["sen"], true);
                    TempData["pas"] = Descriptografa(campos["pas"], true);
                }
            }
            if (Request.Method == "POST")
            {
                string sal = Request.Form["sal"];
                if (ope == "1" && sal != null)
                {
                    sql = "Insert into CadUsuarios (";
                    sql += "Nome, ";
                    sql += "NomeCompleto, ";
                    sql += "Ativo, ";
                    sql += "Senha, ";
                    sql += "SenhaConfirmacao, ";
                    sql += "Administrador, ";
                    sql += "NFCe, ";
                    sql += "Caixa, ";
                    sql += "AbaEmitente, ";
                    sql += "Email, ";
                    sql += "Estoque, ";
                    sql += "Entradas, ";
                    sql += "Venda, ";
                    sql += "Financeiro, ";
                    sql += "MDFe, ";
                    sql += "CTeOs, ";
                    sql += "CTe, ";
                    sql += "NFe, ";
                    sql += "VerificaPendencias, ";
                    sql += "Observacoes ";
                    sql += ") values ( ";
                    sql += "'" + Request.Form["nom_v"] + "',";
                    sql += "'" + Request.Form["com_v"] + "',";
                    sql += "'" + Request.Form["ati_v"] + "',";
                    sql += "'" + Criptografa(Request.Form["sen_v"], true) + "',";
                    sql += "'" + Criptografa(Request.Form["pas_v"], true) + "',";
                    sql += "'" + Request.Form["adm_v"] + "',";
                    sql += "'" + Request.Form["nfc_v"] + "',";
                    sql += "'" + (Request.Form["cai_v"].ToString() == "" ? "0" : Request.Form["cai_v"].ToString()) + "',";
                    sql += "'" + Request.Form["emi_v"] + "',";
                    sql += "'" + Request.Form["ema_v"] + "',";
                    sql += "'" + "0" + "',";
                    sql += "'" + "0" + "',";
                    sql += "'" + "0" + "',";
                    sql += "'" + "0" + "',";
                    sql += "'" + "0" + "',";
                    sql += "'" + "0" + "',";
                    sql += "'" + "0" + "',";
                    sql += "'" + "0" + "',";
                    sql += "'" + "0" + "',";
                    sql += "'" + Request.Form["obs_v"] + "')";
                    byte ret = DadosModel.ComandoSql(sql);
                    TempData["num_v"] = DadosModel.Ultimo_Pro();
                    if (ret == 99)
                    {
                        TempData["erro"] = DadosModel.wkserroco;
                    }
                    if (Request.Form["ete_v"] != "0" && Request.Form["ete_v"] != "**") {
                        sql = "Insert into CadUsuariosEmitentes (";
                        sql += "FkCadUsuarios, ";
                        sql += "FkCadEmitentes ";
                        sql += ") values ( ";
                        sql += "'" + Request.Form["num_w"] + "',";
                        sql += "'" + Request.Form["ete_v"] + "')";
                        ret = DadosModel.ComandoSql(sql);
                    }
                }
                if (ope == "2" && sal != null)
                {
                    sql = "Update CadUsuarios set ";
                    sql += "Nome = '" + Request.Form["nom_v"] + "',";
                    sql += "NomeCompleto = '" + Request.Form["com_v"] + "',";
                    sql += "Ativo = '" + Request.Form["ati_v"] + "',";
                    sql += "Senha = '" + Criptografa(Request.Form["sen_v"], true) + "',";
                    sql += "SenhaConfirmacao = '" + Criptografa(Request.Form["pas_v"], true) + "',";
                    sql += "Administrador = '" + Request.Form["adm_v"] + "',";
                    sql += "NFCe = '" + Request.Form["nfc_v"] + "',";
                    sql += "Caixa = '" + (Request.Form["cai_v"].ToString() == "" ? "0" : Request.Form["cai_v"].ToString()) + "',";
                    sql += "AbaEmitente = '" + Request.Form["emi_v"] + "',";
                    sql += "Email = '" + Request.Form["ema_v"] + "',";
                    sql += "Observacoes = '" + Request.Form["obs_v"] + "' ";
                    sql += "where Pk = " + cod;
                    byte ret = DadosModel.ComandoSql(sql);
                    if (ret == 99)
                    {
                        TempData["erro"] = DadosModel.wkserroco;
                    }
                }
                if (ope == "3" && sal != null)
                {
                    sql = "Delete from CadUsuarios where Pk = " + cod;
                    byte ret = DadosModel.ComandoSql(sql);
                    if (ret == 99)
                    {
                        TempData["erro"] = DadosModel.wkserroco;
                    }
                }
                ope = "1"; cod = "0"; TempData["ope_p"] = "1";
            }

            return View("Man_Usuario");
        }

        public IActionResult Pro_Saida(string ope, string cod)
        {
            TempData["nom"] = wksnomusu;
            TempData["ema"] = wksemausu;
            wksnomusu = null; wksemausu = null;
            return View("Pro_Saida");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static string Criptografa(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            // Get the key from config file
            // System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            // string key = (string)settingsReader.GetValue("TipSoft", typeof(String));

            string key = DadosModel.wkschatip;

            //System.Windows.Forms.MessageBox.Show(key);
            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        private static string Descriptografa(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            //Get your key from config file to open the lock!
            // System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            // string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));

            string key = DadosModel.wkschatip;

            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public static byte Envia_email(string origem, string destino, string copia, string assunto, string mensagem, ref string erro)
        {
            try
            {
                byte ret = 0; erro = "";
                string senha = "Profsa|1993";
                if (origem == "")
                {
                    origem = "Profsa Informática <suporte@profsa.com.br>";
                }
                if (assunto == "")
                {
                    assunto = "E-Mail enviado por Profsa Informática em " + DateTime.Now.ToLongDateString();
                }

                MailMessage envio = new MailMessage();
                envio.From = new MailAddress(origem);
                envio.To.Add(destino);
                envio.IsBodyHtml = true;
                envio.Subject = assunto;
                envio.Body = mensagem;
                if (copia != "")
                {
                    MailAddress outro = new MailAddress(copia);
                    envio.CC.Add(outro);
                }
                SmtpClient smtp = new SmtpClient("br108.hostgator.com.br"); // smtp.gmail.com
                smtp.Port = 587;    // 25 ou 465
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(origem, senha);
                smtp.Send(envio);

                return ret;
            }
            catch (Exception ex)
            {
                byte sta = 1;
                erro = ex.Message;
                return sta;
            }
        }

        public static string Somente_Nro(string campo)
        {
            string texto = ""; 
            if (campo == "" || campo == null ) { return "0"; }
            if (campo.ToUpper()  == "ISENTO" || campo.ToUpper()  == "ISENTA") { return campo; }
            foreach (char letra in campo)
            {
                if (Char.IsNumber(letra) == true)
                {
                    texto += letra.ToString();
                }
            }
            return texto;
        }
    }
}
