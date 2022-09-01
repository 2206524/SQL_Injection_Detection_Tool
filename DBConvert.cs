using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLIDetector
{
    public static class DBConvert
    {

        public static bool ToBoolean(object Value)
        {
            if (Value == DBNull.Value)
                Value = 0;
            return Convert.ToBoolean(Value);
        }

        public static byte ToByte(object Value)
        {
            if (Value == DBNull.Value)
                Value = 0;
            return Convert.ToByte(Value);
        }

        public static char ToChar(object Value)
        {
            if (Value == DBNull.Value)
                Value = "";
            return Convert.ToChar(Value);
        }

        public static DateTime ToDateTime(object Value)
        {
            if (Value == DBNull.Value)
                Value = DateTime.MaxValue;
            return Convert.ToDateTime(Value);
        }

        public static decimal ToDecimal(object Value)
        {
            if (Value == DBNull.Value)
                Value = 0;
            return Convert.ToDecimal(Value);
        }

        public static double ToDouble(object Value)
        {
            if (Value == DBNull.Value)
                Value = 0;
            return Convert.ToDouble(Value);
        }

        public static short ToInt16(object Value)
        {
            if (Value == DBNull.Value)
                Value = 0;
            return Convert.ToInt16(Value);
        }

        public static int ToInt32(object Value)
        {
            if (Value == DBNull.Value)
                Value = 0;
            return Convert.ToInt32(Value);
        }

        public static long ToInt64(object Value)
        {
            if (Value == DBNull.Value)
                Value = 0;
            return Convert.ToInt64(Value);
        }


        public static string ToString(object Value)
        {
            if (Value == DBNull.Value)
                Value = "";
            return Convert.ToString(Value);
        }

        public static Object GetDBValue(Int32 Value)
        {
            if (Value == 0)
                return DBNull.Value;
            else
                return Value;
        }
    }
}