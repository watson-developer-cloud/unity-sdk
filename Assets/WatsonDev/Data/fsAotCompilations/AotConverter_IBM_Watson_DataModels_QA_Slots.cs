using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_QA_Slots_DirectConverter Register_IBM_Watson_DataModels_QA_Slots;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_QA_Slots_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.QA.Slots> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.QA.Slots model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "pred", model.pred);
            result += SerializeMember(serialized, "subj", model.subj);
            result += SerializeMember(serialized, "objprep", model.objprep);
            result += SerializeMember(serialized, "psubj", model.psubj);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.QA.Slots model) {
            var result = fsResult.Success;

            var t0 = model.pred;
            result += DeserializeMember(data, "pred", out t0);
            model.pred = t0;

            var t1 = model.subj;
            result += DeserializeMember(data, "subj", out t1);
            model.subj = t1;

            var t2 = model.objprep;
            result += DeserializeMember(data, "objprep", out t2);
            model.objprep = t2;

            var t3 = model.psubj;
            result += DeserializeMember(data, "psubj", out t3);
            model.psubj = t3;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.QA.Slots();
        }
    }
}
