using System.Collections.Generic;
using System.Dynamic;

namespace RestaurantKata
{
    public class ExtensibleDynamicObject : DynamicObject
    {
        private readonly Dictionary<string, object> data;

        public ExtensibleDynamicObject()
        {
            data = new Dictionary<string, object>();
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            data[binder.Name] = value;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = data[binder.Name];
            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return data.Keys;
        }
    }
}