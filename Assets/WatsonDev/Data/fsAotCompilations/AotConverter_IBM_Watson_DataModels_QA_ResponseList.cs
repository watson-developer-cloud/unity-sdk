using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_QA_ResponseList_DirectConverter Register_IBM_Watson_DataModels_QA_ResponseList;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_QA_ResponseList_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.QA.ResponseList> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.QA.ResponseList model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "responses", model.responses);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.QA.ResponseList model) {
            var result = fsResult.Success;

            var t0 = model.responses;
            result += DeserializeMember(data, "responses", out t0);
            model.responses = t0;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.QA.ResponseList();
        }
    }
}
