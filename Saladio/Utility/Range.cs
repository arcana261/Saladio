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
    public static class Range
    {
        public static IEnumerable<int> New(int from, int to , int increment)
        {
            if (increment > 0)
            {
                while (from < to)
                {
                    yield return from;
                    from = from + increment;
                }
            }
            else if (increment < 0)
            {
                while(from > to)
                {
                    yield return from;
                    from = from + increment;
                }
            }
            else
            {
                throw new ArgumentException($"increment '{increment} can not be zero", "increment");
            }
        }

        public static IEnumerable<int> New(int from, int to)
        {
            return New(from, to, from < to ? 1 : -1);
        }
    }
}