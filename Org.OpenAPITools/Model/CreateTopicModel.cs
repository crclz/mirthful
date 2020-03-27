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
    /// CreateTopicModel
    /// </summary>
    [DataContract]
    public partial class CreateTopicModel :  IEquatable<CreateTopicModel>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTopicModel" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected CreateTopicModel() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTopicModel" /> class.
        /// </summary>
        /// <param name="isGroup">isGroup (required).</param>
        /// <param name="name">name (required).</param>
        /// <param name="description">description (required).</param>
        /// <param name="relatedWork">relatedWork.</param>
        public CreateTopicModel(bool isGroup = default(bool), string name = default(string), string description = default(string), string relatedWork = default(string))
        {
            this.IsGroup = isGroup;
            // to ensure "name" is required (not null)
            this.Name = name ?? throw new ArgumentNullException("name is a required property for CreateTopicModel and cannot be null");;
            // to ensure "description" is required (not null)
            this.Description = description ?? throw new ArgumentNullException("description is a required property for CreateTopicModel and cannot be null");;
            this.RelatedWork = relatedWork;
        }
        
        /// <summary>
        /// Gets or Sets IsGroup
        /// </summary>
        [DataMember(Name="isGroup", EmitDefaultValue=false)]
        public bool IsGroup { get; set; }

        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [DataMember(Name="name", EmitDefaultValue=false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets Description
        /// </summary>
        [DataMember(Name="description", EmitDefaultValue=false)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or Sets RelatedWork
        /// </summary>
        [DataMember(Name="relatedWork", EmitDefaultValue=true)]
        public string RelatedWork { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class CreateTopicModel {\n");
            sb.Append("  IsGroup: ").Append(IsGroup).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Description: ").Append(Description).Append("\n");
            sb.Append("  RelatedWork: ").Append(RelatedWork).Append("\n");
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
            return this.Equals(input as CreateTopicModel);
        }

        /// <summary>
        /// Returns true if CreateTopicModel instances are equal
        /// </summary>
        /// <param name="input">Instance of CreateTopicModel to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(CreateTopicModel input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.IsGroup == input.IsGroup ||
                    (this.IsGroup != null &&
                    this.IsGroup.Equals(input.IsGroup))
                ) && 
                (
                    this.Name == input.Name ||
                    (this.Name != null &&
                    this.Name.Equals(input.Name))
                ) && 
                (
                    this.Description == input.Description ||
                    (this.Description != null &&
                    this.Description.Equals(input.Description))
                ) && 
                (
                    this.RelatedWork == input.RelatedWork ||
                    (this.RelatedWork != null &&
                    this.RelatedWork.Equals(input.RelatedWork))
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
                if (this.IsGroup != null)
                    hashCode = hashCode * 59 + this.IsGroup.GetHashCode();
                if (this.Name != null)
                    hashCode = hashCode * 59 + this.Name.GetHashCode();
                if (this.Description != null)
                    hashCode = hashCode * 59 + this.Description.GetHashCode();
                if (this.RelatedWork != null)
                    hashCode = hashCode * 59 + this.RelatedWork.GetHashCode();
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
            // Name (string) minLength
            if(this.Name != null && this.Name.Length < 1)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Name, length must be greater than 1.", new [] { "Name" });
            }

            // Description (string) minLength
            if(this.Description != null && this.Description.Length < 3)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Description, length must be greater than 3.", new [] { "Description" });
            }

            yield break;
        }
    }

}