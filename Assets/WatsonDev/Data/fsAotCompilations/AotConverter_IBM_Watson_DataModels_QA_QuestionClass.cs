using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_QA_QuestionClass_DirectConverter Register_IBM_Watson_DataModels_QA_QuestionClass;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_QA_QuestionClass_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.QA.QuestionClass> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.QA.QuestionClass model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "out_of_domain", model.out_of_domain);
            result += SerializeMember(serialized, "question", model.question);
            result += SerializeMember(serialized, "domain", model.domain);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.QA.QuestionClass model) {
            var result = fsResult.Success;

            var t0 = model.out_of_domain;
            result += DeserializeMember(data, "out_of_domain", out t0);
            model.out_of_domain = t0;

            var t1 = model.question;
            result += DeserializeMember(data, "question", out t1);
            model.question = t1;

            var t2 = model.domain;
            result += DeserializeMember(data, "domain", out t2);
            model.domain = t2;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.QA.QuestionClass();
        }
    }
}
