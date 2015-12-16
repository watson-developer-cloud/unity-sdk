using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_TranslationModels_DirectConverter Register_IBM_Watson_DataModels_TranslationModels;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_TranslationModels_DirectConverter : fsDirectConverter<IBM.Watson.DeveloperCloud.DataModels.TranslationModels> {
        protected override fsResult DoSerialize(IBM.Watson.DeveloperCloud.DataModels.TranslationModels model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "models", model.models);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DeveloperCloud.DataModels.TranslationModels model) {
            var result = fsResult.Success;

            var t0 = model.models;
            result += DeserializeMember(data, "models", out t0);
            model.models = t0;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DeveloperCloud.DataModels.TranslationModels();
        }
    }
}
