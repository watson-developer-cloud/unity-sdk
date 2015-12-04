using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_QA_MetaDataMap_DirectConverter Register_IBM_Watson_DataModels_QA_MetaDataMap;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_QA_MetaDataMap_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.QA.MetaDataMap> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.QA.MetaDataMap model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "originalFile", model.originalFile);
            result += SerializeMember(serialized, "title", model.title);
            result += SerializeMember(serialized, "corpusName", model.corpusName);
            result += SerializeMember(serialized, "fileName", model.fileName);
            result += SerializeMember(serialized, "DOCNO", model.DOCNO);
            result += SerializeMember(serialized, "CorpusPlusDocno", model.CorpusPlusDocno);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.QA.MetaDataMap model) {
            var result = fsResult.Success;

            var t0 = model.originalFile;
            result += DeserializeMember(data, "originalFile", out t0);
            model.originalFile = t0;

            var t1 = model.title;
            result += DeserializeMember(data, "title", out t1);
            model.title = t1;

            var t2 = model.corpusName;
            result += DeserializeMember(data, "corpusName", out t2);
            model.corpusName = t2;

            var t3 = model.fileName;
            result += DeserializeMember(data, "fileName", out t3);
            model.fileName = t3;

            var t4 = model.DOCNO;
            result += DeserializeMember(data, "DOCNO", out t4);
            model.DOCNO = t4;

            var t5 = model.CorpusPlusDocno;
            result += DeserializeMember(data, "CorpusPlusDocno", out t5);
            model.CorpusPlusDocno = t5;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.QA.MetaDataMap();
        }
    }
}
