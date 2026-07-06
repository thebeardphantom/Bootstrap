using BeardPhantom.Bootstrap.SourceGen;

namespace SourceGenTest
{
    [GenerateSingleton(SingletonAccess.OutMethod)]
    [GenerateLogger]
    public partial class Class1 { }
}