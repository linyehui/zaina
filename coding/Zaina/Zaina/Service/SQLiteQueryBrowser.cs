using System;  
using System.Data;  
using System.Data.Common;  
using System.Data.SQLite;

namespace SQLiteQueryBrowser
{  
    /// <summary>  
    /// ˵��������һ�����System.Data.SQLite�����ݿⳣ�������װ��ͨ���ࡣ  
    /// ���ߣ�zhoufoxcn(�ܹ���  
    /// ���ڣ�2010-04-01  
    /// Blog:http://zhoufoxcn.blog.51cto.com or http://blog.csdn.net/zhoufoxcn  
    /// Version:0.1  
    /// </summary>  
    public class SQLiteDBHelper  
    {  
        private string connectionString = string.Empty;  
        /// <summary>  
        /// ���캯��  
        /// </summary>  
        /// <param name="dbPath">SQLite���ݿ��ļ�·��</param>  
        public SQLiteDBHelper(string dbPath)  
        {  
            this.connectionString = "Data Source=" + dbPath;  
        }  
        /// <summary>  
        /// ����SQLite���ݿ��ļ�  
        /// </summary>  
        /// <param name="dbPath">Ҫ������SQLite���ݿ��ļ�·��</param>  
        public static void CreateDB(string dbPath)  
        {  
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbPath))  
            {  
                connection.Open();  
                using (SQLiteCommand command = new SQLiteCommand(connection))  
                {  
                    command.CommandText = "CREATE TABLE Demo(id integer NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE)";  
                    command.ExecuteNonQuery();  
  
                    command.CommandText = "DROP TABLE Demo";  
                    command.ExecuteNonQuery();  
                }  
            }  
        }  
        /// <summary>  
        /// ��SQLite���ݿ�ִ����ɾ�Ĳ�����������Ӱ���������  
        /// </summary>  
        /// <param name="sql">Ҫִ�е���ɾ�ĵ�SQL���</param>  
        /// <param name="parameters">ִ����ɾ���������Ҫ�Ĳ���������������������SQL����е�˳��Ϊ׼</param>  
        /// <returns></returns>  
        public int ExecuteNonQuery(string sql, SQLiteParameter[] parameters)  
        {  
            int affectedRows = 0;  
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))  
            {  
                connection.Open();  
                using (DbTransaction transaction = connection.BeginTransaction())  
                {  
                    using (SQLiteCommand command = new SQLiteCommand(connection))  
                    {  
                        command.CommandText = sql;  
                        if (parameters != null)  
                        {  
                            command.Parameters.AddRange(parameters);  
                        }  
                        affectedRows = command.ExecuteNonQuery();  
                    }  
                    transaction.Commit();  
                }  
            }  
            return affectedRows;  
        }  
        /// <summary>  
        /// ִ��һ����ѯ��䣬����һ��������SQLiteDataReaderʵ��  
        /// </summary>  
        /// <param name="sql">Ҫִ�еĲ�ѯ���</param>  
        /// <param name="parameters">ִ��SQL��ѯ�������Ҫ�Ĳ���������������������SQL����е�˳��Ϊ׼</param>  
        /// <returns></returns>  
        public SQLiteDataReader ExecuteReader(string sql, SQLiteParameter[] parameters)  
        {  
            SQLiteConnection connection = new SQLiteConnection(connectionString);  
            SQLiteCommand command = new SQLiteCommand(sql, connection);  
            if (parameters != null)  
            {  
                command.Parameters.AddRange(parameters);  
            }  
            connection.Open();  
            return command.ExecuteReader(CommandBehavior.CloseConnection);  
        }  
        /// <summary>  
        /// ִ��һ����ѯ��䣬����һ��������ѯ�����DataTable  
        /// </summary>  
        /// <param name="sql">Ҫִ�еĲ�ѯ���</param>  
        /// <param name="parameters">ִ��SQL��ѯ�������Ҫ�Ĳ���������������������SQL����е�˳��Ϊ׼</param>  
        /// <returns></returns>  
        public DataTable ExecuteDataTable(string sql, SQLiteParameter[] parameters)  
        {  
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))  
            {  
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))  
                {  
                    if (parameters != null)  
                    {  
                        command.Parameters.AddRange(parameters);  
                    }  
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);  
                    DataTable data = new DataTable();  
                    adapter.Fill(data);  
                    return data;  
                }  
            }  
              
        }  
        /// <summary>  
        /// ִ��һ����ѯ��䣬���ز�ѯ����ĵ�һ�е�һ��  
        /// </summary>  
        /// <param name="sql">Ҫִ�еĲ�ѯ���</param>  
        /// <param name="parameters">ִ��SQL��ѯ�������Ҫ�Ĳ���������������������SQL����е�˳��Ϊ׼</param>  
        /// <returns></returns>  
        public Object ExecuteScalar(string sql, SQLiteParameter[] parameters)  
        {  
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))  
            {  
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))  
                {  
                    if (parameters != null)  
                    {  
                        command.Parameters.AddRange(parameters);  
                    }  
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);  
                    DataTable data = new DataTable();  
                    adapter.Fill(data);  
                    return data;  
                }  
            }  
        }  
        /// <summary>  
        /// ��ѯ���ݿ��е���������������Ϣ  
        /// </summary>  
        /// <returns></returns>  
        public DataTable GetSchema()  
        {  
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))  
            {  
                connection.Open();  
                DataTable data=connection.GetSchema("TABLES");  
                connection.Close();  
                //foreach (DataColumn column in data.Columns)  
                //{  
                //    Console.WriteLine(column.ColumnName);  
                //}  
                return data;  
            }  
        }  
  
        public bool IsTableExist(string table)
        {
            string sql = "select sql from sqlite_master WHERE type = 'table' and name = '";
            sql += table;
            sql += "'";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))  
            {  
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))  
                {  
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);  
                    DataTable data = new DataTable();  
                    adapter.Fill(data);  
                    return data.Rows.Count > 0;  
                }  
            }
        }
    }  
}  