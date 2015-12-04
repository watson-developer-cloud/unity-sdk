using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_TranslationModel_DirectConverter Register_IBM_Watson_DataModels_TranslationModel;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_TranslationModel_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.TranslationModel> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.TranslationModel model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "model_id", model.model_id);
            result += SerializeMember(serialized, "name", model.name);
            result += SerializeMember(serialized, "source", model.source);
            result += SerializeMember(serialized, "target", model.target);
            result += SerializeMember(serialized, "base_model_id", model.base_model_id);
            result += SerializeMember(serialized, "domain", model.domain);
            result += SerializeMember(serialized, "customizable", model.customizable);
            result += SerializeMember(serialized, "default", model.@default);
            result += SerializeMember(serialized, "owner", model.owner);
            result += SerializeMember(serialized, "status", model.status);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.TranslationModel model) {
            var result = fsResult.Success;

            var t0 = model.model_id;
            result += DeserializeMember(data, "model_id", out t0);
            model.model_id = t0;

            var t1 = model.name;
            result += DeserializeMember(data, "name", out t1);
            model.name = t1;

            var t2 = model.source;
            result += DeserializeMember(data, "source", out t2);
            model.source = t2;

            var t3 = model.target;
            result += DeserializeMember(data, "target", out t3);
            model.target = t3;

            var t4 = model.base_model_id;
            result += DeserializeMember(data, "base_model_id", out t4);
            model.base_model_id = t4;

            var t5 = model.domain;
            result += DeserializeMember(data, "domain", out t5);
            model.domain = t5;

            var t6 = model.customizable;
            result += DeserializeMember(data, "customizable", out t6);
            model.customizable = t6;

            var t7 = model.@default;
            result += DeserializeMember(data, "default", out t7);
            model.@default = t7;

            var t8 = model.owner;
            result += DeserializeMember(data, "owner", out t8);
            model.owner = t8;

            var t9 = model.status;
            result += DeserializeMember(data, "status", out t9);
            model.status = t9;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.TranslationModel();
        }
    }
}
