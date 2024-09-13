using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Enums
{
    public enum EventType
    {
        Wedding,
        Death
    }

    public static class EventTypeExtensions
    {
        public static string Code(this EventType type)
        {
            switch (type)
            {
                case EventType.Wedding:
                    return "M";
                case EventType.Death:
                    return "D";
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }
    }
}
