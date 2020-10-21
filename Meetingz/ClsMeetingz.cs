using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net;
using System.IO;
using System.Diagnostics;
using Meetingz.Log;


namespace Meetingz
{
    /// <summary>
    /// This project is developed by Meetingz.Ir
    /// Contact me support in integrating Meetingz with .Net or for customizing Meetingz in anyway.
    /// E-mail me at mmd.azadi@gmail.com for any support on this code or even support on Meetingz.
    /// </summary>
    public class ClsMeetingz
    {
        readonly ClsLog _objclsLog = new ClsLog(AppDomain.CurrentDomain.BaseDirectory + "log.txt", 1000);

        private string StrServerIpAddress { get; set; }
        private string StrSalt { get; set; }

        //public ClsMeetingz()
        //{
        //    StrServerIpAddress =  "https://" + File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "ServerIPAddress.txt") + "/mtz/";
        //    StrSalt = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "ServerId.txt");
        //}
        public ClsMeetingz(string apiUrl, string apiSecret)
        {
            StrServerIpAddress = apiUrl;
            StrSalt = apiSecret;
        }




        #region "CreateMeeting"

        /// <summary>
        /// Creates the Meeting
        /// </summary>
        /// <param name="MeetingName">Creates the Meeting with the Specified MeetingName</param>
        /// <param name="MeetingId">Creates the Meeting with the Specified MeetingId</param>
        /// <param name="attendeePw">Creates the Meeting with the Specified AttendeeePassword</param>
        /// <param name="moderatorPw">Creates the Meeting with the Specified ModeratorPassword</param>
        /// <returns></returns>
        public DataTable CreateMeeting(string MeetingName, string MeetingId, string attendeePw, string moderatorPw)
        {
            try
            {
                var strParameters = "name=" + MeetingName + "&meetingID=" + MeetingId + "&attendeePW=" + attendeePw +
                                    "&moderatorPW=" + moderatorPw;
                var strSha1CheckSum = ClsData.GetSha1("create" + strParameters + StrSalt);
                var request =
                    (HttpWebRequest) WebRequest.Create(StrServerIpAddress + "api/create?" + strParameters +
                                                       "&checksum=" + strSha1CheckSum);
                var response = (HttpWebResponse) request.GetResponse();
                var sr = new StreamReader(response.GetResponseStream());
                var ds = new DataSet("DataSet1");
                ds.ReadXml(sr);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                _objclsLog.Write(ex.Message);
                return null;
            }
        }

        #endregion

        #region "JoinMeeting"

        /// <summary>
        /// To Join in the Existing Meeting
        /// </summary>
        /// <param name="MeetingName">To Join in the ExistingMeeting with the Specified MeetingName</param>
        /// <param name="MeetingId">To Join in the ExistingMeeting with the Specified MeetingId</param>
        /// <param name="Password">To Join in the ExistingMeeting with the Specified ModeratorPW/AttendeePW</param>
        /// <param name="ShowInBrowser">If its true,will Show the Meeting UI in the Browser </param>
        /// <returns></returns>
        public string JoinMeeting(string MeetingName, string MeetingId, string Password, bool ShowInBrowser)
        {
            try
            {
                var strParameters = "fullName=" + MeetingName + "&meetingID=" + MeetingId + "&password=" + Password;
                var strSha1CheckSum = ClsData.GetSha1("join" + strParameters + StrSalt);
                if (!ShowInBrowser)
                {
                    var request = (HttpWebRequest) WebRequest.Create(
                        StrServerIpAddress + "api/join?" + strParameters + "&checksum=" + strSha1CheckSum);
                    var response = (HttpWebResponse) request.GetResponse();
                    var sr = new StreamReader(response.GetResponseStream());
                    return sr.ReadToEnd();
                }
                else
                {
                    Process.Start(StrServerIpAddress + "api/join?" + strParameters + "&checksum=" + strSha1CheckSum);
                    return "Showed Successfully";
                }
            }
            catch (Exception ex)
            {
                _objclsLog.Write(ex.Message);
                return null;
            }
        }

        #endregion

