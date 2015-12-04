using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_QA_Synonym_DirectConverter Register_IBM_Watson_DataModels_QA_Synonym;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_QA_Synonym_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.QA.Synonym> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.QA.Synonym model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "isChosen", model.isChosen);
            result += SerializeMember(serialized, "value", model.value);
            result += SerializeMember(serialized, "weight", model.weight);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.QA.Synonym model) {
            var result = fsResult.Success;

            var t0 = model.isChosen;
            result += DeserializeMember(data, "isChosen", out t0);
            model.isChosen = t0;

            var t1 = model.value;
            result += DeserializeMember(data, "value", out t1);
            model.value = t1;

            var t2 = model.weight;
            result += DeserializeMember(data, "weight", out t2);
            model.weight = t2;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.QA.Synonym();
        }
    }
}
