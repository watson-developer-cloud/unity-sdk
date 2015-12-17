using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_ConverseResponse_DirectConverter Register_IBM_Watson_DataModels_ConverseResponse;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_ConverseResponse_DirectConverter : fsDirectConverter<IBM.Watson.DeveloperCloud.DataModels.ConverseResponse> {
        protected override fsResult DoSerialize(IBM.Watson.DeveloperCloud.DataModels.ConverseResponse model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "response", model.response);
            result += SerializeMember(serialized, "input", model.input);
            result += SerializeMember(serialized, "conversation_id", model.conversation_id);
            result += SerializeMember(serialized, "confidence", model.confidence);
            result += SerializeMember(serialized, "client_id", model.client_id);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DeveloperCloud.DataModels.ConverseResponse model) {
            var result = fsResult.Success;

            var t0 = model.response;
            result += DeserializeMember(data, "response", out t0);
            model.response = t0;

            var t1 = model.input;
            result += DeserializeMember(data, "input", out t1);
            model.input = t1;

            var t2 = model.conversation_id;
            result += DeserializeMember(data, "conversation_id", out t2);
            model.conversation_id = t2;

            var t3 = model.confidence;
            result += DeserializeMember(data, "confidence", out t3);
            model.confidence = t3;

            var t4 = model.client_id;
            result += DeserializeMember(data, "client_id", out t4);
            model.client_id = t4;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DeveloperCloud.DataModels.ConverseResponse();
        }
    }
}
