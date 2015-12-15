using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_WordConfidence_DirectConverter Register_IBM_Watson_DataModels_WordConfidence;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_WordConfidence_DirectConverter : fsDirectConverter<IBM.Watson.DeveloperCloud.DataModels.WordConfidence> {
        protected override fsResult DoSerialize(IBM.Watson.DeveloperCloud.DataModels.WordConfidence model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "Word", model.Word);
            result += SerializeMember(serialized, "Confidence", model.Confidence);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DeveloperCloud.DataModels.WordConfidence model) {
            var result = fsResult.Success;

            var t0 = model.Word;
            result += DeserializeMember(data, "Word", out t0);
            model.Word = t0;

            var t1 = model.Confidence;
            result += DeserializeMember(data, "Confidence", out t1);
            model.Confidence = t1;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DeveloperCloud.DataModels.WordConfidence();
        }
    }
}
