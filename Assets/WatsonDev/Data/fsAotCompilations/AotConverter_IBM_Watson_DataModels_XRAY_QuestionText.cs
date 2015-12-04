using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_XRAY_QuestionText_DirectConverter Register_IBM_Watson_DataModels_XRAY_QuestionText;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_XRAY_QuestionText_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.XRAY.QuestionText> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.XRAY.QuestionText model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "focus", model.focus);
            result += SerializeMember(serialized, "lat", model.lat);
            result += SerializeMember(serialized, "questionText", model.questionText);
            result += SerializeMember(serialized, "taggedText", model.taggedText);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.XRAY.QuestionText model) {
            var result = fsResult.Success;

            var t0 = model.focus;
            result += DeserializeMember(data, "focus", out t0);
            model.focus = t0;

            var t1 = model.lat;
            result += DeserializeMember(data, "lat", out t1);
            model.lat = t1;

            var t2 = model.questionText;
            result += DeserializeMember(data, "questionText", out t2);
            model.questionText = t2;

            var t3 = model.taggedText;
            result += DeserializeMember(data, "taggedText", out t3);
            model.taggedText = t3;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.XRAY.QuestionText();
        }
    }
}
