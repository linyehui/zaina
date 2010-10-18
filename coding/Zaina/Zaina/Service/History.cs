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
    class History
    {
        private const string MODULE_DB = @"\location.db";
        public History()
        {

        }

        public bool Add(DateTime currentTime, double lat, double lng, string address)
        {
            CreateTable();
            InsertItem(currentTime, lat, lng, address);
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
            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine(ex.Message);
            }            
        }

        protected bool InsertItem(DateTime currentTime, double lat, double lng, string address)
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
