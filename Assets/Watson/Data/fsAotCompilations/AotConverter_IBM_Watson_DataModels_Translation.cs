using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_Translation_DirectConverter Register_IBM_Watson_DataModels_Translation;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_Translation_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.Translation> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.Translation model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "translation", model.translation);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.Translation model) {
            var result = fsResult.Success;

            var t0 = model.translation;
            result += DeserializeMember(data, "translation", out t0);
            model.translation = t0;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.Translation();
        }
    }
}
