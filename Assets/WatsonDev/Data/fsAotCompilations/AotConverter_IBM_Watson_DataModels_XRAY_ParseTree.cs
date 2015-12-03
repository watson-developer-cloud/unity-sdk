using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_XRAY_ParseTree_DirectConverter Register_IBM_Watson_DataModels_XRAY_ParseTree;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_XRAY_ParseTree_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.XRAY.ParseTree> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.XRAY.ParseTree model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "position", model.position);
            result += SerializeMember(serialized, "text", model.text);
            result += SerializeMember(serialized, "parentNode", model.parentNode);
            result += SerializeMember(serialized, "rightChildren", model.rightChildren);
            result += SerializeMember(serialized, "leftChildren", model.leftChildren);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.XRAY.ParseTree model) {
            var result = fsResult.Success;

            var t0 = model.position;
            result += DeserializeMember(data, "position", out t0);
            model.position = t0;

            var t1 = model.text;
            result += DeserializeMember(data, "text", out t1);
            model.text = t1;

            var t2 = model.parentNode;
            result += DeserializeMember(data, "parentNode", out t2);
            model.parentNode = t2;

            var t3 = model.rightChildren;
            result += DeserializeMember(data, "rightChildren", out t3);
            model.rightChildren = t3;

            var t4 = model.leftChildren;
            result += DeserializeMember(data, "leftChildren", out t4);
            model.leftChildren = t4;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.XRAY.ParseTree();
        }
    }
}
