using System;
namespace Communication
{
    public interface IConnectParams
    {
        string Hostname { get; set; }
        string Nickname { get; set; }
        string Passkey { get; set; }
        int Port { get; set; }
    }
}
