using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_XRAY_Question_DirectConverter Register_IBM_Watson_DataModels_XRAY_Question;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_XRAY_Question_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.XRAY.Question> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.XRAY.Question model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "_id", model._id);
            result += SerializeMember(serialized, "topConfidence", model.topConfidence);
            result += SerializeMember(serialized, "questionId", model.questionId);
            result += SerializeMember(serialized, "question", model.question);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.XRAY.Question model) {
            var result = fsResult.Success;

            var t0 = model._id;
            result += DeserializeMember(data, "_id", out t0);
            model._id = t0;

            var t1 = model.topConfidence;
            result += DeserializeMember(data, "topConfidence", out t1);
            model.topConfidence = t1;

            var t2 = model.questionId;
            result += DeserializeMember(data, "questionId", out t2);
            model.questionId = t2;

            var t3 = model.question;
            result += DeserializeMember(data, "question", out t3);
            model.question = t3;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.XRAY.Question();
        }
    }
}
