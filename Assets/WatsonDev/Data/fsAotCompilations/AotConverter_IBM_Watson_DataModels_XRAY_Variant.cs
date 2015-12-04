using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_XRAY_Variant_DirectConverter Register_IBM_Watson_DataModels_XRAY_Variant;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_XRAY_Variant_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.XRAY.Variant> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.XRAY.Variant model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "text", model.text);
            result += SerializeMember(serialized, "relationship", model.relationship);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.XRAY.Variant model) {
            var result = fsResult.Success;

            var t0 = model.text;
            result += DeserializeMember(data, "text", out t0);
            model.text = t0;

            var t1 = model.relationship;
            result += DeserializeMember(data, "relationship", out t1);
            model.relationship = t1;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.XRAY.Variant();
        }
    }
}
