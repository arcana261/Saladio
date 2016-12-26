using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using SQLite;
using Android.Util;

namespace Saladio.Data
{
    public static class DbConnection
    {
        private static string mDataBasePath = null;
        private static SQLiteConnection mConnection = null;

        private static string GetDataBasePath()
        {
            if (mDataBasePath == null)
            {
                mDataBasePath = Path.Combine(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
                    "saladio.db3");
            }

            return mDataBasePath;
        }

        public static SQLiteConnection Connection
        {
            get
            {
                if (mConnection == null)
                {
                    mConnection = new SQLiteConnection(GetDataBasePath());
                    try
                    {
                        mConnection.CreateTable<KeyValue>();
                    }
                    catch (Exception e)
                    {
                        Log.Debug("Saladio.DbConnection", e.ToString());
                    }

                    var pairVersion = mConnection.Table<KeyValue>().Where(x => x.Key.Equals("migration-version")).FirstOrDefault();
                    int version = 0;

                    if (pairVersion != null)
                    {
                        version = int.Parse(pairVersion.Value);
                    }

                    while (MigrateNext(mConnection, version))
                    {
                        version = version + 1;
                        mConnection.InsertOrReplace(new KeyValue()
                        {
                            Key = "migration-version",
                            Value = version.ToString()
                        });
                    }
                    mConnection.InsertOrReplace(new KeyValue()
                    {
                        Key = "migration-version",
                        Value = version.ToString()
                    });
                }

                return mConnection;
            }
        }

        private static bool MigrateNext(SQLiteConnection connection, int version)
        {
            return false;
        }
    }
}