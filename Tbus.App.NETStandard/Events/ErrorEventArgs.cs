using System;

namespace Tbus.App.NETStandard.Events
{
    public class ErrorEventArgs : EventArgs
    {
        public string Message { get; }

        public ErrorEventArgs(string message)
        {
            Message = message;
        }
    }
}
