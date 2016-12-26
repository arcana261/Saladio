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

namespace Saladio
{
    public static class StringExtensions
    {
        public static string ToLatinNumbers(this string value)
        {
            StringBuilder ret = new StringBuilder();

            foreach (char ch in value)
            {
                char nextChar;

                switch (ch)
                {
                    case '۰':
                        nextChar = '0';
                        break;
                    case '۱':
                        nextChar = '1';
                        break;
                    case '۲':
                        nextChar = '2';
                        break;
                    case '۳':
                        nextChar = '3';
                        break;
                    case '۴':
                        nextChar = '4';
                        break;
                    case '۵':
                        nextChar = '5';
                        break;
                    case '۶':
                        nextChar = '6';
                        break;
                    case '۷':
                        nextChar = '7';
                        break;
                    case '۸':
                        nextChar = '8';
                        break;
                    case '۹':
                        nextChar = '9';
                        break;
                    default:
                        nextChar = ch;
                        break;
                }

                ret.Append(nextChar);
            }

            return ret.ToString();
        }

        public static string ToPersianNumbers(this string value)
        {
            StringBuilder ret = new StringBuilder();

            foreach (var ch in value)
            {
                char nextChar;

                switch (ch)
                {
                    case '0':
                        nextChar = '۰';
                        break;
                    case '1':
                        nextChar = '۱';
                        break;
                    case '2':
                        nextChar = '۲';
                        break;
                    case '3':
                        nextChar = '۳';
                        break;
                    case '4':
                        nextChar = '۴';
                        break;
                    case '5':
                        nextChar = '۵';
                        break;
                    case '6':
                        nextChar = '۶';
                        break;
                    case '7':
                        nextChar = '۷';
                        break;
                    case '8':
                        nextChar = '۸';
                        break;
                    case '9':
                        nextChar = '۹';
                        break;
                    default:
                        nextChar = ch;
                        break;
                }

                ret.Append(nextChar);
            }

            return ret.ToString();
        }
    }
}