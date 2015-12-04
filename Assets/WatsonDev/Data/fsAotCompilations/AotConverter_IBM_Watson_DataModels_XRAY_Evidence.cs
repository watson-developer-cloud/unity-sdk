using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_XRAY_Evidence_DirectConverter Register_IBM_Watson_DataModels_XRAY_Evidence;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_XRAY_Evidence_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.XRAY.Evidence> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.XRAY.Evidence model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "title", model.title);
            result += SerializeMember(serialized, "passage", model.passage);
            result += SerializeMember(serialized, "decoratedPassage", model.decoratedPassage);
            result += SerializeMember(serialized, "corpus", model.corpus);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.XRAY.Evidence model) {
            var result = fsResult.Success;

            var t0 = model.title;
            result += DeserializeMember(data, "title", out t0);
            model.title = t0;

            var t1 = model.passage;
            result += DeserializeMember(data, "passage", out t1);
            model.passage = t1;

            var t2 = model.decoratedPassage;
            result += DeserializeMember(data, "decoratedPassage", out t2);
            model.decoratedPassage = t2;

            var t3 = model.corpus;
            result += DeserializeMember(data, "corpus", out t3);
            model.corpus = t3;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.XRAY.Evidence();
        }
    }
}
