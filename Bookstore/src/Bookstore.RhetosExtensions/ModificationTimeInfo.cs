using System.ComponentModel.Composition;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;

namespace Bookstore.RhetosExtension
{
    // <summary>
    /// Automatically enters time when the records was created.
    /// </summary>
    //[Export(typeof(IConceptInfo))]
    [ConceptKeyword("ModificationTime")]
    public class ModificationTimeInfo
    {
        [ConceptKey]
        public DateTimePropertyInfo Property { get; set; }
    }
}
