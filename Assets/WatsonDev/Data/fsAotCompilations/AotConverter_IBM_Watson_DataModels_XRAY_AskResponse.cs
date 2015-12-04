using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_XRAY_AskResponse_DirectConverter Register_IBM_Watson_DataModels_XRAY_AskResponse;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_XRAY_AskResponse_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.XRAY.AskResponse> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.XRAY.AskResponse model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "questions", model.questions);
            result += SerializeMember(serialized, "answers", model.answers);
            result += SerializeMember(serialized, "parseData", model.parseData);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.XRAY.AskResponse model) {
            var result = fsResult.Success;

            var t0 = model.questions;
            result += DeserializeMember(data, "questions", out t0);
            model.questions = t0;

            var t1 = model.answers;
            result += DeserializeMember(data, "answers", out t1);
            model.answers = t1;

            var t2 = model.parseData;
            result += DeserializeMember(data, "parseData", out t2);
            model.parseData = t2;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.XRAY.AskResponse();
        }
    }
}
