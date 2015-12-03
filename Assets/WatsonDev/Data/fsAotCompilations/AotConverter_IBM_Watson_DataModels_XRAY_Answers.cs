using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_XRAY_Answers_DirectConverter Register_IBM_Watson_DataModels_XRAY_Answers;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_XRAY_Answers_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.XRAY.Answers> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.XRAY.Answers model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "_id", model._id);
            result += SerializeMember(serialized, "_rev", model._rev);
            result += SerializeMember(serialized, "transactionId", model.transactionId);
            result += SerializeMember(serialized, "featureScoreMin", model.featureScoreMin);
            result += SerializeMember(serialized, "featureScoreMax", model.featureScoreMax);
            result += SerializeMember(serialized, "featureScoreRange", model.featureScoreRange);
            result += SerializeMember(serialized, "answers", model.answers);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.XRAY.Answers model) {
            var result = fsResult.Success;

            var t0 = model._id;
            result += DeserializeMember(data, "_id", out t0);
            model._id = t0;

            var t1 = model._rev;
            result += DeserializeMember(data, "_rev", out t1);
            model._rev = t1;

            var t2 = model.transactionId;
            result += DeserializeMember(data, "transactionId", out t2);
            model.transactionId = t2;

            var t3 = model.featureScoreMin;
            result += DeserializeMember(data, "featureScoreMin", out t3);
            model.featureScoreMin = t3;

            var t4 = model.featureScoreMax;
            result += DeserializeMember(data, "featureScoreMax", out t4);
            model.featureScoreMax = t4;

            var t5 = model.featureScoreRange;
            result += DeserializeMember(data, "featureScoreRange", out t5);
            model.featureScoreRange = t5;

            var t6 = model.answers;
            result += DeserializeMember(data, "answers", out t6);
            model.answers = t6;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.XRAY.Answers();
        }
    }
}
