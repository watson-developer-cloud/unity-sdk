using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_XRAY_Value_DirectConverter Register_IBM_Watson_DataModels_XRAY_Value;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_XRAY_Value_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.XRAY.Value> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.XRAY.Value model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "text", model.text);
            result += SerializeMember(serialized, "value", model.value);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.XRAY.Value model) {
            var result = fsResult.Success;

            var t0 = model.text;
            result += DeserializeMember(data, "text", out t0);
            model.text = t0;

            var t1 = model.value;
            result += DeserializeMember(data, "value", out t1);
            model.value = t1;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.XRAY.Value();
        }
    }
}
