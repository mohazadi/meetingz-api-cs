using System;
using System.IO;

namespace Meetingz.Log
{
    public class ClsLog
    {
        string _strLogFilePath;
        string _strLineBreak= "\n========================\n";
        string _strLineBreakCustom = "\n*********************************\n\n\n\n";
        string _strLineBreakEnd ="\n----------------------------------------------------------\n\n\n";
        FileInfo _logFileInfo;
        long _maxLogFileSize=0;
        
        /// <summary>
        /// Log class used to write exception details or 
        /// other specific details into a text file.
        /// </summary>
        /// <param name="strLogFilePath">Full path of the log file including filename</param>
        /// <param name="MaxLogFileSize">Maximum Log Size that can be acccomodated on the disk.
        /// (number of Bytes as Long).
        /// Log will be deleted/cleared if size exceeds.
        /// Pass 0 for NO LIMIT on filesize</param>
        public ClsLog(string strLogFilePath, long MaxLogFileSize)
        {
            this._maxLogFileSize = MaxLogFileSize;
            this._strLogFilePath = strLogFilePath;
            _logFileInfo  = new FileInfo(strLogFilePath);
        }

        private bool CheckLogSize()
        {
            if (_maxLogFileSize != 0)
            {
                if (_logFileInfo.Length > _maxLogFileSize)
                {
                    File.Delete(_strLogFilePath);
                    return true;
                }
                else
                {
                    return false;
                }

            }
            return false;
        }
        /// <summary>
        /// Writes exceptions to log files
        /// </summary>
        /// <param name="ex">Pass the exception ex as parameter.</param>
        /// <returns>Returns false if an exception occurs while writing to file</returns>
        public bool Write(Exception ex)
        {
            try
            {
                CheckLogSize();
                if (File.Exists(_strLogFilePath))
                {
                    File.AppendAllText(_strLogFilePath, DateTime.Now
                                                        + " : Exception :" 
                                                        + ex.Message + "\n"
                                                        + "Inner Exception : " + _strLineBreak
                                                        + ex.InnerException + "\n"
                                                        + "Stack Trace :" + _strLineBreak
                                                        + ex.StackTrace + "\n"
                                                        + "Source : " + _strLineBreak
                                                        + ex.Source + _strLineBreakEnd);
                    return true;
                }
                else
                {
                    File.WriteAllText(_strLogFilePath, DateTime.Now
                                                       + " : Exception :"+ _strLineBreak
                                                       + ex.Message + "\n"
                                                       + "Inner Exception :" + _strLineBreak
                                                       + ex.InnerException + "\n"
                                                       + "Stack Trace :" + _strLineBreak
                                                       + ex.StackTrace + "\n"
                                                       + "Source :" + _strLineBreak
                                                       + ex.Source + _strLineBreakEnd);
                    return true;
                }
            }
            catch { return false; }
        }

        /// <summary>
        /// Write custom strings apart from exceptions
        /// </summary>
        /// <param name="strMessage"></param>
        /// <returns></returns>
        public bool Write(string strMessage)
        {
            try
            {
                if (File.Exists(_strLogFilePath))
                {
                    File.AppendAllText(_strLogFilePath, _strLineBreak
                                                        + DateTime.Now
                                                        + " : " + strMessage + _strLineBreakCustom);
                    return true;
                }
                else
                {
                    File.WriteAllText(_strLogFilePath, _strLineBreak
                                                       + DateTime.Now
                                                       + " : " + strMessage + _strLineBreakCustom);
                    return true;
                }
            }
            catch { return false; }
        }



    }
}

    

