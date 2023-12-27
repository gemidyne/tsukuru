using Tsukuru.Maps.Compiler.ViewModels;

namespace Tsukuru.Maps.Compiler.Messages;

public class RemoveResourceFolderFromPackingMessage
{
    public ResourceFolderViewModel Folder { get; set; }
}