using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_Classifiers_DirectConverter Register_IBM_Watson_DataModels_Classifiers;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_Classifiers_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.Classifiers> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.Classifiers model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "classifiers", model.classifiers);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.Classifiers model) {
            var result = fsResult.Success;

            var t0 = model.classifiers;
            result += DeserializeMember(data, "classifiers", out t0);
            model.classifiers = t0;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.Classifiers();
        }
    }
}
