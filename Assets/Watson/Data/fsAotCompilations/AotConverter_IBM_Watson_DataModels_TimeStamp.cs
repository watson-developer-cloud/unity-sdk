using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_TimeStamp_DirectConverter Register_IBM_Watson_DataModels_TimeStamp;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_TimeStamp_DirectConverter : fsDirectConverter<IBM.Watson.DeveloperCloud.DataModels.TimeStamp> {
        protected override fsResult DoSerialize(IBM.Watson.DeveloperCloud.DataModels.TimeStamp model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "Word", model.Word);
            result += SerializeMember(serialized, "Start", model.Start);
            result += SerializeMember(serialized, "End", model.End);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DeveloperCloud.DataModels.TimeStamp model) {
            var result = fsResult.Success;

            var t0 = model.Word;
            result += DeserializeMember(data, "Word", out t0);
            model.Word = t0;

            var t1 = model.Start;
            result += DeserializeMember(data, "Start", out t1);
            model.Start = t1;

            var t2 = model.End;
            result += DeserializeMember(data, "End", out t2);
            model.End = t2;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DeveloperCloud.DataModels.TimeStamp();
        }
    }
}
