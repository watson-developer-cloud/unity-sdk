using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_QA_ParseTree_DirectConverter Register_IBM_Watson_DataModels_QA_ParseTree;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_QA_ParseTree_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.QA.ParseTree> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.QA.ParseTree model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "parseScore", model.parseScore);
            result += SerializeMember(serialized, "compSlotParseNodes", model.compSlotParseNodes);
            result += SerializeMember(serialized, "slotname", model.slotname);
            result += SerializeMember(serialized, "wordtext", model.wordtext);
            result += SerializeMember(serialized, "slotnameoptions", model.slotnameoptions);
            result += SerializeMember(serialized, "wordsense", model.wordsense);
            result += SerializeMember(serialized, "numericsense", model.numericsense);
            result += SerializeMember(serialized, "seqno", model.seqno);
            result += SerializeMember(serialized, "wordbegin", model.wordbegin);
            result += SerializeMember(serialized, "framebegin", model.framebegin);
            result += SerializeMember(serialized, "frameend", model.frameend);
            result += SerializeMember(serialized, "wordend", model.wordend);
            result += SerializeMember(serialized, "features", model.features);
            result += SerializeMember(serialized, "lmods", model.lmods);
            result += SerializeMember(serialized, "rmods", model.rmods);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.QA.ParseTree model) {
            var result = fsResult.Success;

            var t0 = model.parseScore;
            result += DeserializeMember(data, "parseScore", out t0);
            model.parseScore = t0;

            var t1 = model.compSlotParseNodes;
            result += DeserializeMember(data, "compSlotParseNodes", out t1);
            model.compSlotParseNodes = t1;

            var t2 = model.slotname;
            result += DeserializeMember(data, "slotname", out t2);
            model.slotname = t2;

            var t3 = model.wordtext;
            result += DeserializeMember(data, "wordtext", out t3);
            model.wordtext = t3;

            var t4 = model.slotnameoptions;
            result += DeserializeMember(data, "slotnameoptions", out t4);
            model.slotnameoptions = t4;

            var t5 = model.wordsense;
            result += DeserializeMember(data, "wordsense", out t5);
            model.wordsense = t5;

            var t6 = model.numericsense;
            result += DeserializeMember(data, "numericsense", out t6);
            model.numericsense = t6;

            var t7 = model.seqno;
            result += DeserializeMember(data, "seqno", out t7);
            model.seqno = t7;

            var t8 = model.wordbegin;
            result += DeserializeMember(data, "wordbegin", out t8);
            model.wordbegin = t8;

            var t9 = model.framebegin;
            result += DeserializeMember(data, "framebegin", out t9);
            model.framebegin = t9;

            var t10 = model.frameend;
            result += DeserializeMember(data, "frameend", out t10);
            model.frameend = t10;

            var t11 = model.wordend;
            result += DeserializeMember(data, "wordend", out t11);
            model.wordend = t11;

            var t12 = model.features;
            result += DeserializeMember(data, "features", out t12);
            model.features = t12;

            var t13 = model.lmods;
            result += DeserializeMember(data, "lmods", out t13);
            model.lmods = t13;

            var t14 = model.rmods;
            result += DeserializeMember(data, "rmods", out t14);
            model.rmods = t14;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.QA.ParseTree();
        }
    }
}
