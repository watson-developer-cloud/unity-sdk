using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_ClassifyResult_DirectConverter Register_IBM_Watson_DataModels_ClassifyResult;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_ClassifyResult_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.ClassifyResult> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.ClassifyResult model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "classifier_id", model.classifier_id);
            result += SerializeMember(serialized, "url", model.url);
            result += SerializeMember(serialized, "text", model.text);
            result += SerializeMember(serialized, "top_class", model.top_class);
            result += SerializeMember(serialized, "classes", model.classes);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.ClassifyResult model) {
            var result = fsResult.Success;

            var t0 = model.classifier_id;
            result += DeserializeMember(data, "classifier_id", out t0);
            model.classifier_id = t0;

            var t1 = model.url;
            result += DeserializeMember(data, "url", out t1);
            model.url = t1;

            var t2 = model.text;
            result += DeserializeMember(data, "text", out t2);
            model.text = t2;

            var t3 = model.top_class;
            result += DeserializeMember(data, "top_class", out t3);
            model.top_class = t3;

            var t4 = model.classes;
            result += DeserializeMember(data, "classes", out t4);
            model.classes = t4;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.ClassifyResult();
        }
    }
}
