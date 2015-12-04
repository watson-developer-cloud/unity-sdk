using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_Classifier_DirectConverter Register_IBM_Watson_DataModels_Classifier;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_Classifier_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.Classifier> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.Classifier model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "name", model.name);
            result += SerializeMember(serialized, "language", model.language);
            result += SerializeMember(serialized, "url", model.url);
            result += SerializeMember(serialized, "classifier_id", model.classifier_id);
            result += SerializeMember(serialized, "created", model.created);
            result += SerializeMember(serialized, "status", model.status);
            result += SerializeMember(serialized, "status_description", model.status_description);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.Classifier model) {
            var result = fsResult.Success;

            var t0 = model.name;
            result += DeserializeMember(data, "name", out t0);
            model.name = t0;

            var t1 = model.language;
            result += DeserializeMember(data, "language", out t1);
            model.language = t1;

            var t2 = model.url;
            result += DeserializeMember(data, "url", out t2);
            model.url = t2;

            var t3 = model.classifier_id;
            result += DeserializeMember(data, "classifier_id", out t3);
            model.classifier_id = t3;

            var t4 = model.created;
            result += DeserializeMember(data, "created", out t4);
            model.created = t4;

            var t5 = model.status;
            result += DeserializeMember(data, "status", out t5);
            model.status = t5;

            var t6 = model.status_description;
            result += DeserializeMember(data, "status_description", out t6);
            model.status_description = t6;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.Classifier();
        }
    }
}
