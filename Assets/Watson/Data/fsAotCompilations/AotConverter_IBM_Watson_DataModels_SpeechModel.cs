using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_SpeechModel_DirectConverter Register_IBM_Watson_DataModels_SpeechModel;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_SpeechModel_DirectConverter : fsDirectConverter<IBM.Watson.DeveloperCloud.DataModels.SpeechModel> {
        protected override fsResult DoSerialize(IBM.Watson.DeveloperCloud.DataModels.SpeechModel model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "Name", model.Name);
            result += SerializeMember(serialized, "Rate", model.Rate);
            result += SerializeMember(serialized, "Language", model.Language);
            result += SerializeMember(serialized, "Description", model.Description);
            result += SerializeMember(serialized, "URL", model.URL);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DeveloperCloud.DataModels.SpeechModel model) {
            var result = fsResult.Success;

            var t0 = model.Name;
            result += DeserializeMember(data, "Name", out t0);
            model.Name = t0;

            var t1 = model.Rate;
            result += DeserializeMember(data, "Rate", out t1);
            model.Rate = t1;

            var t2 = model.Language;
            result += DeserializeMember(data, "Language", out t2);
            model.Language = t2;

            var t3 = model.Description;
            result += DeserializeMember(data, "Description", out t3);
            model.Description = t3;

            var t4 = model.URL;
            result += DeserializeMember(data, "URL", out t4);
            model.URL = t4;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DeveloperCloud.DataModels.SpeechModel();
        }
    }
}
