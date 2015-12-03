using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_XRAY_Table_DirectConverter Register_IBM_Watson_DataModels_XRAY_Table;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_XRAY_Table_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.XRAY.Table> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.XRAY.Table model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "rows", model.rows);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.XRAY.Table model) {
            var result = fsResult.Success;

            var t0 = model.rows;
            result += DeserializeMember(data, "rows", out t0);
            model.rows = t0;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.XRAY.Table();
        }
    }
}
