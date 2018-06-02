using System;

namespace Tbus.Parser.NETStandard
{
    public class TbusParserException : Exception
    {
        public TbusParserException(string message) : base(message) { }
    }
}
