/* 
 * TheAPI
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: v1
 * 
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using OpenAPIDateConverter = Org.OpenAPITools.Client.OpenAPIDateConverter;

namespace Org.OpenAPITools.Model
{
    /// <summary>
    /// HandleRequestModel
    /// </summary>
    [DataContract]
    public partial class HandleRequestModel :  IEquatable<HandleRequestModel>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HandleRequestModel" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected HandleRequestModel() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="HandleRequestModel" /> class.
        /// </summary>
        /// <param name="requestId">requestId (required).</param>
        /// <param name="accept">accept (required).</param>
        public HandleRequestModel(string requestId = default(string), bool accept = default(bool))
        {
            // to ensure "requestId" is required (not null)
            this.RequestId = requestId ?? throw new ArgumentNullException("requestId is a required property for HandleRequestModel and cannot be null");;
            this.Accept = accept;
        }
        
        /// <summary>
        /// Gets or Sets RequestId
        /// </summary>
        [DataMember(Name="requestId", EmitDefaultValue=false)]
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or Sets Accept
        /// </summary>
        [DataMember(Name="accept", EmitDefaultValue=false)]
        public bool Accept { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class HandleRequestModel {\n");
            sb.Append("  RequestId: ").Append(RequestId).Append("\n");
            sb.Append("  Accept: ").Append(Accept).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as HandleRequestModel);
        }

        /// <summary>
        /// Returns true if HandleRequestModel instances are equal
        /// </summary>
        /// <param name="input">Instance of HandleRequestModel to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(HandleRequestModel input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.RequestId == input.RequestId ||
                    (this.RequestId != null &&
                    this.RequestId.Equals(input.RequestId))
                ) && 
                (
                    this.Accept == input.Accept ||
                    this.Accept.Equals(input.Accept)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.RequestId != null)
                    hashCode = hashCode * 59 + this.RequestId.GetHashCode();
                hashCode = hashCode * 59 + this.Accept.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}
