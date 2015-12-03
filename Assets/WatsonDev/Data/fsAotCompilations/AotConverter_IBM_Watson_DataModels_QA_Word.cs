using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_QA_Word_DirectConverter Register_IBM_Watson_DataModels_QA_Word;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_QA_Word_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.QA.Word> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.QA.Word model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

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

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.QA.Word model) {
            var result = fsResult.Success;

            var t0 = model.compSlotParseNodes;
            result += DeserializeMember(data, "compSlotParseNodes", out t0);
            model.compSlotParseNodes = t0;

            var t1 = model.slotname;
            result += DeserializeMember(data, "slotname", out t1);
            model.slotname = t1;

            var t2 = model.wordtext;
            result += DeserializeMember(data, "wordtext", out t2);
            model.wordtext = t2;

            var t3 = model.slotnameoptions;
            result += DeserializeMember(data, "slotnameoptions", out t3);
            model.slotnameoptions = t3;

            var t4 = model.wordsense;
            result += DeserializeMember(data, "wordsense", out t4);
            model.wordsense = t4;

            var t5 = model.numericsense;
            result += DeserializeMember(data, "numericsense", out t5);
            model.numericsense = t5;

            var t6 = model.seqno;
            result += DeserializeMember(data, "seqno", out t6);
            model.seqno = t6;

            var t7 = model.wordbegin;
            result += DeserializeMember(data, "wordbegin", out t7);
            model.wordbegin = t7;

            var t8 = model.framebegin;
            result += DeserializeMember(data, "framebegin", out t8);
            model.framebegin = t8;

            var t9 = model.frameend;
            result += DeserializeMember(data, "frameend", out t9);
            model.frameend = t9;

            var t10 = model.wordend;
            result += DeserializeMember(data, "wordend", out t10);
            model.wordend = t10;

            var t11 = model.features;
            result += DeserializeMember(data, "features", out t11);
            model.features = t11;

            var t12 = model.lmods;
            result += DeserializeMember(data, "lmods", out t12);
            model.lmods = t12;

            var t13 = model.rmods;
            result += DeserializeMember(data, "rmods", out t13);
            model.rmods = t13;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.QA.Word();
        }
    }
}
