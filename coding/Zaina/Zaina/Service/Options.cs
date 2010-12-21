using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using SQLiteQueryBrowser;
using System.IO;
using System.Diagnostics;

namespace Zaina
{
    class Options
    {
        private const string MODULE_DB = @"\location.db";
        private const string SinaUserName = "sina_weibo_name";
        private const string SinaPassword = "sina_weibo_psw";
        private const string MapType = "map_type";

        public Options()
        {

        }

        public bool Add(DateTime currentTime, double lat, double lng, string address)
        {
            CreateTable();
            InsertHistoryItem(currentTime, lat, lng, address);
            List<LocationItem> result = GetHistory(2);

            return true;
        }

        public List<LocationItem> GetHistory(int limitCount)
        {
            List<LocationItem> result = new List<LocationItem>();

            try
            {
                SQLiteDBHelper db = new SQLiteDBHelper(DatabasePath);
                string sql = @"select time, lat, lng, address from t_location
                                order by id desc limit ";
                sql += limitCount.ToString();

                using (SQLiteDataReader reader = db.ExecuteReader(sql, null))
                {
                    while (reader.Read())
                    {
                        LocationItem item = new LocationItem();
                        item.CheckinTime = reader.GetString(0);
                        item.Lat = reader.GetDouble(1);
                        item.Lng = reader.GetDouble(2);
                        item.Address = reader.GetString(3);
                        result.Add(item);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine(ex.Message);
            }


            return result;
        }

        public void Clear()
        {
            try
            {
                SQLiteDBHelper db = new SQLiteDBHelper(DatabasePath);
                string sql = @"delete from t_location where 1=1;";
                db.ExecuteNonQuery(sql, null);
            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public bool GetSinaWeiboUserInfo(out string userName, out string password)
        {
            userName = "";
            password = "";

            if (!GetConfigItem(SinaUserName, out userName))
                return false;

            if (!GetConfigItem(SinaPassword, out password))
                return false;

            return true;
        }

        public bool SetSinaWeiboUserInfo(string userName, string password)
        {
            CreateTable();
            SetConfigItem(SinaUserName, userName);
            SetConfigItem(SinaPassword, password);

            return true;
        }

        public bool GetIsShowStatellite()
        {
            string strMapType = "";
            if (!GetConfigItem(MapType, out strMapType))
                return false;
            if ("1" == strMapType)
                return true;
            else
                return false;
        }

        public bool SetIsShowStatellite(bool bShowStatellite)
        {
            CreateTable();
            if (bShowStatellite)
                SetConfigItem(MapType, "1");
            else
                SetConfigItem(MapType, "0");

            return true;
        }

        //////////////////////////////////////////////////////////////////////////
        // protected methods

        protected void CreateTable()
        {
            try
            {
                //如果不存在改数据库文件，则创建该数据库文件  
                if (!System.IO.File.Exists(DatabasePath))
                {
                    SQLiteDBHelper.CreateDB(DatabasePath);
                }

                SQLiteDBHelper db = new SQLiteDBHelper(DatabasePath);
                if (!db.IsTableExist("t_location"))
                {
                    string sqlCreateTable = @"CREATE TABLE t_location(
                                            id integer NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 
                                            time TIMESTAMP NOT NULL,    
                                            lat double NOT NULL, 
                                            lng double NOT NULL, 
                                            address varchar(100))";
                    db.ExecuteNonQuery(sqlCreateTable, null);
                }

                if (!db.IsTableExist("t_config"))
                {
                    string sqlCreateTable = @"CREATE TABLE [t_config] (
                                            [name] varchar(100)  UNIQUE NOT NULL PRIMARY KEY,
                                            [value] varchar(100)  NOT NULL
                                            )";
                    db.ExecuteNonQuery(sqlCreateTable, null);
                }
            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine(ex.Message);
            }            
        }

        protected bool InsertHistoryItem(DateTime currentTime, double lat, double lng, string address)
        {
            try
            {
                string sql = "INSERT INTO t_location(time, lat, lng, address) VALUES ('";
                sql += currentTime.ToString("yyyy-MM-dd HH:mm:ss");
                sql += "','";
                sql += lat.ToString();
                sql += "','";
                sql += lng.ToString();
                sql += "', '";
                sql += address;
                sql += "')";
                SQLiteDBHelper db = new SQLiteDBHelper(DatabasePath);
                int affectedRows = db.ExecuteNonQuery(sql, null);
                return affectedRows > 0;
            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        protected bool GetConfigItem(string name, out string value)
        {
            value = "";
            try
            {
                string sql = "select value from t_config where name = '";
                sql += name;
                sql += "';";
                SQLiteDBHelper db = new SQLiteDBHelper(DatabasePath);
                using (SQLiteDataReader reader = db.ExecuteReader(sql, null))
                {
                    while (reader.Read())
                    {
                        value = reader.GetString(0);
                        return true;
                    }
                }

                return false;
            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        protected bool SetConfigItem(string name, string value)
        {
            if (IsConfigHasExist(name))
                return UpdateConfigItem(name, value);
            else
                return InsertConfigItem(name, value);
        }

        protected bool IsConfigHasExist(string name)
        {
            try
            {
                string sql = "select * from t_config where name = '";
                sql += name;
                sql += "';";
                SQLiteDBHelper db = new SQLiteDBHelper(DatabasePath);
                using (SQLiteDataReader reader = db.ExecuteReader(sql, null))
                {
                    return reader.Read();
                }                
            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        protected bool InsertConfigItem(string name, string value)
        {
            try
            {
                string sql = "INSERT INTO t_config(name, value) VALUES ('";
                sql += name;
                sql += "','";
                sql += value;
                sql += "')";

                SQLiteDBHelper db = new SQLiteDBHelper(DatabasePath);
                int affectedRows = db.ExecuteNonQuery(sql, null);
                return affectedRows > 0;
            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        protected bool UpdateConfigItem(string name, string value)
        {
            try
            {
                string sql = "update t_config set value = '";
                sql += value;
                sql += "' where name = '";
                sql += name;
                sql += "';";

                SQLiteDBHelper db = new SQLiteDBHelper(DatabasePath);
                int affectedRows = db.ExecuteNonQuery(sql, null);
                return affectedRows > 0;
            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        protected string DatabasePath
        {
            get
            {
                string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                path += MODULE_DB;
                return path;
            }
        }
    }
}
