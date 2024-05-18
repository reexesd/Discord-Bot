using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Logger
{
    public interface ITimeService
    {
        public DateTime LocalTime { get; }

        public DateTime HomeTime { get; }
    }
}
