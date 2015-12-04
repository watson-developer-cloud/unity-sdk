using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_QA_Value_DirectConverter Register_IBM_Watson_DataModels_QA_Value;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_QA_Value_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.QA.Value> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.QA.Value model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "value", model.value);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.QA.Value model) {
            var result = fsResult.Success;

            var t0 = model.value;
            result += DeserializeMember(data, "value", out t0);
            model.value = t0;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.QA.Value();
        }
    }
}
