using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Extensions
{
    public static class StreamExtensions
    {
        public static void Reset(this Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }
        }
    }
}
