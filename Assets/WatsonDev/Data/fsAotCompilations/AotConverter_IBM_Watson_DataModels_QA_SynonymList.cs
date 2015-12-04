using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_QA_SynonymList_DirectConverter Register_IBM_Watson_DataModels_QA_SynonymList;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_QA_SynonymList_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.QA.SynonymList> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.QA.SynonymList model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "partOfSpeech", model.partOfSpeech);
            result += SerializeMember(serialized, "value", model.value);
            result += SerializeMember(serialized, "lemma", model.lemma);
            result += SerializeMember(serialized, "synSet", model.synSet);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.QA.SynonymList model) {
            var result = fsResult.Success;

            var t0 = model.partOfSpeech;
            result += DeserializeMember(data, "partOfSpeech", out t0);
            model.partOfSpeech = t0;

            var t1 = model.value;
            result += DeserializeMember(data, "value", out t1);
            model.value = t1;

            var t2 = model.lemma;
            result += DeserializeMember(data, "lemma", out t2);
            model.lemma = t2;

            var t3 = model.synSet;
            result += DeserializeMember(data, "synSet", out t3);
            model.synSet = t3;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.QA.SynonymList();
        }
    }
}
