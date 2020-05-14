using System;
using Aggregail.Newtonsoft.Json;

namespace EventSourcing.Demo.Cases
{
    public sealed class CaseIdConverter : ValueObjectConverter<CaseId, Guid>
    {
        public CaseIdConverter()
            : base(value => new CaseId(value))
        {
        }
    }
}