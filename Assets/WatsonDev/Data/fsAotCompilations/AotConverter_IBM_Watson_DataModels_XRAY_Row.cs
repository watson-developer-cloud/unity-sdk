using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_XRAY_Row_DirectConverter Register_IBM_Watson_DataModels_XRAY_Row;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_XRAY_Row_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.XRAY.Row> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.XRAY.Row model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "columns", model.columns);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.XRAY.Row model) {
            var result = fsResult.Success;

            var t0 = model.columns;
            result += DeserializeMember(data, "columns", out t0);
            model.columns = t0;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.XRAY.Row();
        }
    }
}
