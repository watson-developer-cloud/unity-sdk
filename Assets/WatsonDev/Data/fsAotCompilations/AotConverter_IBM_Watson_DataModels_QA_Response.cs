using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_QA_Response_DirectConverter Register_IBM_Watson_DataModels_QA_Response;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_QA_Response_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.QA.Response> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.QA.Response model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "question", model.question);
            result += SerializeMember(serialized, "questionClasses", model.questionClasses);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.QA.Response model) {
            var result = fsResult.Success;

            var t0 = model.question;
            result += DeserializeMember(data, "question", out t0);
            model.question = t0;

            var t1 = model.questionClasses;
            result += DeserializeMember(data, "questionClasses", out t1);
            model.questionClasses = t1;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.QA.Response();
        }
    }
}
