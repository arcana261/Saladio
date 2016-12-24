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
using Saladio.Data;
using IO.Swagger.Api;
using IO.Swagger.Client;

namespace Saladio.Config
{
    public static class SharedConfig
    {
        private static string mUserName;
        private static string mPassword;

        public static string GetValue(string key)
        {
            var pair = DbConnection.Connection.Table<KeyValue>().Where(x => x.Key.Equals(key)).FirstOrDefault();

            if (pair == null)
            {
                return null;
            }

            return pair.Value;
        }

        public static void SetValue(string key, string value)
        {
            DbConnection.Connection.InsertOrReplace(new KeyValue()
            {
                Key = key,
                Value = value
            });
        }

        public static string UserName
        {
            get
            {
                if (mUserName == null)
                {
                    mUserName = GetValue("global:username");
                }

                return mUserName;
            }
            set
            {
                mUserName = value;
                SetValue("global:username", value);
            }
        }

        public static string Password
        {
            get
            {
                if (mPassword == null)
                {
                    mPassword = GetValue("global:password");
                }

                return mPassword;
            }
            set
            {
                mPassword = value;
                SetValue("global:password", value);
            }
        }

        public static bool IsRegistered
        {
            get
            {
                return UserName != null && Password != null;
            }
        }

        public static bool IsRegistrationValid
        {
            get
            {
                if (!IsRegistered)
                {
                    return false;
                }

                try
                {
                    UsersApi api = new UsersApi(AuthorizedApiConfig);
                    api.GetCurrentUser();
                }
                catch (ApiException e)
                {
                    if (e.ErrorCode == 401 || e.ErrorCode == 403)
                    {
                        return false;
                    }
                    else
                    {
                        throw e;
                    }
                }

                return true;
            }
        }

        public static IO.Swagger.Client.Configuration UnAuthorizedApiConfig
        {
            get
            {
                var ret = new IO.Swagger.Client.Configuration(new IO.Swagger.Client.ApiClient(ApiEndpoint));

                return ret;
            }
        }

        public static IO.Swagger.Client.Configuration AuthorizedApiConfig
        {
            get
            {
                var ret = UnAuthorizedApiConfig;
                ret.Username = UserName;
                ret.Password = Password;

                return ret;
            }
        }

        public static string ApiEndpoint
        {
            get
            {
                //return "http://192.168.2.9:10010/v1";
                return "http://169.254.89.238:10010/v1";
            }
        }
    }
}