using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_XRAY_Parse_DirectConverter Register_IBM_Watson_DataModels_XRAY_Parse;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_XRAY_Parse_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.XRAY.Parse> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.XRAY.Parse model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "flags", model.flags);
            result += SerializeMember(serialized, "words", model.words);
            result += SerializeMember(serialized, "pos", model.pos);
            result += SerializeMember(serialized, "slot", model.slot);
            result += SerializeMember(serialized, "features", model.features);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.XRAY.Parse model) {
            var result = fsResult.Success;

            var t0 = model.flags;
            result += DeserializeMember(data, "flags", out t0);
            model.flags = t0;

            var t1 = model.words;
            result += DeserializeMember(data, "words", out t1);
            model.words = t1;

            var t2 = model.pos;
            result += DeserializeMember(data, "pos", out t2);
            model.pos = t2;

            var t3 = model.slot;
            result += DeserializeMember(data, "slot", out t3);
            model.slot = t3;

            var t4 = model.features;
            result += DeserializeMember(data, "features", out t4);
            model.features = t4;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.XRAY.Parse();
        }
    }
}