        #region "IsMeetingRunning"

        /// <summary>
        /// To find the Status of the Existing Meeting
        /// </summary>
        /// <param name="MeetingId">To find the Status of the Existing Meeting with the Specified MeetingId</param>
        /// <returns></returns>
        public DataTable IsMeetingRunning(string MeetingId)
        {
            try
            {
                var strParameters = "meetingID=" + MeetingId;
                var strSha1CheckSum = ClsData.GetSha1("isMeetingRunning" + strParameters + StrSalt);
                var request = (HttpWebRequest) WebRequest.Create(
                    StrServerIpAddress + "api/isMeetingRunning?" + strParameters + "&checksum=" + strSha1CheckSum);
                var response = (HttpWebResponse) request.GetResponse();
                var sr = new StreamReader(response.GetResponseStream());
                var ds = new DataSet("DataSet1");
                ds.ReadXml(sr);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                _objclsLog.Write(ex.Message);
                return null;
            }
        }

        #endregion

        #region "GetMeetingInfo"

        /// <summary>
        /// To Get the relavant information about the Meeting
        /// </summary>
        /// <param name="MeetingId">To Get the relevant information about the Meeting with the Specified MeetingId</param>
        /// <param name="ModeratorPassword">To Get the relevant information about the Meeting with the Specified ModeratorPW</param>
        /// <returns></returns>
        public DataTable GetMeetingInfo(string MeetingId, string ModeratorPassword)
        {
            try
            {
                var strParameters = "meetingID=" + MeetingId + "&password=" + ModeratorPassword;
                var strSha1CheckSum = ClsData.GetSha1("getMeetingInfo" + strParameters + StrSalt);
                var request = (HttpWebRequest) WebRequest.Create(
                    StrServerIpAddress + "api/getMeetingInfo?" + strParameters + "&checksum=" + strSha1CheckSum);
                var response = (HttpWebResponse) request.GetResponse();
                var sr = new StreamReader(response.GetResponseStream());
                var ds = new DataSet("DataSet1");
                ds.ReadXml(sr);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                _objclsLog.Write(ex.Message);
                return null;
            }
        }

        #endregion

        #region "EndMeeting"

        /// <summary>
        /// To End the Meeting
        /// </summary>
        /// <param name="MeetingId">To End the Meeting with the Specified MeetingId</param>
        /// <param name="ModeratorPassword">To End the Meeting with the Specified ModeratorPW</param>
        /// <returns></returns>
        public DataTable EndMeeting(string MeetingId, string ModeratorPassword)
        {
            try
            {
                var strParameters = "meetingID=" + MeetingId + "&password=" + ModeratorPassword;
                var strSha1CheckSum = ClsData.GetSha1("end" + strParameters + StrSalt);
                var request =
                    (HttpWebRequest) WebRequest.Create(StrServerIpAddress + "api/end?" + strParameters + "&checksum=" +
                                                       strSha1CheckSum);
                var response = (HttpWebResponse) request.GetResponse();
                var sr = new StreamReader(response.GetResponseStream());
                var ds = new DataSet("DataSet1");
                ds.ReadXml(sr);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                _objclsLog.Write(ex.Message);
                return null;
            }
        }

        #endregion


        #region "getMeetings"

        /// <summary>
        /// To Get all the Meeting's Information running in the Server
        /// </summary>
        /// <returns></returns>
        public DataTable GetMeetings()
        {
            try
            {
                var r = new Random(0);
                var strParameters = "random=" + r.Next(100);
                var strSha1CheckSum = ClsData.GetSha1("getMeetings" + strParameters + StrSalt);
                var request = (HttpWebRequest) WebRequest.Create(
                    StrServerIpAddress + "api/getMeetings?" + strParameters + "&checksum=" + strSha1CheckSum);
                var response = (HttpWebResponse) request.GetResponse();
                var sr = new StreamReader(response.GetResponseStream());
                var ds = new DataSet("DataSet1");
                ds.ReadXml(sr);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                _objclsLog.Write(ex.Message);
                return null;
            }
        }

        #endregion

    }
}
