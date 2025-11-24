using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlugRMK.GenericUti
{
    public static class NumberDisplayUtility
    {
        const int MILLION = 1_000_000;
        const int THOUSAND = 1000;
        public static string Shorten(this int number, bool noTrailing = true)
        {
            string numberString;
            
            if (number >= MILLION)
                numberString = ((float)number/MILLION).ToString("f1")+"M";
            else if (number >= THOUSAND)
                numberString = ((float)number/THOUSAND).ToString("f1")+"K";
            else 
                numberString = number.ToString("f1");

            if (noTrailing && numberString.EndsWith(".0"))
                return numberString[..^2];
            else
                return numberString;
        }

        public static string Shorten(this float number, bool noTrailing = true)
        {
            string numberString;

            if (number >= MILLION)
                numberString = (number/MILLION).ToString("f1")+"M";
            else if (number >= THOUSAND)
                numberString = (number/THOUSAND).ToString("f1")+"K";
            else 
                numberString = number.ToString("f1");

            if (noTrailing && numberString.EndsWith(".0"))
                return numberString[..^2];
            else
                return numberString;
        }

        public static int RoundToHundredsOrHundredThousands(int value)
        {
            if (value > 1_000_000)
                return  Mathf.FloorToInt(value / 100_000) * 100_000;
            else if (value > 1000)
                return  Mathf.FloorToInt(value / 100) * 100;
            else
                return Mathf.FloorToInt(value);
        }
    
        public static string ToClockDisplay(this int seconds, string separator = ":")
        {
            var minutes = seconds / 60;
            var secondsLeft = seconds % 60;
            return string.Format("{0:D2}{1}{2:D2}", minutes, separator, secondsLeft);
        }
    }
}
