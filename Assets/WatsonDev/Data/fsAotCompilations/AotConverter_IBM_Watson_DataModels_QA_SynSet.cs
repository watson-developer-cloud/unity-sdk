using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_QA_SynSet_DirectConverter Register_IBM_Watson_DataModels_QA_SynSet;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_QA_SynSet_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.QA.SynSet> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.QA.SynSet model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "name", model.name);
            result += SerializeMember(serialized, "synSet", model.synSet);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.QA.SynSet model) {
            var result = fsResult.Success;

            var t0 = model.name;
            result += DeserializeMember(data, "name", out t0);
            model.name = t0;

            var t1 = model.synSet;
            result += DeserializeMember(data, "synSet", out t1);
            model.synSet = t1;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.QA.SynSet();
        }
    }
}
