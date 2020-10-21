using System;
using System.Data;
using Meetingz;

namespace ConsoleApplication1
{
    class Program
    {
       
 
        static void Main(string[] args)
        {
            var dt = new DataTable();
            var objMeetingz = new ClsMeetingz("https://api.meetingz.ir/test/", "9184e6b9-e778-45f5-9c69-d0dfb6246264");
            
            //Console.WriteLine(ClsData.getSha1("createname=Test+Meeting&meetingID=abc123&attendeePW=111222&moderatorPW=33344404f3591a48c820cebfe5096e6cffd0b3"));
            dt = objMeetingz.CreateMeeting("Mkalaiselvi", "a2b", "selvi", "kalai");
            dt = objMeetingz.IsMeetingRunning("a2b");
            
            objMeetingz.JoinMeeting("Mkalaiselvi", "a2b", "kalai", true);
            objMeetingz.JoinMeeting("Mkalaiselvi", "a2b", "selvi", true);
            
            dt =objMeetingz.GetMeetings();
            dt = objMeetingz.GetMeetingInfo("a2b", "kalai");          
            Console.WriteLine("End meeting?");         
            Console.ReadLine();         
            
            dt = objMeetingz.EndMeeting("a2b", "kalai");
            dt =objMeetingz.IsMeetingRunning("aaa");    
            Console.ReadLine();         
         
        }
       
    }
}

