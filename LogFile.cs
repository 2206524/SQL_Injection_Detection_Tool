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


namespace SQLIDetector
{
    public static class LogFile
    {
        //private static StreamWriter writer;

         /// <summary>
        /// New Log File Instance
        /// </summary>
        /// <param name="Process">Batch Process Name</param>
        /// <param name="FilePath">Log Directory Path</param>
        public static void WriteLog(string Process, string FilePath,string msg)
        {
            CreateLogFile(Process, FilePath, false, msg);
        }

        /// <summary>
        /// New Log File Instance
        /// </summary>
        /// <param name="Process">Batch Process Name</param>
        public static void WriteLog(string Process, string msg)
        {
            //string FilePath = ConfigurationManager.AppSettings["LogPath"].ToString();
            string FilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            CreateLogFile(Process, FilePath, false, msg);
        }

        private static void CreateLogFile(string Process, string FilePath, bool CreateLogFolder, string strMsg)
        {

            FilePath = FilePath + @"\\Logs\\";
            if (!Directory.Exists(FilePath))
                Directory.CreateDirectory(FilePath);

            string FileName = string.Format("{0}Log_{1}_{2}.log", FilePath, Process, DateTime.Now.ToString("yyyy_MM_dd"));

            using (var streamWriter = new StreamWriter(FileName, true))
            {
                streamWriter.WriteLine(DateTime.Now +" : " + strMsg);
            }
        }       
    }
}