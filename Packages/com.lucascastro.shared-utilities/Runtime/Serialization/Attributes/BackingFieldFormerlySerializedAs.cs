using SharedUtilities.Extensions;
using UnityEngine.Serialization;

namespace SharedUtilities.Serialization.Attributes
{
    public class FormerlySerializedAsBackingFieldForAttribute : FormerlySerializedAsAttribute
    {
        public FormerlySerializedAsBackingFieldForAttribute(string propertyName) 
            : base(FieldUtils.GetBackingFieldName(propertyName))
        {
        }
    }
}