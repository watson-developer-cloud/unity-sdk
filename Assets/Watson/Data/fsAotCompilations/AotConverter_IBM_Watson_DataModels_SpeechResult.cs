using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_SpeechResult_DirectConverter Register_IBM_Watson_DataModels_SpeechResult;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_SpeechResult_DirectConverter : fsDirectConverter<IBM.Watson.DeveloperCloud.DataModels.SpeechResult> {
        protected override fsResult DoSerialize(IBM.Watson.DeveloperCloud.DataModels.SpeechResult model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "Final", model.Final);
            result += SerializeMember(serialized, "Alternatives", model.Alternatives);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DeveloperCloud.DataModels.SpeechResult model) {
            var result = fsResult.Success;

            var t0 = model.Final;
            result += DeserializeMember(data, "Final", out t0);
            model.Final = t0;

            var t1 = model.Alternatives;
            result += DeserializeMember(data, "Alternatives", out t1);
            model.Alternatives = t1;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DeveloperCloud.DataModels.SpeechResult();
        }
    }
}
