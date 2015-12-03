using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_XRAY_Feature_DirectConverter Register_IBM_Watson_DataModels_XRAY_Feature;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_XRAY_Feature_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.XRAY.Feature> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.XRAY.Feature model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "featureId", model.featureId);
            result += SerializeMember(serialized, "label", model.label);
            result += SerializeMember(serialized, "displayLabel", model.displayLabel);
            result += SerializeMember(serialized, "unweightedScore", model.unweightedScore);
            result += SerializeMember(serialized, "weightedScore", model.weightedScore);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.XRAY.Feature model) {
            var result = fsResult.Success;

            var t0 = model.featureId;
            result += DeserializeMember(data, "featureId", out t0);
            model.featureId = t0;

            var t1 = model.label;
            result += DeserializeMember(data, "label", out t1);
            model.label = t1;

            var t2 = model.displayLabel;
            result += DeserializeMember(data, "displayLabel", out t2);
            model.displayLabel = t2;

            var t3 = model.unweightedScore;
            result += DeserializeMember(data, "unweightedScore", out t3);
            model.unweightedScore = t3;

            var t4 = model.weightedScore;
            result += DeserializeMember(data, "weightedScore", out t4);
            model.weightedScore = t4;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.XRAY.Feature();
        }
    }
}
