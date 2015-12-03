using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_SpeechAlt_DirectConverter Register_IBM_Watson_DataModels_SpeechAlt;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_SpeechAlt_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.SpeechAlt> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.SpeechAlt model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "Transcript", model.Transcript);
            result += SerializeMember(serialized, "Confidence", model.Confidence);
            result += SerializeMember(serialized, "Timestamps", model.Timestamps);
            result += SerializeMember(serialized, "WordConfidence", model.WordConfidence);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.SpeechAlt model) {
            var result = fsResult.Success;

            var t0 = model.Transcript;
            result += DeserializeMember(data, "Transcript", out t0);
            model.Transcript = t0;

            var t1 = model.Confidence;
            result += DeserializeMember(data, "Confidence", out t1);
            model.Confidence = t1;

            var t2 = model.Timestamps;
            result += DeserializeMember(data, "Timestamps", out t2);
            model.Timestamps = t2;

            var t3 = model.WordConfidence;
            result += DeserializeMember(data, "WordConfidence", out t3);
            model.WordConfidence = t3;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.SpeechAlt();
        }
    }
}
