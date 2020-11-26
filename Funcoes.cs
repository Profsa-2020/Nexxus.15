using Microsoft.Extensions.Configuration;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Net.Mail;

namespace Nexxus15
{
    public class Funcoes
    {
        public static int wkschausu = 0;
        public static string wksnomusu = "";
        public static string wksnomcom = "";
        public static string wksemausu = "";
        public static string wkserroco = "";
        public static string wkschatip = "";

        public static string Descriptografa(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            System.Configuration.AppSettingsReader settingsReader =
                                                new AppSettingsReader();
            //Get your key from config file to open the lock!
            // string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));

            string key = Funcoes.wkschatip;

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

        public static string Criptografa(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            System.Configuration.AppSettingsReader settingsReader =
                                                new AppSettingsReader();
            // Get the key from config file

            // string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
            string key = Funcoes.wkschatip;

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

        public static byte Envia_email(string origem, string destino, string copia, string assunto, string mensagem)
        {
            try
            {
                byte ret = 0;
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
            catch(Exception ex)
            {
                byte sta = 1;
                string erro = ex.Message;
                return sta;
            }
        }

    }
}
