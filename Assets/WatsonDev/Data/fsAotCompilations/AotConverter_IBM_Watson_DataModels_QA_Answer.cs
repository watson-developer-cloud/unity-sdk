using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_QA_Answer_DirectConverter Register_IBM_Watson_DataModels_QA_Answer;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_QA_Answer_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.QA.Answer> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.QA.Answer model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "id", model.id);
            result += SerializeMember(serialized, "text", model.text);
            result += SerializeMember(serialized, "pipeline", model.pipeline);
            result += SerializeMember(serialized, "formattedText", model.formattedText);
            result += SerializeMember(serialized, "confidence", model.confidence);
            result += SerializeMember(serialized, "evidence", model.evidence);
            result += SerializeMember(serialized, "entityTypes", model.entityTypes);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.QA.Answer model) {
            var result = fsResult.Success;

            var t0 = model.id;
            result += DeserializeMember(data, "id", out t0);
            model.id = t0;

            var t1 = model.text;
            result += DeserializeMember(data, "text", out t1);
            model.text = t1;

            var t2 = model.pipeline;
            result += DeserializeMember(data, "pipeline", out t2);
            model.pipeline = t2;

            var t3 = model.formattedText;
            result += DeserializeMember(data, "formattedText", out t3);
            model.formattedText = t3;

            var t4 = model.confidence;
            result += DeserializeMember(data, "confidence", out t4);
            model.confidence = t4;

            var t5 = model.evidence;
            result += DeserializeMember(data, "evidence", out t5);
            model.evidence = t5;

            var t6 = model.entityTypes;
            result += DeserializeMember(data, "entityTypes", out t6);
            model.entityTypes = t6;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.QA.Answer();
        }
    }
}
