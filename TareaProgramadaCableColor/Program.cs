using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using TareaProgramadaCableColor.Conexion;

namespace TareaProgramadaCableColor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var listaPagos = ListapPagosCableColor();
                int pagosrealizados = 0;
                pagosrealizados = listaPagos.Count;



                if (pagosrealizados > 0)
                {
                    var monto = pagosrealizados * 10;
                    var contabilizar = hacerContabilidad("2614000414", "5299927000000000", "2110101030500000", "COBRO COMISION CABLE COLOR", "COBRO COMISION CABLE COLOR", monto.ToString(), "2", "0", "APLICATIVO", 0, 0, "LPS", "01");
                    if (contabilizar)
                    {
                        Enviar_Correo();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }


        public static List<AUDHI> ListapPagosCableColor()
        {
            AS400 con = new AS400();
            con.abrirConexion();
            var fecha = DateTime.Now.AddDays(-1);
            var dia = fecha.ToString("dd");
            var mes = fecha.ToString("MM");
            var año = fecha.ToString("yyyy");

            OleDbDataReader reader = con.consultar("SELECT * FROM BHCCYFILES.AUDHI WHERE AUDTCD=5711 AND AUDFLG='A' AND AUDDTM=" + mes + " AND AUDDTD=" + dia + " AND AUDDTY=" + año);
            List<AUDHI> listaDOCTIP = new List<AUDHI>();
            try
            {
                while (reader.Read())
                {
                    AUDHI a = new AUDHI();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        try
                        {
                            try
                            {
                                var celda = a.GetType().GetProperty(reader.GetName(i));
                                celda.SetValue(a, reader.GetValue(i));
                            }
                            catch (Exception)
                            {


                            }
                        }
                        catch (Exception)
                        {


                        }
                    }
                    listaDOCTIP.Add(a);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            finally
            {
                con.cerrarConexion();
            }
            return listaDOCTIP;
        }

        public static void Enviar_Correo()
        {
            System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
            string destinatario = "pagos@cablecolor.net,Tesoreria@cablecolor.net";
            string asunto = "Conciliación BANHCAFE";
            string contenido = "Hola, Buenos días. \n Se adjunta la concialición de pagos Cable Color al día "+DateTime.Now.ToString("dd/MM/yyyy")+".";

            //Destino de correo y su contenido
            correo.From = new System.Net.Mail.MailAddress("no-reply@banhcafe.bhc");
            correo.Subject = asunto;
            correo.To.Add(destinatario);
            correo.IsBodyHtml = true;
            correo.Body = contenido;
            correo.CC.Add("jaguilar@banhcafe.hn, emelgares@banhcafe.hn, cbarahona@banhcafe.hn");
            correo.Priority = System.Net.Mail.MailPriority.High;

            var adjunto = Program.Creartxt();
            var file = adjunto;
            
            var Data = new Attachment(file, MediaTypeNames.Application.Octet);
            var disposition = new ContentDisposition();
            disposition = Data.ContentDisposition;
            disposition.CreationDate = System.IO.File.GetCreationTime(file);
            disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
            disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
            correo.Attachments.Add(Data);

            var Servidor = new System.Net.Mail.SmtpClient();
            Servidor.Host = "correo.banhcafe.hn";
            Servidor.Credentials = new System.Net.NetworkCredential("no-reply@banhcafe.bhc", "BHCgen2017");
            try
            {
                Console.WriteLine("Enviando correo");
                Servidor.Send(correo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        public static string Creartxt()
        {
            IntegradorEntities integrador = new IntegradorEntities();
            string texto = string.Empty;
            string dir = Path.GetTempPath();
            string nombreArchivo = "ConciliacionCableColor" + DateTime.Now.ToString("ddMMyyyy") + ".txt";
            string fecha = DateTime.Now.ToString("yyyyMMdd");

            var consulta = integrador.cableColor.Where(x => x.fecha == fecha && x.identificador_unico_pago != null && x.identificador_unico_reversion == null).ToList();

            foreach(var item in consulta)
            {
                texto += item.codCliente + ":" + item.numReferencia + ":" + item.saldo + ":" + item.cajero.Trim() + ":" + item.codAgencia+"\n";
            }

            File.WriteAllText(dir+"/"+nombreArchivo,texto);

            return dir + "/" + nombreArchivo;

        }


        //Hacer contabilidad Directamente en el TRANS
        public static bool hacerContabilidad(string cuentaDebito, string cuentaCredito, string cuentaDetalle, string descripcionCredito, string descripcionDebito, string monto, string agencia, string referencia, string usuario, int id, int? env, string moneda, string banco)
        {
            AS400 con = new AS400();
            con.abrirConexion();
            var insert = 0;
            string descd = descripcionDebito;
            string descc = descripcionCredito;
            
            try
            {
                var i = DateTime.Now;
                var dia = i.ToString("dd");
                var mes = i.ToString("MM");
                var año = i.ToString("yyyy");




                var sql2 = "INSERT INTO BHCCYFILES.TRANS (TRABTH, TRAVDM, TRAVDD, TRAVDY, TRABNK, TRABRN, TRACCY, TRAGLN, TRAACC, TRABDM, TRABDD, TRABDY, TRACDE, TRANAR, TRAAMT, TRADCC, TRAUSA, TRAUSO, TRAEXR, TRADRR, TRAPTS, TRAAPC, TRAREF, TRANID, TRATRN, TRAPMN, TRASEQ, TRACKN, TRACCN, TRACUN, TRAMOD, TRAOBK, TRAOBR, TRAAAF, TRAEQV, TRAACR, TRACNU, TRARCL, TRACNL, TRADED, TRADSQ, TRAIVP, TRAIVB, TRAREV, TRAOLF, TRACOD, TRATMS)" +
                            " VALUES(8002, " + mes + ", " + dia + ", " + año + ", '" + banco + "', " + agencia + ", '" + moneda + "', " + cuentaCredito + ",0, " + mes + ", " + dia + ", " + año + ", 'MC', '" + descripcionCredito + "', " + monto + ", '5', '" + usuario + "', '" + usuario + "', 1.000000, 0, 0, '  ', '                         ', 0, 0, 0, 2, 0, 0, 336409, '1',  '" + banco + "', 1, ' ', 0.00, 0, 0, '                    ', '$1', '    ', 0, 0.000, 0.00, ' ', ' ', '    ', CURRENT_TIMESTAMP)";
                con.ejecutar(sql2);

                var sqlED = "INSERT INTO BHCCYFILES.TRANS (TRABTH, TRAVDM, TRAVDD, TRAVDY, TRABNK, TRABRN, TRACCY, TRAGLN, TRAACC, TRABDM, TRABDD, TRABDY, TRACDE, TRANAR, TRAAMT, TRADCC, TRAUSA, TRAUSO, TRAEXR, TRADRR, TRAPTS, TRAAPC, TRAREF, TRANID, TRATRN, TRAPMN, TRASEQ, TRACKN, TRACCN, TRACUN, TRAMOD, TRAOBK, TRAOBR, TRAAAF, TRAEQV, TRAACR, TRACNU, TRARCL, TRACNL, TRADED, TRADSQ, TRAIVP, TRAIVB, TRAREV, TRAOLF, TRACOD, TRATMS)" +
                                    " VALUES(8002, " + mes + ", " + dia + ", " + año + ", '" + banco + "', " + agencia + ", '" + moneda + "', " + cuentaDetalle + ", " + cuentaDebito + ", " + mes + ", " + dia + ", " + año + ", 'MD', '" + descripcionDebito + "', " + monto + ", '0', '" + usuario + "', '" + usuario + "', 1.000000, 0, 0, '  ', '                         ', 0, 0, 0, 2, 0, 0, 336409, '1',  '" + banco + "', 1, ' ', 0.00, 0, 0, '                    ', '$1', '    ', 0, 0.000, 0.00, ' ', ' ', '    ', CURRENT_TIMESTAMP)";
                con.ejecutar(sqlED);




            }

            catch (Exception ex)
            {

                throw;
            }


            if (insert == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



    }
}
