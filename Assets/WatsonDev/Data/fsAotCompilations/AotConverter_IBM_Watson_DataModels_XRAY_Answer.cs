using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_XRAY_Answer_DirectConverter Register_IBM_Watson_DataModels_XRAY_Answer;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_XRAY_Answer_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.XRAY.Answer> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.XRAY.Answer model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "answerText", model.answerText);
            result += SerializeMember(serialized, "confidence", model.confidence);
            result += SerializeMember(serialized, "correctAnswer", model.correctAnswer);
            result += SerializeMember(serialized, "evidence", model.evidence);
            result += SerializeMember(serialized, "variants", model.variants);
            result += SerializeMember(serialized, "features", model.features);
            result += SerializeMember(serialized, "tables", model.tables);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.XRAY.Answer model) {
            var result = fsResult.Success;

            var t0 = model.answerText;
            result += DeserializeMember(data, "answerText", out t0);
            model.answerText = t0;

            var t1 = model.confidence;
            result += DeserializeMember(data, "confidence", out t1);
            model.confidence = t1;

            var t2 = model.correctAnswer;
            result += DeserializeMember(data, "correctAnswer", out t2);
            model.correctAnswer = t2;

            var t3 = model.evidence;
            result += DeserializeMember(data, "evidence", out t3);
            model.evidence = t3;

            var t4 = model.variants;
            result += DeserializeMember(data, "variants", out t4);
            model.variants = t4;

            var t5 = model.features;
            result += DeserializeMember(data, "features", out t5);
            model.features = t5;

            var t6 = model.tables;
            result += DeserializeMember(data, "tables", out t6);
            model.tables = t6;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.XRAY.Answer();
        }
    }
}
