using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_QA_EvidenceRequest_DirectConverter Register_IBM_Watson_DataModels_QA_EvidenceRequest;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_QA_EvidenceRequest_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.QA.EvidenceRequest> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.QA.EvidenceRequest model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "items", model.items);
            result += SerializeMember(serialized, "profile", model.profile);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.QA.EvidenceRequest model) {
            var result = fsResult.Success;

            var t0 = model.items;
            result += DeserializeMember(data, "items", out t0);
            model.items = t0;

            var t1 = model.profile;
            result += DeserializeMember(data, "profile", out t1);
            model.profile = t1;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.QA.EvidenceRequest();
        }
    }
}
