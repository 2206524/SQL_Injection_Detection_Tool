using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Text.RegularExpressions;
using System.Data;
using System.Net.Mail;

namespace SQLIDetector
{
    public static partial class SQLIValidator
    {
        public static bool Validate_SQLIStrings(string txt)
        {
            if (txt.Contains(';') || txt.Contains("--") || txt.Contains("'") || txt.Contains("/*") || txt.Contains("*/") || txt.Contains("xp_"))
            {
                return false;
            }
            return true;
        }

       

        public static bool IsValidURL(string URLString)
        {
            return Uri.IsWellFormedUriString(URLString, UriKind.RelativeOrAbsolute);

        }

        public static bool ValidateURLStatement(string strurl)
        {
            // Construct Regular Expression To Find Text Blocks, Statement Breaks & SQL Statement Headers
            string regExText = @"('(''|[^'])*')|(;)|(\b(ALTER|CREATE|DELETE|DROP|EXEC(UTE){0,1}|INSERT( +INTO){0,1}|MERGE|SELECT|UPDATE|UNION( +ALL){0,1})\b)";
            // Remove extra separators
            RegexOptions regExOptions = RegexOptions.IgnoreCase | RegexOptions.Multiline;
            regExText = Regex.Replace(regExText, @"\(\|", "(", regExOptions);
            regExText = Regex.Replace(regExText, @"\|{2,}", "|", regExOptions);
            regExText = Regex.Replace(regExText, @"\|\)", ")", regExOptions);

            // Check for errors
            MatchCollection patternMatchList = Regex.Matches(strurl, regExText, regExOptions);
            for (int patternIndex = patternMatchList.Count - 1; patternIndex >= 0; patternIndex -= 1)
            {
                string value = patternMatchList[patternIndex].Value.Trim();
                if (string.IsNullOrWhiteSpace(value))
                {
                }
                // Continue - Not an error.
                else if (value.StartsWith("'") && value.EndsWith("'"))
                {
                }
                // Continue - Text Block
                else if (value.Trim() == ";")
                {
                    return true;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static void ValidateStatement(string commandText, StatementType authorizedStatements)
        {
            // Construct Regular Expression To Find Text Blocks, Statement Breaks & SQL Statement Headers
            string regExText = @"('(''|[^'])*')|(;)|(\b(ALTER|CREATE|DELETE|DROP|EXEC(UTE){0,1}|INSERT( +INTO){0,1}|MERGE|SELECT|UPDATE|UNION( +ALL){0,1})\b)";

            // Remove Authorized Options
            if ((authorizedStatements & StatementType.Batch) == StatementType.Batch)
                regExText = regExText.Replace("(;)", string.Empty);
            if ((authorizedStatements & StatementType.Delete) == StatementType.Delete)
                regExText = regExText.Replace("DELETE", string.Empty);
            if ((authorizedStatements & StatementType.Insert) == StatementType.Insert)
                regExText = regExText.Replace("INSERT( +INTO){0,1}", string.Empty);
            if ((authorizedStatements & StatementType.Select) == StatementType.Select)
                regExText = regExText.Replace("SELECT", string.Empty);
            if ((authorizedStatements & StatementType.Update) == StatementType.Update)
                regExText = regExText.Replace("UPDATE", string.Empty);

            // Remove extra separators
            RegexOptions regExOptions = RegexOptions.IgnoreCase | RegexOptions.Multiline;
            regExText = Regex.Replace(regExText, @"\(\|", "(", regExOptions);
            regExText = Regex.Replace(regExText, @"\|{2,}", "|", regExOptions);
            regExText = Regex.Replace(regExText, @"\|\)", ")", regExOptions);

            // Check for errors
            MatchCollection patternMatchList = Regex.Matches(commandText, regExText, regExOptions);
            for (int patternIndex = patternMatchList.Count - 1; patternIndex >= 0; patternIndex -= 1)
            {
                string value = patternMatchList[patternIndex].Value.Trim();
                if (string.IsNullOrWhiteSpace(value))
                {
                }
                // Continue - Not an error.
                else if (value.StartsWith("'") && value.EndsWith("'"))
                {
                }
                // Continue - Text Block
                else if (value.Trim() == ";")
                {
                    throw new UnauthorizedAccessException("Batch statements not authorized:"  + commandText);
                }
                else
                {
                    throw new UnauthorizedAccessException(value.Substring(0, 1).ToUpper() + value.Substring(1).ToLower() + " statements not authorized:" + commandText);
                }
            }
        }

        static readonly char[] SCapitals = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        /// <summary>
        /// Checks if the string is any form of a blank string.
        /// </summary>
        /// <param name="input">String to be checked, can safely be null</param>
        /// <returns>
        /// true if myString is null, or equals the empty string
        /// </returns>
        private static bool IsBlank(string input)
        {
            return (string.IsNullOrEmpty(input) || input.Trim().Length == 0);
        }

        /// <summary>
        /// Procedure to Check whether the Number is Integer
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>True if string is a valid integer</returns>
        private static bool IsNumber(string input)
        {
            if (IsBlank(input))
                return false;

            return input.All(Char.IsNumber);
        }

        /// <summary>
        /// Procedure to Check whether the number is Decimal
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="numeralPart">The numeral part.</param>
        /// <param name="decimalPart">The decimal part.</param>
        /// <returns>
        /// 	<c>true</c> if the specified input is decimal; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDecimal(string input, int numeralPart, int decimalPart)
        {
            if (IsBlank(input))
                return false;

            string[] parts = input.Split('.');

            if (parts.Length > 2)
            {
                return false;
            }

            if (parts[0].Length > numeralPart)
            {
                return false;
            }

            if (parts.Length > 1)
            {
                if (parts[1].Length > decimalPart)
                {
                    return false;
                }
            }

            return parts.All(IsNumber);
        }

        /// <summary>
        /// Procedure to Check for AlphaNumeric
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// 	<c>true</c> if [is alpha numeric] [the specified input]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAlphaNumeric(string input)
        {
            Regex alphaNumericPattern = new Regex("[^a-zA-Z0-9]");

            return !alphaNumericPattern.IsMatch(input);
        }

        /// <summary>
        /// Procedure to test for Integers both Positive and Negative
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// 	<c>true</c> if the specified input is integer; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInteger(string input)
        {
            Regex notIntPattern = new Regex("[^0-9-]");
            Regex intPattern = new Regex("^-[0-9]+$|^[0-9]+$");

            return !notIntPattern.IsMatch(input) && intPattern.IsMatch(input);
        }

        /// <summary>
        /// Procedure to test for Positive Integers
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// 	<c>true</c> if [is natural number] [the specified input]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNaturalNumber(string input)
        {
            Regex notNaturalPattern = new Regex("[^0-9]");
            Regex naturalPattern = new Regex("0*[0-9][0-9]*");

            return !notNaturalPattern.IsMatch(input) && naturalPattern.IsMatch(input);
        }

        /// <summary>
        /// Procedure to test for Alphabets
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// 	<c>true</c> if the specified input is alpha; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAlpha(String input)
        {
            Regex alphaPattern = new Regex("[^a-zA-Z]");

            return !alphaPattern.IsMatch(input);
        }

        public static bool isValidTextInput(String input)
        {
            Regex validTextPattern = new Regex(@"^[\p{L}\p{M}' \.\-]+$");

            return !validTextPattern.IsMatch(input);
        }

    /// <summary>
    /// Checks if a value is null or DBNull
    /// </summary>
    /// <param name="value">The possibly null value</param>
    /// <returns>
    /// true if the value is either null or equals System.DBNull.Value
    /// </returns>
    public static bool IsNull(object value)
        {
            return (null == value || DBNull.Value == value);
        }

        /// <summary>
        /// Removes all leading and trailing blanks, converts tabs, newlines and carriage-returns into blanks, removes
        /// repeated blanks.
        /// </summary>
        /// <param name="input">string to be trimmed.</param>
        /// <returns>trimmed up string</returns>
        private static string CleanWhiteSpace(string input)
        {
            if (IsBlank(input))
                return input;

            input = input.Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' ');

            while (-1 != input.IndexOf("  "))
                input = input.Replace("  ", " ");

            return input.Trim();
        }

        /// <summary>
        /// Converts an enumeration to a displayable string.
        /// </summary>
        /// <param name="enumValue">The enumeration</param>
        /// <returns>
        /// A string with all capitals preceded by blanks
        /// </returns>
        public static string DisplayableEnumeration(Enum enumValue)
        {
            string display = enumValue.ToString();
            int start = 1;

            while (true)
            {
                int capital = display.IndexOfAny(SCapitals, start);

                if (capital > 0)
                {
                    display = display.Substring(0, capital) + " " + display.Substring(capital);
                    start = capital + 2;
                }
                else
                {
                    return display.Trim();
                }
            }
        }

        /// <summary>
        /// Converts a camel string to a displayable string.
        /// </summary>
        /// <param name="value">The camel string</param>
        /// <returns>
        /// A string with all capitals preceded by blanks
        /// </returns>
        public static string DisplayableCamelString(string value)
        {
            string display = value;
            int start = 2;

            while (true)
            {
                int capital = display.IndexOfAny(SCapitals, start);

                if (capital <= 0)
                {
                    return display.Trim();
                }

                display = display.Substring(0, capital) + " " + display.Substring(capital);
                start = capital + 2;
            }
        }

        /// <summary>
        /// XMLs the encode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string XMLEncode(string value)
        {
            return value.Replace("&", "&amp;").Replace("'", "&apos;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

        /// <summary>
        /// XMLs the decode.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string XMLDecode(string value)
        {
            return value.Replace("&amp;", "&").Replace("&apos;", "'").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"");
        }

        /// <summary>
        /// Lefts the specified param.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static string Left(string param, int length)
        {
            //we start at 0 since we want to get the characters starting from the
            //left and with the specified lenght and assign it to a variable
            string result = param.Substring(0, length);
            //return the result of the operation
            return result;
        }
        /// <summary>
        /// Rights the specified param.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static string Right(string param, int length)
        {
            //start at the index based on the lenght of the sting minus
            //the specified lenght and assign it a variable
            string result = param.Substring(param.Length - length, length);
            //return the result of the operation
            return result;
        }

        /// <summary>
        /// Mids the specified param.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static string Mid(string param, int startIndex, int length)
        {
            //start at the specified index in the string ang get N number of
            //characters depending on the lenght and assign it to a variable
            string result = param.Substring(startIndex, length);
            //return the result of the operation
            return result;
        }

        /// <summary>
        /// Mids the specified param.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns></returns>
        public static string Mid(string param, int startIndex)
        {
            //start at the specified index and return all characters after it
            //and assign it to a variable
            string result = param.Substring(startIndex);
            //return the result of the operation
            return result;
        }

        public static bool IsValidCurrency(string currencyValue)
        {
            string pattern = @"\p{Sc}+\s*\d+";
            return Regex.IsMatch(currencyValue, pattern);
        }

        public static bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static bool IsValidPostalCode(string input)
        {
            Regex PostalCodePattern = new Regex("^([Gg][Ii][Rr] 0[Aa]{2}|([A-Za-z][0-9]{1,2}|[A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2}|[A-Za-z][0-9][A-Za-z]|[A-Za-z][A-Ha-hJ-Yj-y][0-9]?[A-Za-z]) {0,1}[0-9][A-Za-z]{2})$");

            return PostalCodePattern.IsMatch(input);
        }

        public static bool IsValidMobileNumber(string input)
        {
            Regex MobileNumberPattern = new Regex(@"^(((\+44\s?\d{4}|\(?0\d{4}\)?)\s?\d{3}\s?\d{3})|((\+44\s?\d{3}|\(?0\d{3}\)?)\s?\d{3}\s?\d{4})|((\+44\s?\d{2}|\(?0\d{2}\)?)\s?\d{4}\s?\d{4}))(\s?\#(\d{4}|\d{3}))?$");

            return MobileNumberPattern.IsMatch(input);
        }

    }
}
