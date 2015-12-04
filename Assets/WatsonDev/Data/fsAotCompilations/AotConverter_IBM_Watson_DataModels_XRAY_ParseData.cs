using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_XRAY_ParseData_DirectConverter Register_IBM_Watson_DataModels_XRAY_ParseData;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_XRAY_ParseData_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.XRAY.ParseData> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.XRAY.ParseData model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "Words", model.Words);
            result += SerializeMember(serialized, "parse", model.parse);
            result += SerializeMember(serialized, "parseTree", model.parseTree);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.XRAY.ParseData model) {
            var result = fsResult.Success;

            var t0 = model.Words;
            result += DeserializeMember(data, "Words", out t0);
            model.Words = t0;

            var t1 = model.parse;
            result += DeserializeMember(data, "parse", out t1);
            model.parse = t1;

            var t2 = model.parseTree;
            result += DeserializeMember(data, "parseTree", out t2);
            model.parseTree = t2;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.XRAY.ParseData();
        }
    }
}
