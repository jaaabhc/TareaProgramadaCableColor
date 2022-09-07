using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TareaProgramadaCableColor.Conexion
{
    public class AS400
    {

        String conexion = "Provider=IBMDA400.1;Data Source=172.19.20.60;User id=APLICATIVO;Password=fechabanap";
        OleDbConnection dbConnection;

        public void abrirConexion()
        {
            try
            {
                dbConnection = new OleDbConnection(conexion);
                dbConnection.Open();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public void cerrarConexion()
        {
            try
            {
                dbConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public OleDbDataReader consultar(string consulta)
        {
            try
            {
                OleDbCommand command = new OleDbCommand();
                command.CommandText = consulta;
                command.Connection = dbConnection;
                OleDbDataReader reader = command.ExecuteReader();
                return reader;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public OleDbDataReader consultar2(string consulta)
        {
            try
            {
                OleDbCommand command = new OleDbCommand();
                command.CommandText = consulta;
                command.Connection = dbConnection;
                OleDbDataReader reader = command.ExecuteReader();
                return reader;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public int ejecutar(string consulta)
        {
            try
            {
                OleDbCommand command = new OleDbCommand();
                command.CommandText = consulta;
                command.Connection = dbConnection;
                int reader = command.ExecuteNonQuery();

                return reader;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

    }
}
