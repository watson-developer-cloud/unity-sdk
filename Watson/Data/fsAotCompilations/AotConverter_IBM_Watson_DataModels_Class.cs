using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_Class_DirectConverter Register_IBM_Watson_DataModels_Class;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_Class_DirectConverter : fsDirectConverter<IBM.Watson.DeveloperCloud.DataModels.Class> {
        protected override fsResult DoSerialize(IBM.Watson.DeveloperCloud.DataModels.Class model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "confidence", model.confidence);
            result += SerializeMember(serialized, "class_name", model.class_name);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DeveloperCloud.DataModels.Class model) {
            var result = fsResult.Success;

            var t0 = model.confidence;
            result += DeserializeMember(data, "confidence", out t0);
            model.confidence = t0;

            var t1 = model.class_name;
            result += DeserializeMember(data, "class_name", out t1);
            model.class_name = t1;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DeveloperCloud.DataModels.Class();
        }
    }
}
