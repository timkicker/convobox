using SharedDefinitions;

namespace Convobox.Server;

public class ServerInfo
{
    public string Domain { get; set; } = Definition.DefaultDomain;
    public int PortCommunication { get; set; } = Definition.DefaultPortCommunication;
    public int PortFiles { get; set; } = Definition.DefaultPortFiles;
}