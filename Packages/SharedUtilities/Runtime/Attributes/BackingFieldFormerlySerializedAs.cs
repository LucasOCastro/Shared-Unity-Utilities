using SharedUtilities.Extensions;
using UnityEngine.Serialization;

namespace SharedUtilities.Attributes
{
    public class FormerlySerializedAsBackingFieldForAttribute : FormerlySerializedAsAttribute
    {
        public FormerlySerializedAsBackingFieldForAttribute(string propertyName) 
            : base(FieldUtils.GetBackingFieldName(propertyName))
        {
        }
    }
}