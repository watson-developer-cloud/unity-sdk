using System;
using System.Collections.Generic;

namespace FullSerializer {
    partial class fsConverterRegistrar {
        public static Speedup.IBM_Watson_DataModels_QA_Question_DirectConverter Register_IBM_Watson_DataModels_QA_Question;
    }
}

namespace FullSerializer.Speedup {
    public class IBM_Watson_DataModels_QA_Question_DirectConverter : fsDirectConverter<IBM.Watson.DataModels.QA.Question> {
        protected override fsResult DoSerialize(IBM.Watson.DataModels.QA.Question model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, "qclasslist", model.qclasslist);
            result += SerializeMember(serialized, "focuslist", model.focuslist);
            result += SerializeMember(serialized, "latlist", model.latlist);
            result += SerializeMember(serialized, "evidencelist", model.evidencelist);
            result += SerializeMember(serialized, "synonymList", model.synonymList);
            result += SerializeMember(serialized, "disambiguatedEntities", model.disambiguatedEntities);
            result += SerializeMember(serialized, "xsgtopparses", model.xsgtopparses);
            result += SerializeMember(serialized, "casXml", model.casXml);
            result += SerializeMember(serialized, "pipelineid", model.pipelineid);
            result += SerializeMember(serialized, "formattedAnswer", model.formattedAnswer);
            result += SerializeMember(serialized, "selectedProcessingComponents", model.selectedProcessingComponents);
            result += SerializeMember(serialized, "category", model.category);
            result += SerializeMember(serialized, "items", model.items);
            result += SerializeMember(serialized, "status", model.status);
            result += SerializeMember(serialized, "id", model.id);
            result += SerializeMember(serialized, "questionText", model.questionText);
            result += SerializeMember(serialized, "evidenceRequest", model.evidenceRequest);
            result += SerializeMember(serialized, "answers", model.answers);
            result += SerializeMember(serialized, "errorNotifications", model.errorNotifications);
            result += SerializeMember(serialized, "passthru", model.passthru);
            result += SerializeMember(serialized, "questionId", model.questionId);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref IBM.Watson.DataModels.QA.Question model) {
            var result = fsResult.Success;

            var t0 = model.qclasslist;
            result += DeserializeMember(data, "qclasslist", out t0);
            model.qclasslist = t0;

            var t1 = model.focuslist;
            result += DeserializeMember(data, "focuslist", out t1);
            model.focuslist = t1;

            var t2 = model.latlist;
            result += DeserializeMember(data, "latlist", out t2);
            model.latlist = t2;

            var t3 = model.evidencelist;
            result += DeserializeMember(data, "evidencelist", out t3);
            model.evidencelist = t3;

            var t4 = model.synonymList;
            result += DeserializeMember(data, "synonymList", out t4);
            model.synonymList = t4;

            var t5 = model.disambiguatedEntities;
            result += DeserializeMember(data, "disambiguatedEntities", out t5);
            model.disambiguatedEntities = t5;

            var t6 = model.xsgtopparses;
            result += DeserializeMember(data, "xsgtopparses", out t6);
            model.xsgtopparses = t6;

            var t7 = model.casXml;
            result += DeserializeMember(data, "casXml", out t7);
            model.casXml = t7;

            var t8 = model.pipelineid;
            result += DeserializeMember(data, "pipelineid", out t8);
            model.pipelineid = t8;

            var t9 = model.formattedAnswer;
            result += DeserializeMember(data, "formattedAnswer", out t9);
            model.formattedAnswer = t9;

            var t10 = model.selectedProcessingComponents;
            result += DeserializeMember(data, "selectedProcessingComponents", out t10);
            model.selectedProcessingComponents = t10;

            var t11 = model.category;
            result += DeserializeMember(data, "category", out t11);
            model.category = t11;

            var t12 = model.items;
            result += DeserializeMember(data, "items", out t12);
            model.items = t12;

            var t13 = model.status;
            result += DeserializeMember(data, "status", out t13);
            model.status = t13;

            var t14 = model.id;
            result += DeserializeMember(data, "id", out t14);
            model.id = t14;

            var t15 = model.questionText;
            result += DeserializeMember(data, "questionText", out t15);
            model.questionText = t15;

            var t16 = model.evidenceRequest;
            result += DeserializeMember(data, "evidenceRequest", out t16);
            model.evidenceRequest = t16;

            var t17 = model.answers;
            result += DeserializeMember(data, "answers", out t17);
            model.answers = t17;

            var t18 = model.errorNotifications;
            result += DeserializeMember(data, "errorNotifications", out t18);
            model.errorNotifications = t18;

            var t19 = model.passthru;
            result += DeserializeMember(data, "passthru", out t19);
            model.passthru = t19;

            var t20 = model.questionId;
            result += DeserializeMember(data, "questionId", out t20);
            model.questionId = t20;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return new IBM.Watson.DataModels.QA.Question();
        }
    }
}
