
using Microsoft.Data.SqlClient;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommonServices.DataAccess
{
    public class DataAcces
    {


        private SqlConnection myConnection = new SqlConnection();
        private SqlDataAdapter oSqlAdap = new SqlDataAdapter();
        private DataSet Ds = new DataSet();
        SqlCommand oCommand = new SqlCommand();

        public SqlCommand OCommand { get => oCommand; set => oCommand = value; }

        public DataSet ExecuteDataSet(string str, string connectionstr)
        {
            DataSet functionReturnValue = null;
            myConnection = new SqlConnection(connectionstr);
            Ds = new DataSet();
            try
            {
                myConnection.Open();
                oSqlAdap = new SqlDataAdapter(str, myConnection);
                oSqlAdap.SelectCommand.CommandTimeout = 600;
                oSqlAdap.Fill(Ds, "T_Temp");
                functionReturnValue = Ds;
            }
            catch (Exception ex)
            {
                myConnection.Close();
                throw new ApplicationException("Exception in Run Query  " + ex.Message.ToString());
            }
            finally
            {
                myConnection.Close();
                myConnection = null;
                oSqlAdap = null;
            }
            return functionReturnValue;
        }
        public DataSet ExecuteDataSetWithParam(string str, string connectionstr, SqlParameter[] parameters)
        {
            DataSet functionReturnValue = null;
            myConnection = new SqlConnection(connectionstr);
            Ds = new DataSet();
            try
            {
                myConnection.Open();
                oSqlAdap = new SqlDataAdapter(str, myConnection);
                if (parameters != null)
                {
                    oSqlAdap.SelectCommand.Parameters.AddRange(parameters);
                }
                oSqlAdap.Fill(Ds, "T_Temp");
                functionReturnValue = Ds;
            }
            catch (Exception ex)
            {
                myConnection.Close();
                throw new ApplicationException("Exception in Run Query " + ex.Message.ToString());
            }
            finally
            {
                myConnection.Close();
                myConnection = null;
                oSqlAdap = null;
            }
            return functionReturnValue;
        }
        public static object IfNull(object obj)
        {
            if (obj == null)
                return "0";
            else if (obj == null)
                return "0";
            else if (obj.ToString() == "")
                return "0";
            else
                return obj;
        }
        public DataTable StreamReaderToDataTable(StreamReader reader)
        {
            DataTable dt = new DataTable();
            string? line;
            bool isHeader = true;
            int expectedColumnCount = 0;

            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var values = ParseCsvLine(line);

                if (isHeader)
                {
                    foreach (var header in values)
                        dt.Columns.Add(header.Trim());
                    expectedColumnCount = dt.Columns.Count;
                    isHeader = false;
                }
                else
                {
                    // Handle uneven rows
                    var row = dt.NewRow();
                    for (int j = 0; j < expectedColumnCount && j < values.Count; j++)
                    {
                        row[j] = values[j].Trim();
                    }
                    dt.Rows.Add(row);
                }
            }

            return dt;
        }
        private List<string> ParseCsvLine(string line)
        {
            List<string> fields = new List<string>();
            bool inQuotes = false;
            string field = "";

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '\"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == '^' && !inQuotes)
                {
                    fields.Add(field);
                    field = "";
                }
                else
                {
                    field += c;
                }
            }

            fields.Add(field); // Add the last field
            return fields;
        }
        public DataTable StreamToDataTable(string csvContent)
        {
            var dt = new DataTable();
            var lines = csvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length == 0)
                return dt;

            var headers = lines[0].Split(',');
            foreach (var header in headers)
                dt.Columns.Add(header.Trim());

            for (int i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(',');
                dt.Rows.Add(values);
            }

            return dt;
        }

        public static object IfStrNull(object obj)
        {
            if (obj == null)
                return "";
            else if (obj == null)
                return "";
            else if (obj.ToString() == "")
                return "";
            else
                return obj;
        }

        public DataTable ExecuteDataTable(string strQuery, string strconnection)
        {

            myConnection = new SqlConnection(strconnection);
            DataTable _retVal = null;
            DataSet oDataSet = new DataSet();
            try
            {
                myConnection.Open();

                if (myConnection.State == ConnectionState.Open)
                {
                    oCommand.Connection = myConnection;
                    oCommand.CommandText = strQuery;
                    oCommand.CommandType = CommandType.Text;
                    oSqlAdap = new SqlDataAdapter(oCommand);
                    if (oSqlAdap != null)
                        oSqlAdap.Fill(oDataSet);
                }
                else
                {
                    throw new Exception("");
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                myConnection.Close();
                SqlCommand oCommand = null;
                oSqlAdap = null;
            }
            return _retVal = oDataSet.Tables[0];

        }
        public int ExecuteNonQuery(string str, string strconn, string strIdentity = "")
        {
            int _retVal = 0;

            SqlConnection myConnection = new SqlConnection(strconn);
            try
            {
                myConnection.Open();
                oCommand = new SqlCommand(str, myConnection);
                _retVal = oCommand.ExecuteNonQuery();
                if (_retVal > 0)
                {
                    if (strIdentity.Length > 0)
                    {
                        oCommand = new SqlCommand(strIdentity, myConnection);
                        _retVal = Convert.ToInt32(oCommand.ExecuteScalar());
                    }
                }

            }
            catch (Exception ex)
            {
                myConnection.Close();
                throw new ApplicationException("Exception in Run Query  " + ex.Message.ToString());
            }
            finally
            {
                myConnection.Close();
                myConnection = null;
                oCommand = null;
            }
            return _retVal;
        }
        public object ConvertDataTabletoString(DataTable dt)
        {
            try
            {


                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;
                foreach (DataRow dr in dt.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);
                }
                string abd = JsonConvert.SerializeObject(rows);
                return abd;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }


        }
        public List<T> ConvertToList<T>(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name.ToLower()))
                    {
                        try
                        {
                            pro.SetValue(objT, row[pro.Name]);
                        }
                        catch (Exception ex) { }
                    }
                }
                return objT;
            }).ToList();
        }
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType;
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        public string DataTableToJSONWithJSONNet(DataTable table)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);

            return JSONString;
        }
        public DataSet ExecuteFunctionAsync(string storedProcedure, string connection, Dictionary<string, object> parameters)
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(connection))
                using (SqlCommand oCommand = new SqlCommand(storedProcedure, myConnection))
                {
                    oCommand.CommandType = CommandType.StoredProcedure;
                    oCommand.CommandTimeout = 120;

                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            oCommand.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(oCommand))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);
                        return dataSet;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int ExecuteScalarQuery(string str, string strconn)
        {
            using (SqlConnection myConnection = new SqlConnection(strconn))
            {
                try
                {
                    myConnection.Open();
                    using (SqlCommand cmd = new SqlCommand(str, myConnection))
                    {
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
                catch (Exception ex)
                {
                    myConnection.Close();
                    throw new ApplicationException("Exception in Run Query  " + ex.Message.ToString());
                }
                finally
                {
                    myConnection.Close();
                }

            }
        }
        public string Decrypturls(string encryptedText)
        {
            using var aesAlg = Aes.Create();
            //var keysection = _configuration.GetSection("AEskeys");
            string enkey =/* keysection["EncryptKey"]*/"42358357407472253245745740747545";
            aesAlg.Key = Encoding.UTF8.GetBytes(enkey);
            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Padding = PaddingMode.PKCS7;
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] iv = new byte[aesAlg.IV.Length];
            Array.Copy(encryptedBytes, 0, iv, 0, iv.Length);
            byte[] encryptedMessage = new byte[encryptedBytes.Length];
            Array.Copy(encryptedBytes, 0, encryptedMessage, 0, encryptedMessage.Length);

            using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv))
            {
                byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedMessage, 0, encryptedMessage.Length);
                string decryptedText = Encoding.UTF8.GetString(decryptedBytes);


                try
                {
                    JObject jsonObject = JObject.Parse(decryptedText);
                    return jsonObject.ToString();
                }
                catch (JsonReaderException)
                {

                    return decryptedText;
                }
            }
        }
    }
}
