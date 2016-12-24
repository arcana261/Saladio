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

namespace Saladio.Utility
{
    static class PagedApiHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startLengthExec">out: number of records, first param: start, second param: length</param>
        public static void FetchAll(Func<int, int, int> startLengthExec)
        {
            int start = 0;
            int total = 0;

            do
            {
                total = startLengthExec(start, 10);
                start += 10;
            } while (start < total);
        }
    }
}