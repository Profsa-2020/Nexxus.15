using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Nexxus15
{
    public class Dados
    {
        public static string conecta = "";

        public static byte Valida_login(string email, string senha, ref int chave, ref string nome, ref string completo, ref string ativo, ref int admin, ref int emite, ref int caixa)
        {
            byte sta = 9; 
            SqlConnection coneccao = new SqlConnection(conecta);
            coneccao.Open();

            senha = Funcoes.Criptografa(senha, true);
            SqlCommand comando = new SqlCommand("Select * from CadUsuarios where email = '" + email + "' and senha = '" + senha + "'", coneccao);

            SqlDataReader registro = comando.ExecuteReader();
            if (registro.HasRows == true)
            {
                while (registro.Read())
                {
                    sta = 0;
                    chave = Convert.ToInt32(registro["PK"]);
                    nome = Convert.ToString(registro["nome"]);
                    completo = Convert.ToString(registro["nomecompleto"]);
                    ativo = Convert.ToString(registro["ativo"]);
                    admin = Convert.ToInt32(registro["administrador"]);
                    emite = Convert.ToInt32(registro["NFCe"]);
                    caixa = registro["caixa"] is DBNull ? 0 : Convert.ToInt32(registro["caixa"]);
                }
            }
            return sta;
        }

        public static byte Verifica_email(string email, ref string nome, ref string senha, ref string ativo, ref string completo)
        {
            byte sta = 1;
            nome = "";
            senha = "";
            completo = "";
            ativo = "";
            SqlConnection coneccao = new SqlConnection(conecta);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select Pk, nome, nomecompleto, senha, ativo from CadUsuarios where email = '" + email + "'", coneccao);
            SqlDataReader registro = comando.ExecuteReader();
            if (registro.HasRows == true)
            {
                while (registro.Read())
                {
                    sta = 0;
                    ativo = Convert.ToString(registro["ativo"]);
                    senha = Convert.ToString(registro["senha"]);
                    nome = Convert.ToString(registro["nome"]);
                    completo = Convert.ToString(registro["nomecompleto"]);
                }
            }
            return sta;
        }
        public static DataTable Lista_Emi()
        {
            SqlConnection coneccao = new SqlConnection(conecta);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select E.*, C.nome as Cidade, X.sigla as Estado from ((CadEmitentes E left join CadCidades C on E.FkCidades = C.Pk) left join CadEstados X on E.FkEstados = X.Pk) order by RazaoSocial, PK", coneccao);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = comando;
            DataTable tabela = new DataTable();
            adapter.Fill(tabela);
            return tabela;
        }
        public static int Ultimo_Pag()
        {
            int cod = 1;
            SqlConnection coneccao = new SqlConnection(conecta);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select Top 1 Pk from CadFormaPagto order by Pk desc" , coneccao);
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
            SqlConnection coneccao = new SqlConnection(conecta);
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
            SqlConnection coneccao = new SqlConnection(conecta);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select * from CadFormaPagto where Pk = " + num, coneccao);
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
        public static byte ComandoSql(string Sql)
        {
            try
            {
                byte sta = 0;
                SqlConnection coneccao = new SqlConnection(conecta);
                coneccao.Open();
                SqlCommand comando = new SqlCommand(Sql);
                comando.Connection = coneccao;
                comando.ExecuteNonQuery();
                coneccao.Close();
                coneccao.Dispose();
                return sta;
            }catch(Exception erro) {
                Funcoes.wkserroco = erro.Message;
                return 99;
            }
        }
        public static int Ultimo_Emi()
        {
            int cod = 1;
            SqlConnection coneccao = new SqlConnection(conecta);
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
        public static byte Ler_Emitente(string num, ref string cod, ref string des, ref string obs)
        {
            byte sta = 0;
            SqlConnection coneccao = new SqlConnection(conecta);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select * from CadEmitentes where Pk = " + num, coneccao);
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
        public static DataTable Lista_Est()
        {
            SqlConnection coneccao = new SqlConnection(conecta);
            coneccao.Open();
            SqlCommand comando = new SqlCommand("Select * from CadEstados order by sigla", coneccao);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = comando;
            DataTable tabela = new DataTable();
            adapter.Fill(tabela);
            return tabela;
        }
        public static DataTable Lista_Cid(int estado)
        {
            SqlConnection coneccao = new SqlConnection(conecta);
            coneccao.Open();    // Fkestados é o número não a sigla
            SqlCommand comando = new SqlCommand("Select * from CadCidades where Fkestados = " + estado + " order by nome", coneccao);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = comando;
            DataTable tabela = new DataTable();
            adapter.Fill(tabela);
            return tabela;
        }

    }
}
