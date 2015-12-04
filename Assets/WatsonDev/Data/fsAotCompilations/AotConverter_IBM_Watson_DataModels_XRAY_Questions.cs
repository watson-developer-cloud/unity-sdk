using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_XRAY_Questions_DirectConverter Register_IBM_Watson_DataModels_XRAY_Questions;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_XRAY_Questions_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.XRAY.Questions> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.XRAY.Questions model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "questions", model.questions);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.XRAY.Questions model) {
            var result = fsResult.Success;

            var t0 = model.questions;
            result += DeserializeMember(data, "questions", out t0);
            model.questions = t0;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.XRAY.Questions();
        }
    }
}
