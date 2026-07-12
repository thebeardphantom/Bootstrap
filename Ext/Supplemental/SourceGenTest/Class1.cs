using BeardPhantom.Bootstrap.SourceGen;

namespace SourceGenTest
{
    [GenerateSingleton(SingletonAccessors.OutMethod)]
    [GenerateLogger]
    public partial class Class1 { }
}