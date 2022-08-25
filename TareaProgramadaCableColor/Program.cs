using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TareaProgramadaCableColor.Conexion;

namespace TareaProgramadaCableColor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var listaPagos = ListapPagosCableColor();
            int pagosrealizados = 0;
            pagosrealizados=listaPagos.Count;

            var monto = pagosrealizados * 10;

          var contabilizar= hacerContabilidad("2614000414", "5299927000000000", "2110101030500000", "COBRO COMISION CABLE COLOR", "COBRO COMISION CABLE COLOR", monto.ToString(),"2","0","APLICATIVO",0,0,"LPS","01");

            Console.ReadLine();

        }


        public static List<AUDHI> ListapPagosCableColor()
        {
            AS400 con = new AS400();
            con.abrirConexion();
            OleDbDataReader reader = con.consultar("SELECT * FROM BHCCYFILES.AUDHI WHERE AUDTCD=5711 AND AUDFLG='A' AND AUDDTM=8");
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


        //Hacer contabilidad Directamente en el TRANS
        public static bool hacerContabilidad(string cuentaDebito, string cuentaCredito,string cuentaDetalle, string descripcionCredito, string descripcionDebito, string monto, string agencia, string referencia, string usuario, int id, int? env, string moneda, string banco)
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
