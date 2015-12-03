using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_Translations_DirectConverter Register_IBM_Watson_DataModels_Translations;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_Translations_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.Translations> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.Translations model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "word_count", model.word_count);
            result += SerializeMember(serialized, "character_count", model.character_count);
            result += SerializeMember(serialized, "translations", model.translations);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.Translations model) {
            var result = fsResult.Success;

            var t0 = model.word_count;
            result += DeserializeMember(data, "word_count", out t0);
            model.word_count = t0;

            var t1 = model.character_count;
            result += DeserializeMember(data, "character_count", out t1);
            model.character_count = t1;

            var t2 = model.translations;
            result += DeserializeMember(data, "translations", out t2);
            model.translations = t2;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.Translations();
        }
    }
}
