using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_Dialogs_DirectConverter Register_IBM_Watson_DataModels_Dialogs;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_Dialogs_DirectConverter : fsDirectConverter<IBM.Watson.DeveloperCloud.DataModels.Dialogs> {
        protected override fsResult DoSerialize(IBM.Watson.DeveloperCloud.DataModels.Dialogs model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "dialogs", model.dialogs);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DeveloperCloud.DataModels.Dialogs model) {
            var result = fsResult.Success;

            var t0 = model.dialogs;
            result += DeserializeMember(data, "dialogs", out t0);
            model.dialogs = t0;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DeveloperCloud.DataModels.Dialogs();
        }
    }
}
