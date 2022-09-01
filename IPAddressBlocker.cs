using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Web;


namespace SQLIDetector
{
    public static partial class IPAddressBlocker
    {
        private static FileStream stream;
        private static string IPA;
        public static void BlockIPA(HttpRequestBase request)
        {
            IPA = IPAddressValidator.ClientIPFromRequest(request, false);
            CreateFile();
            LogFile.WriteLog("IP Address Blocker", string.Format("SQL Injection Detected from IP Address - {0}", IPA));
            SendEmailAlert(IPA);
        }

        public static bool CheckBlackListIPA (HttpRequestBase request)
        {
            IPA = IPAddressValidator.ClientIPFromRequest(request, false);
            //string FilePath = ConfigurationManager.AppSettings["IPABlockerPath"].ToString();
            string FilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            FilePath = FilePath + @"\\IPA\\";
            if (!Directory.Exists(FilePath))
                return false;
            string FileName = string.Format("{0}IPA_Blocked.log", FilePath);
            if (!File.ReadLines(FileName).Any(line => line.Contains(IPA)))
            {
               return true;
            }
            return false;
        }


        private static void CreateFile()
        {
            string FilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            FilePath = FilePath + @"\\IPA\\";
            if (!Directory.Exists(FilePath))
                Directory.CreateDirectory(FilePath);
            string FileName = string.Format("{0}IPA_Blocked.log", FilePath);
            // Create a FileStream with mode CreateNew  
            stream = new FileStream(FileName, FileMode.Append);
            WriteLine();
        }
        private static void WriteLine()
        {
            using (var streamWriter = new StreamWriter(stream))
            {
                streamWriter.WriteLine(IPA);
            }
        }

        private static void SendEmailAlert(string IPAddress)
        {
            Email mail = new Email();
            mail.emailToAddress = ConfigurationManager.AppSettings["SysAdminEmail"].ToString();
            mail.subject = "Alert - SQL Injection Detected";
            mail.body = string.Format("SQL Injection Detected from IP Address - {0}", IPAddress);
            //mail.SendEmail();
        }
       
    }

    
}
