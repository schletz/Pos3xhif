using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Application
{
    public record LogEntry(
        DateTime Timestamp,
        string Ip,
        string RequestType,
        string RequestUrl)
    {
        public override string ToString() => $"{Timestamp.ToString("O")};{Ip};{RequestType};{RequestUrl}";
    }    

}
