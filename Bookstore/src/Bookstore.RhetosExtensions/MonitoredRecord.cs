using System.ComponentModel.Composition;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;

namespace Bookstore.RhetosExtensions
{
    [Export(typeof(IConceptInfo))]
    [ConceptKeyword("MonitoredRecord")]
    public class MonitoredRecordInfo : IConceptInfo
    {
        [ConceptKey]
        public EntityInfo Entity { get; set; }
    }

    [Export(typeof(IConceptMacro))]
    public class MonitoredRecordMacro : IConceptMacro<MonitoredRecordInfo>
    {
        public IEnumerable<IConceptInfo> CreateNewConcepts(MonitoredRecordInfo conceptInfo, IDslModel existingConcepts)
        {
            var newConcepts = new List<IConceptInfo>();

            var logging = new EntityLoggingInfo
            {
                Entity = conceptInfo.Entity
            };
            newConcepts.Add(logging);

            var createdAt = new DateTimePropertyInfo 
            { 
                DataStructure = conceptInfo.Entity,
                Name = "CreatedAt"
            };
            newConcepts.Add(createdAt);

            return newConcepts;
        }
    }
}
