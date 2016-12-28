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
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;

namespace Saladio
{
    public static class ObjectExtensions
    {
        public static string SerializeObject<T>(this T toSerialize)
        {
            return JsonConvert.SerializeObject(toSerialize);
        }

        public static T DeSerializeObject<T>(this string toDeserialize)
        {
            return JsonConvert.DeserializeObject<T>(toDeserialize);
        }
    }
}