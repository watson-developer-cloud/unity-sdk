using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_Language_DirectConverter Register_IBM_Watson_DataModels_Language;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_Language_DirectConverter : fsDirectConverter<IBM.Watson.DeveloperCloud.DataModels.Language> {
        protected override fsResult DoSerialize(IBM.Watson.DeveloperCloud.DataModels.Language model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "language", model.language);
            result += SerializeMember(serialized, "name", model.name);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DeveloperCloud.DataModels.Language model) {
            var result = fsResult.Success;

            var t0 = model.language;
            result += DeserializeMember(data, "language", out t0);
            model.language = t0;

            var t1 = model.name;
            result += DeserializeMember(data, "name", out t1);
            model.name = t1;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DeveloperCloud.DataModels.Language();
        }
    }
}
