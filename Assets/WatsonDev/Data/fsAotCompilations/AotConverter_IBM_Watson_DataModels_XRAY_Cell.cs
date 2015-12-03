using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_XRAY_Cell_DirectConverter Register_IBM_Watson_DataModels_XRAY_Cell;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_XRAY_Cell_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.XRAY.Cell> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.XRAY.Cell model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "Value", model.Value);
            result += SerializeMember(serialized, "ColSpan", model.ColSpan);
            result += SerializeMember(serialized, "Highlighted", model.Highlighted);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.XRAY.Cell model) {
            var result = fsResult.Success;

            var t0 = model.Value;
            result += DeserializeMember(data, "Value", out t0);
            model.Value = t0;

            var t1 = model.ColSpan;
            result += DeserializeMember(data, "ColSpan", out t1);
            model.ColSpan = t1;

            var t2 = model.Highlighted;
            result += DeserializeMember(data, "Highlighted", out t2);
            model.Highlighted = t2;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.XRAY.Cell();
        }
    }
}
