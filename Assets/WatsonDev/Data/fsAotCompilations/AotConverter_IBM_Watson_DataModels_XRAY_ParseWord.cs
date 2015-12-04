using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_XRAY_ParseWord_DirectConverter Register_IBM_Watson_DataModels_XRAY_ParseWord;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_XRAY_ParseWord_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.XRAY.ParseWord> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.XRAY.ParseWord model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "Word", model.Word);
            result += SerializeMember(serialized, "Pos", model.Pos);
            result += SerializeMember(serialized, "Slot", model.Slot);
            result += SerializeMember(serialized, "Features", model.Features);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.XRAY.ParseWord model) {
            var result = fsResult.Success;

            var t0 = model.Word;
            result += DeserializeMember(data, "Word", out t0);
            model.Word = t0;

            var t1 = model.Pos;
            result += DeserializeMember(data, "Pos", out t1);
            model.Pos = t1;

            var t2 = model.Slot;
            result += DeserializeMember(data, "Slot", out t2);
            model.Slot = t2;

            var t3 = model.Features;
            result += DeserializeMember(data, "Features", out t3);
            model.Features = t3;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.XRAY.ParseWord();
        }
    }
}
