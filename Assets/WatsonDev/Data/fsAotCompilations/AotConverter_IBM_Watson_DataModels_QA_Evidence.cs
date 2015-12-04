using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_QA_Evidence_DirectConverter Register_IBM_Watson_DataModels_QA_Evidence;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_QA_Evidence_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.QA.Evidence> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.QA.Evidence model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "value", model.value);
            result += SerializeMember(serialized, "text", model.text);
            result += SerializeMember(serialized, "id", model.id);
            result += SerializeMember(serialized, "title", model.title);
            result += SerializeMember(serialized, "document", model.document);
            result += SerializeMember(serialized, "copyright", model.copyright);
            result += SerializeMember(serialized, "termsOfUse", model.termsOfUse);
            result += SerializeMember(serialized, "metadataMap", model.metadataMap);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.QA.Evidence model) {
            var result = fsResult.Success;

            var t0 = model.value;
            result += DeserializeMember(data, "value", out t0);
            model.value = t0;

            var t1 = model.text;
            result += DeserializeMember(data, "text", out t1);
            model.text = t1;

            var t2 = model.id;
            result += DeserializeMember(data, "id", out t2);
            model.id = t2;

            var t3 = model.title;
            result += DeserializeMember(data, "title", out t3);
            model.title = t3;

            var t4 = model.document;
            result += DeserializeMember(data, "document", out t4);
            model.document = t4;

            var t5 = model.copyright;
            result += DeserializeMember(data, "copyright", out t5);
            model.copyright = t5;

            var t6 = model.termsOfUse;
            result += DeserializeMember(data, "termsOfUse", out t6);
            model.termsOfUse = t6;

            var t7 = model.metadataMap;
            result += DeserializeMember(data, "metadataMap", out t7);
            model.metadataMap = t7;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.QA.Evidence();
        }
    }
}
