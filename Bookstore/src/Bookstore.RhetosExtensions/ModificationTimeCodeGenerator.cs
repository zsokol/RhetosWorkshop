using System.ComponentModel.Composition;
using Rhetos.Compiler;
using Rhetos.Dom.DefaultConcepts;
using Rhetos.Extensibility;

namespace Bookstore.RhetosExtension
{
    [Export(typeof(IConceptCodeGenerator))]
    [ExportMetadata(MefProvider.Implements, typeof(ModificationTimeInfo))]
    public class ModificationTimeCodeGenerator : IConceptCodeGenerator
    {
        public void GenerateCode(Rhetos.Dsl.IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            var info = (ModificationTimeInfo)conceptInfo;

            string snippet =
            $@"{{ 
                var now = _executionContext.SqlUtility.GetDatabaseTime(_executionContext.SqlExecuter);

                foreach (var updatedItem in updatedNew)
                        updatedItem.{info.Property.Name} = now;
            }}
            ";

            codeBuilder.InsertCode(snippet, WritableOrmDataStructureCodeGenerator.InitializationTag, info.Property.DataStructure);
        }
    }
}
