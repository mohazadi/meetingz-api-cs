using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Meetingz
{
    public class ClsData
    {
        #region "getSha1"

        /// <summary>
        /// Returns the SHA-1 Value for the InputString
        /// </summary>
        /// <param name="StrValue"></param>
        /// <returns></returns>
        public static string GetSha1(string StrValue)
        {
            var md = new HashFx();
            return md.EncryptString(StrValue, 1);
        }
        #endregion
    }
}
