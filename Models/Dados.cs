using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Nexxus16.Models
{
    public class DadosModel
    {

        public static string wksstrsql = "";
        public static string wkschatip = "";
        public static string wkserroco = "";
        public static string wkscamfis = "";
        public static string wkscamweb = "";

        public static byte Valida_Login(string email, string senha, ref int chave, ref string nome, ref string ativo, ref byte emite)
        {
            byte ret = 0;
            chave = 0;
            nome = "";
            int caixa = 0;
            string conecta = wksstrsql;
            AppSettingsReader settingsReader = new AppSettingsReader();
            SqlConnection coneccao = new SqlConnection(conecta); coneccao.Open();
            SqlCommand comando = new SqlCommand("Select * from CadUsuarios where email = '" + email + "' and senha = '" + senha + "'", coneccao);
            SqlDataReader registro = comando.ExecuteReader();
            if (registro.HasRows == true)
            {
                while (registro.Read())
                {
                    ret = 1;
                    chave = Convert.ToInt32(registro["PK"]);
                    nome = Convert.ToString(registro["nome"]);
                    ativo = Convert.ToString(registro["ativo"]);
                    emite = Convert.ToByte(registro["NFCe"]);
                    caixa = registro["caixa"] is DBNull ? 0 : Convert.ToInt32(registro["caixa"]);
                }
            }

            return ret;
        }

        public static byte Verifica_Email(string email, ref int chave, ref string senha, ref string ativo, ref string nome, ref string completo)
        {
            chave = 0;
            byte ret = 0;
            string conecta = wksstrsql;
            AppSettingsReader settingsReader = new AppSettingsReader();
            SqlConnection coneccao = new SqlConnection(conecta); coneccao.Open();
            SqlCommand comando = new SqlCommand("Select * from CadUsuarios where email = '" + email + "'", coneccao);
            SqlDataReader registro = comando.ExecuteReader();
            if (registro.HasRows == true)
            {
                while (registro.Read())
                {
                    ret = 1;
                    chave = Convert.ToInt32(registro["PK"]);
                    nome = Convert.ToString(registro["nome"]);
                    senha = Convert.ToString(registro["senha"]);
                    ativo = Convert.ToString(registro["ativo"]);
                    completo = Convert.ToString(registro["nomecompleto"]);
                }
            }

            return ret;
        }

        public static int Ultimo_Pag()
        {
            int cod = 1;
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select Top 1 Pk from CadFormaPagto order by Pk desc", coneccao);
            SqlDataReader registro = comando.ExecuteReader();
            if (registro.HasRows == true)
            {
                while (registro.Read())
                {
                    cod = Convert.ToInt32(registro["Pk"]);
                }
            }
            return cod;
        }
        public static DataTable Lista_Pag()
        {
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select * from CadFormaPagto order by descricao, PK", coneccao);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = comando;
            DataTable tabela = new DataTable();
            adapter.Fill(tabela);
            return tabela;
        }
        public static byte Ler_Pagto(string num, ref string cod, ref string des, ref string obs)
        {
            byte sta = 0;
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select * from CadFormaPagto where Pk = " + (num == null ? "0" : num), coneccao);
            SqlDataReader registro = comando.ExecuteReader();
            if (registro.HasRows == true)
            {
                while (registro.Read())
                {
                    sta = 1;
                    cod = registro["codigo"].ToString();
                    des = registro["descricao"].ToString();
                    obs = registro["observacoes"].ToString();
                }
            }
            return sta;
        }
        public static DataTable Lista_Emi()
        {
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select E.*, C.nome as Cidade, X.sigla as Estado from ((CadEmitentes E left join CadCidades C on E.FkCidades = C.Pk) left join CadEstados X on E.FkEstados = X.Pk) order by RazaoSocial, PK", coneccao);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = comando;
            DataTable tabela = new DataTable();
            adapter.Fill(tabela);
            return tabela;
        }
        public static int Ultimo_Emi()
        {
            int cod = 1;
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select Top 1 Pk from CadEmitentes order by Pk desc", coneccao);
            SqlDataReader registro = comando.ExecuteReader();
            if (registro.HasRows == true)
            {
                while (registro.Read())
                {
                    cod = Convert.ToInt32(registro["Pk"]);
                }
            }
            return cod;
        }
        public static int Ultimo_Pro()
        {
            int cod = 1;
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select Top 1 Pk from CadProdutos order by Pk desc", coneccao);
            SqlDataReader registro = comando.ExecuteReader();
            if (registro.HasRows == true)
            {
                while (registro.Read())
                {
                    cod = Convert.ToInt32(registro["Pk"]);
                }
            }
            return cod;
        }
        public static DataTable Lista_Uni()
        {
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select * from CadUnidade order by descricao", coneccao);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = comando;
            DataTable tabela = new DataTable();
            adapter.Fill(tabela);
            return tabela;
        }
        public static DataTable Lista_Cst()
        {
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select * from CadCstIcms order by pk", coneccao);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = comando;
            DataTable tabela = new DataTable();
            adapter.Fill(tabela);
            return tabela;
        }
        public static DataTable Lista_Cfo()
        {
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select * from CadCfop order by codigo", coneccao);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = comando;
            DataTable tabela = new DataTable();
            adapter.Fill(tabela);
            return tabela;
        }
        public static DataTable Lista_Ncm()
        {
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select * from CadNcm order by codigo", coneccao);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = comando;
            DataTable tabela = new DataTable();
            adapter.Fill(tabela);
            return tabela;
        }
        public static DataTable Lista_Gru()
        {
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select * from GruposCadProdutos order by descricao", coneccao);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = comando;
            DataTable tabela = new DataTable();
            adapter.Fill(tabela);
            return tabela;
        }
        public static byte Ler_Emitente(string num, ref Dictionary<string, string> campos)
        {
            byte sta = 0;
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select * from CadEmitentes where Pk = " + (num == null ? "0" : num), coneccao);
            SqlDataReader registro = comando.ExecuteReader();
            if (registro.HasRows == true)
            {
                while (registro.Read())
                {
                    sta = 1;
                    campos.Add("est", registro["FkEstados"].ToString());
                    campos.Add("cid", registro["FkCidades"].ToString());
                    campos.Add("raz", registro["RazaoSocial"].ToString());
                    campos.Add("fan", registro["NomeFantasia"].ToString());
                    campos.Add("end", registro["Endereco"].ToString());
                    campos.Add("nro", registro["Numero"].ToString());
                    campos.Add("com", registro["Complemento"].ToString());
                    campos.Add("bai", registro["Bairro"].ToString());
                    campos.Add("cep", registro["Cep"].ToString());
                    campos.Add("tel", registro["Telefone"].ToString());
                    campos.Add("cel", registro["Celular"].ToString());
                    campos.Add("ema", registro["Email"].ToString());
                    campos.Add("ati", registro["Ativo"].ToString());
                    campos.Add("cnp", registro["Inscricao"].ToString());
                    campos.Add("ies", registro["InscricaoEstadual"].ToString());
                    campos.Add("imu", registro["InscricaoMunicipal"].ToString());
                    campos.Add("obs", registro["Observacoes"].ToString());
                }
            }
            return sta;
        }
        public static DataTable Lista_Est()
        {
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select * from CadEstados order by sigla", coneccao);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = comando;
            DataTable tabela = new DataTable();
            adapter.Fill(tabela);
            return tabela;
        }
        public static DataTable Lista_Cid(string estado)
        {
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();    // Fkestados é o número não a sigla
            SqlCommand comando = new SqlCommand("Select * from CadCidades where Fkestados = '" + estado + "' order by nome", coneccao);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = comando;
            DataTable tabela = new DataTable();
            adapter.Fill(tabela);
            return tabela;
        }
        public static DataTable Lista_Pro()
        {
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select P.*, U.descricao as desuni, U.unidade as coduni, N.codigo as codncm, N.descricao as desncm, G.descricao as desgru from (((CadProdutos P left join CadUnidade U on P.FkCadUnidade = U.Pk) left join CadNcm N on P.FkCadNcm = N.Pk) left join GruposCadProdutos G on P.FkGruposCadProdutos = G.Pk) order by P.Descricao, P.PK", coneccao);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = comando;
            DataTable tabela = new DataTable();
            adapter.Fill(tabela);
            return tabela;
        }
        public static DataTable Lista_Cli()
        {
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select C.*, X.nome as Cidade, Y.sigla as Estado from ((CadClientesFornecedores C left join CadCidades X on C.FkCidades = X.Pk) left join CadEstados Y on C.FkEstados = Y.Pk) order by RazaoSocial, PK", coneccao);            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = comando;
            DataTable tabela = new DataTable();
            adapter.Fill(tabela);
            return tabela;
        }
        public static byte Ler_Produto(string num, ref Dictionary<string, string> campos)
        {
            byte sta = 0;
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select * from CadProdutos where Pk = " + (num == null ? "0" : num), coneccao);
            SqlDataReader registro = comando.ExecuteReader();
            if (registro.HasRows == true)
            {
                while (registro.Read())
                {
                    sta = 1;
                    campos.Add("ncm", registro["FkCadNcm"].ToString());
                    campos.Add("cfo", registro["FkCadCfop"].ToString());
                    campos.Add("cst", registro["FkCadCstIcms"].ToString());
                    campos.Add("uni", registro["FkCadUnidade"].ToString());
                    campos.Add("des", registro["Descricao"].ToString());
                    campos.Add("cod", registro["Codigo"].ToString());
                    campos.Add("ean", registro["CodigoEan"].ToString());
                    campos.Add("ati", registro["Ativo"].ToString());
                    campos.Add("gru", registro["FkGruposCadProdutos"].ToString());
                    campos.Add("pre", registro["VrVenda"].ToString());
                    campos.Add("obs", registro["Observacoes"].ToString());
                }
            }
            return sta;
        }
        public static int Ultimo_Cli()
        {
            int cod = 1;
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select Top 1 Pk from CadClientesFornecedores order by Pk desc", coneccao);
            SqlDataReader registro = comando.ExecuteReader();
            if (registro.HasRows == true)
            {
                while (registro.Read())
                {
                    cod = Convert.ToInt32(registro["Pk"]);
                }
            }
            return cod;
        }
        public static byte Ler_Cliente(string num, ref Dictionary<string, string> campos)
        {
            byte sta = 0;
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select * from CadClientesFornecedores where Pk = " + (num == null ? "0" : num), coneccao);
            SqlDataReader registro = comando.ExecuteReader();
            if (registro.HasRows == true)
            {
                while (registro.Read())
                {
                    sta = 1;
                    campos.Add("est", registro["FkEstados"].ToString());
                    campos.Add("cid", registro["FkCidades"].ToString());
                    campos.Add("raz", registro["RazaoSocial"].ToString());
                    campos.Add("fan", registro["NomeFantasia"].ToString());
                    campos.Add("end", registro["Endereco"].ToString());
                    campos.Add("nro", registro["Numero"].ToString());
                    campos.Add("com", registro["Complemento"].ToString());
                    campos.Add("bai", registro["Bairro"].ToString());
                    campos.Add("cep", registro["Cep"].ToString());
                    campos.Add("tel", registro["Telefone"].ToString());
                    campos.Add("cel", registro["Celular"].ToString());
                    campos.Add("ema", registro["Email"].ToString());
                    campos.Add("ati", registro["Ativo"].ToString());
                    campos.Add("tip", registro["Tipo"].ToString());
                    campos.Add("con", registro["Contato"].ToString());
                    campos.Add("cnp", registro["Inscricao"].ToString());
                    campos.Add("ies", registro["InscricaoEstadual"].ToString());
                    campos.Add("imu", registro["InscricaoMunicipal"].ToString());
                    campos.Add("obs", registro["Observacoes"].ToString());
                }
            }
            return sta;
        }
        public static DataTable Lista_Usu()
        {
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select * from CadUsuarios order by Nome, PK", coneccao); SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = comando;
            DataTable tabela = new DataTable();
            adapter.Fill(tabela);
            return tabela;
        }
        public static int Ultimo_Usu()
        {
            int cod = 1;
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select Top 1 Pk from CadUsuarios order by Pk desc", coneccao);
            SqlDataReader registro = comando.ExecuteReader();
            if (registro.HasRows == true)
            {
                while (registro.Read())
                {
                    cod = Convert.ToInt32(registro["Pk"]);
                }
            }
            return cod;
        }
        public static byte Ler_Usuario(string num, ref Dictionary<string, string> campos)
        {
            byte sta = 0;
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select U.*, E.Pk as Chave, E.FkCadEmitentes from (CadUsuarios U left join CadUsuariosEmitentes E on U.Pk = E.FkCadUsuarios) where U.Pk = " + (num == null ? "0" : num), coneccao);
            SqlDataReader registro = comando.ExecuteReader();
            if (registro.HasRows == true)
            {
                while (registro.Read())
                {
                    sta = 1;
                    campos.Add("nme", registro["Nome"].ToString());
                    campos.Add("com", registro["NomeCompleto"].ToString());
                    campos.Add("ati", registro["Ativo"].ToString());
                    campos.Add("sen", registro["Senha"].ToString());
                    campos.Add("pas", registro["SenhaConfirmacao"].ToString());
                    campos.Add("obs", registro["Observacoes"].ToString());
                    campos.Add("adm", registro["Administrador"].ToString());
                    campos.Add("nfc", registro["NFCe"].ToString());
                    campos.Add("cai", registro["Caixa"].ToString());
                    campos.Add("emi", registro["AbaEmitente"].ToString());
                    campos.Add("end", registro["Email"].ToString());
                    campos.Add("cha", registro["Chave"].ToString());
                    campos.Add("ete", registro["FkCadEmitentes"].ToString());
                }
            }
            return sta;
        }
        public static DataTable Nomes_Emi()
        {
            SqlConnection coneccao = new SqlConnection(wksstrsql);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select * from CadEmitentes order by RazaoSocial", coneccao);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = comando;
            DataTable tabela = new DataTable();
            adapter.Fill(tabela);
            return tabela;
        }

        public static byte ComandoSql(string Sql)
        {
            try
            {
                byte sta = 0; wkserroco = "";
                SqlConnection coneccao = new SqlConnection(wksstrsql);
                coneccao.Open();
                SqlCommand comando = new SqlCommand(Sql);
                comando.Connection = coneccao;
                comando.ExecuteNonQuery();
                coneccao.Close();
                coneccao.Dispose();
                return sta;
            }
            catch (Exception erro)
            {
                wkserroco = erro.Message;
                return 99;
            }
        }

    }
}
