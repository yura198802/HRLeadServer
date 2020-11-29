using Monica.Core.ModelParametrs.ModelsArgs;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;

namespace Monica.Core.Service.Stimulsoft
{
    public class MySQLAdapter
    {
        private static MySqlConnection _connection;
        private static MySqlDataReader _reader;
        private static CommandJson _command;
        private static string _userName;
    
        private static StimulSoftResult End(StimulSoftResult stimulSoftResult) 
        {
            try 
            {
                _userName = string.Empty;
                if (_reader != null) _reader.Close();
                if (_connection != null) _connection.Close();
                return stimulSoftResult;
            }
            catch (Exception e) 
            {
                return stimulSoftResult;
            }
        }
    
        private static StimulSoftResult OnError(string message) {
            return End(new StimulSoftResult { Success = false, Notice = message });
        }

        private static StimulSoftResult Connect() {
            try
            {
                _connection = new MySqlConnection(_command.ConnectionString);
                _connection.Open();
                var command = new MySqlCommand($"set @UserName = '{_userName}';") 
                { 
                    CommandType = System.Data.CommandType.Text, 
                    Connection = _connection 
                };
                command.ExecuteNonQuery();
                return OnConnect();
            }
            catch(Exception e)
            {
                return OnError(e.Message);
            }
        }

        private static StimulSoftResult OnConnect() {
            if (!String.IsNullOrEmpty(_command.QueryString))
                return Query(_command.QueryString);
            else return End(new StimulSoftResult { Success = true });
        }

        private static StimulSoftResult Query(string queryString) {
            try
            {
                var sqlCommand = _connection.CreateCommand();
                sqlCommand.CommandText = queryString;
                _reader = sqlCommand.ExecuteReader();
                return OnQuery();
            }
            catch (Exception e)
            {
                return OnError(e.Message);
            }
        }

        private static StimulSoftResult OnQuery()
        {
            var columns = new List<string>();
            var rows = new List<string[]>();
            var types = new List<string>();

            for (var index = 0; index < _reader.FieldCount; index++)
            {
                var columnName = _reader.GetName(index);
                var columnType = MySQLAdapter.GetType(_reader.GetFieldType(index));

                columns.Add(columnName);
                types.Add(columnType);
            }

            while (_reader.Read())
            {
                var row = new string[_reader.FieldCount];
                for (var index = 0; index < _reader.FieldCount; index++)
                {
                    var columnName = _reader.GetName(index);
                    var columnType = MySQLAdapter.GetType(_reader.GetFieldType(index));

                    var columnIndex = columns.IndexOf(columnName);
                    if (types[columnIndex] != "array") types[columnIndex] = columnType;
                    object value = null;
                    if (!_reader.IsDBNull(index))
                    {
                        if (columnType == "array")
                        {
                            value = MySQLAdapter.GetBytes(index);
                        }
                        else value = _reader.GetValue(index);
                    }

                    if (value == null) value = "";
                    row[index] = value.ToString();
                }
                rows.Add(row);
            }

            return End(new StimulSoftResult { Success = true, Columns = columns.ToArray(), Rows = rows.ToArray(), Types = types.ToArray() });
        }

        private static string GetBytes(int index)
        {
            var size = _reader.GetBytes(index, 0, null, 0, 0);
            var destination = new MemoryStream();
            var buffer = new byte[8040];
            long offset = 0;
            long read;

            while ((read = _reader.GetBytes(index, offset, buffer, 0, buffer.Length)) > 0)
            {
                offset += read;
                destination.Write(buffer, 0, (int)read);
                if (size == offset) break;
            }

            return System.Convert.ToBase64String(destination.ToArray());
        }

        private static string GetType(Type columnType)
        {
            var typeCode = Type.GetTypeCode(columnType);

            if ((int)typeCode >= 5 && (int)typeCode <= 15) return "number";
            if ((int)typeCode == 1) return "array";

            return "string";
        }

        public static StimulSoftResult Process(CommandJson command,string userName)
        {
            MySQLAdapter._command = command;
            _userName = userName;
            return Connect();
        }
    }
}