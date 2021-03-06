using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalytics_CustomDataProvider
{
    static class CDPHelper
    {
        /// <summary>
        /// Convert int to Guid
        /// </summary>
        /// <param name="value">the int value</param>
        /// <returns>the Guid value</returns>
        public static Guid IntToGuid(int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        /// <summary>
        /// Convert Guid to int
        /// </summary>
        /// <param name="value">the Guid value</param>
        /// <returns>the int value</returns>
        public static int GuidToInt(Guid value)
        {
            byte[] b = value.ToByteArray();
            int bint = BitConverter.ToInt32(b, 0);
            return bint;
        }
    }
}
