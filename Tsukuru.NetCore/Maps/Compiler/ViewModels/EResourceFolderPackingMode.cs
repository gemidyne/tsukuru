using System.ComponentModel;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public enum EResourceFolderPackingMode
    {
        [Description("Pack all files")]
        Everything,

        [Description("Pack necessary assets only")]
        NecessaryAssetsOnly
    }
}