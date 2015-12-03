using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_DialogEntry_DirectConverter Register_IBM_Watson_DataModels_DialogEntry;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_DialogEntry_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.DialogEntry> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.DialogEntry model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "dialog_id", model.dialog_id);
            result += SerializeMember(serialized, "name", model.name);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.DialogEntry model) {
            var result = fsResult.Success;

            var t0 = model.dialog_id;
            result += DeserializeMember(data, "dialog_id", out t0);
            model.dialog_id = t0;

            var t1 = model.name;
            result += DeserializeMember(data, "name", out t1);
            model.name = t1;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.DialogEntry();
        }
    }
}
