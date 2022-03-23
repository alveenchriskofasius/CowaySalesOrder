using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Collections;

namespace Helper
{
    public static class DBHelper
    {
        private static string defaultConnectionString = "Data Source=ALVEEN\\XCLAVEASTRO;Initial Catalog=CSO;Integrated Security=SSPI";
        private const int TIMEOUT = 60;

        public static string DefaultConnectionString { get; set; } = defaultConnectionString;

        public static string ServerName
        {
            set
            {
                int startIndex;
                int endIndex;
                startIndex = DefaultConnectionString.IndexOf("=");
                endIndex = DefaultConnectionString.IndexOf(";");

                DefaultConnectionString = DefaultConnectionString.Substring(0, startIndex + 1) + value + DefaultConnectionString.Substring(endIndex);
            }
        }



        public static DataSet ExecuteProcedure(string PROC_NAME, params object[] parameters)
        {
            if (parameters.Length % 2 != 0)
                throw new ArgumentException("Wrong number of parameters sent to procedure. Expected an even number.");
            DataSet a = new DataSet();
            ListDictionary filters;
            filters = SetParameters(parameters);
            a = Query(PROC_NAME, filters);
            return a;
        }
        public static DataSet Execute(String procedureName, ListDictionary parameters)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlConnection connection = new SqlConnection(DefaultConnectionString);
                SqlCommand command = new SqlCommand();
                SqlDataAdapter da;
                try
                {
                    command.Connection = connection;
                    command.CommandTimeout = TIMEOUT;
                    command.CommandText = procedureName;
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (DictionaryEntry prm in parameters)
                            command.Parameters.Add(new SqlParameter(prm.Key.ToString(), prm.Value));
                    }

                    da = new SqlDataAdapter(command);
                    da.Fill(ds);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();
                }
                return ds;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        #region Private Methods

        private static ListDictionary SetParameters(params object[] parameters)
        {
            ListDictionary param = new ListDictionary();
            for (int i = 0; i < parameters.Length; i += 2)
            {
                if (parameters[i + 1] != null)
                    param.Add(parameters[i], parameters[i + 1]);
            }

            return param;
        }

        private static DataSet Query(String consulta, ListDictionary parameters)
        {
            DataSet ds = new DataSet();
            SqlConnection connection = new SqlConnection(DefaultConnectionString);
            SqlCommand command = new SqlCommand();
            SqlDataAdapter da;
            try
            {
                command.Connection = connection;
                command.CommandText = consulta;
                command.CommandType = CommandType.StoredProcedure;
                //if (parametros != null)
                //{
                //    command.Parameters.AddRange(parametros.ToArray());
                //}
                if (parameters != null)
                {
                    foreach (DictionaryEntry prm in parameters)
                        command.Parameters.Add(new SqlParameter(prm.Key.ToString(), prm.Value));
                }

                da = new SqlDataAdapter(command);
                da.Fill(ds);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
            return ds;
        }

        #endregion
    }

}