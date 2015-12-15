using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_Languages_DirectConverter Register_IBM_Watson_DataModels_Languages;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_Languages_DirectConverter : fsDirectConverter<IBM.Watson.DeveloperCloud.DataModels.Languages> {
        protected override fsResult DoSerialize(IBM.Watson.DeveloperCloud.DataModels.Languages model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "languages", model.languages);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DeveloperCloud.DataModels.Languages model) {
            var result = fsResult.Success;

            var t0 = model.languages;
            result += DeserializeMember(data, "languages", out t0);
            model.languages = t0;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DeveloperCloud.DataModels.Languages();
        }
    }
}
