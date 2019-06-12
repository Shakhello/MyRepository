using CMS.DAL.Models;
using System;
using System.Collections.Generic;

namespace CMS.DAL.Behaviours
{
    internal class BehaviourSelector
    {
        public static readonly Dictionary<FieldType, Func<ICRUDBehaviour<Field>>> FieldBehaviours =
            new Dictionary<FieldType, Func<ICRUDBehaviour<Field>>>()
            {
                { FieldType.DateTime, () => new FieldDefaultBehaviour() },
                { FieldType.Decimal, () => new FieldDefaultBehaviour() },
                { FieldType.Dictionary, () => new FieldDictionaryBehaviour() },
                { FieldType.Double, () => new FieldDefaultBehaviour() },
                { FieldType.File, () => new FieldFileBehaviour() },
                { FieldType.Flag, () => new FieldDefaultBehaviour() },
                { FieldType.Guid, () => new FieldDefaultBehaviour() },
                { FieldType.Integer, () => new FieldDefaultBehaviour() },
                { FieldType.Reference, () => new FieldReferenceBehaviour() },
                { FieldType.Text, () => new FieldDefaultBehaviour() }
            };
    }
}
