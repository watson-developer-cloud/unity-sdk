/**
* (C) Copyright IBM Corp. 2019, 2020.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IBM.Watson.CompareComply.V1.Model
{
    /// <summary>
    /// Information defining an element's subject matter.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// The category of the associated element.
        /// </summary>
        public class LabelValue
        {
            /// <summary>
            /// Constant AMENDMENTS for Amendments
            /// </summary>
            public const string AMENDMENTS = "Amendments";
            /// <summary>
            /// Constant ASSET_USE for Asset Use
            /// </summary>
            public const string ASSET_USE = "Asset Use";
            /// <summary>
            /// Constant ASSIGNMENTS for Assignments
            /// </summary>
            public const string ASSIGNMENTS = "Assignments";
            /// <summary>
            /// Constant AUDITS for Audits
            /// </summary>
            public const string AUDITS = "Audits";
            /// <summary>
            /// Constant BUSINESS_CONTINUITY for Business Continuity
            /// </summary>
            public const string BUSINESS_CONTINUITY = "Business Continuity";
            /// <summary>
            /// Constant COMMUNICATION for Communication
            /// </summary>
            public const string COMMUNICATION = "Communication";
            /// <summary>
            /// Constant CONFIDENTIALITY for Confidentiality
            /// </summary>
            public const string CONFIDENTIALITY = "Confidentiality";
            /// <summary>
            /// Constant DELIVERABLES for Deliverables
            /// </summary>
            public const string DELIVERABLES = "Deliverables";
            /// <summary>
            /// Constant DELIVERY for Delivery
            /// </summary>
            public const string DELIVERY = "Delivery";
            /// <summary>
            /// Constant DISPUTE_RESOLUTION for Dispute Resolution
            /// </summary>
            public const string DISPUTE_RESOLUTION = "Dispute Resolution";
            /// <summary>
            /// Constant FORCE_MAJEURE for Force Majeure
            /// </summary>
            public const string FORCE_MAJEURE = "Force Majeure";
            /// <summary>
            /// Constant INDEMNIFICATION for Indemnification
            /// </summary>
            public const string INDEMNIFICATION = "Indemnification";
            /// <summary>
            /// Constant INSURANCE for Insurance
            /// </summary>
            public const string INSURANCE = "Insurance";
            /// <summary>
            /// Constant INTELLECTUAL_PROPERTY for Intellectual Property
            /// </summary>
            public const string INTELLECTUAL_PROPERTY = "Intellectual Property";
            /// <summary>
            /// Constant LIABILITY for Liability
            /// </summary>
            public const string LIABILITY = "Liability";
            /// <summary>
            /// Constant ORDER_OF_PRECEDENCE for Order of Precedence
            /// </summary>
            public const string ORDER_OF_PRECEDENCE = "Order of Precedence";
            /// <summary>
            /// Constant PAYMENT_TERMS_BILLING for Payment Terms & Billing
            /// </summary>
            public const string PAYMENT_TERMS_BILLING = "Payment Terms & Billing";
            /// <summary>
            /// Constant PRICING_TAXES for Pricing & Taxes
            /// </summary>
            public const string PRICING_TAXES = "Pricing & Taxes";
            /// <summary>
            /// Constant PRIVACY for Privacy
            /// </summary>
            public const string PRIVACY = "Privacy";
            /// <summary>
            /// Constant RESPONSIBILITIES for Responsibilities
            /// </summary>
            public const string RESPONSIBILITIES = "Responsibilities";
            /// <summary>
            /// Constant SAFETY_AND_SECURITY for Safety and Security
            /// </summary>
            public const string SAFETY_AND_SECURITY = "Safety and Security";
            /// <summary>
            /// Constant SCOPE_OF_WORK for Scope of Work
            /// </summary>
            public const string SCOPE_OF_WORK = "Scope of Work";
            /// <summary>
            /// Constant SUBCONTRACTS for Subcontracts
            /// </summary>
            public const string SUBCONTRACTS = "Subcontracts";
            /// <summary>
            /// Constant TERM_TERMINATION for Term & Termination
            /// </summary>
            public const string TERM_TERMINATION = "Term & Termination";
            /// <summary>
            /// Constant WARRANTIES for Warranties
            /// </summary>
            public const string WARRANTIES = "Warranties";
            
        }

        /// <summary>
        /// The type of modification of the feedback entry in the updated labels response.
        /// </summary>
        public class ModificationValue
        {
            /// <summary>
            /// Constant ADDED for added
            /// </summary>
            public const string ADDED = "added";
            /// <summary>
            /// Constant UNCHANGED for unchanged
            /// </summary>
            public const string UNCHANGED = "unchanged";
            /// <summary>
            /// Constant REMOVED for removed
            /// </summary>
            public const string REMOVED = "removed";
            
        }

        /// <summary>
        /// The category of the associated element.
        /// Constants for possible values can be found using Category.LabelValue
        /// </summary>
        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }
        /// <summary>
        /// The type of modification of the feedback entry in the updated labels response.
        /// Constants for possible values can be found using Category.ModificationValue
        /// </summary>
        [JsonProperty("modification", NullValueHandling = NullValueHandling.Ignore)]
        public string Modification { get; set; }
        /// <summary>
        /// Hashed values that you can send to IBM to provide feedback or receive support.
        /// </summary>
        [JsonProperty("provenance_ids", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ProvenanceIds { get; set; }
    }
}
