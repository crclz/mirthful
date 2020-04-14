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
    /// SendDiscussionModel
    /// </summary>
    [DataContract]
    public partial class SendDiscussionModel :  IEquatable<SendDiscussionModel>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendDiscussionModel" /> class.
        /// </summary>
        /// <param name="topicId">topicId.</param>
        /// <param name="text">text.</param>
        /// <param name="imageUrl">imageUrl.</param>
        public SendDiscussionModel(Guid topicId = default(Guid), string text = default(string), string imageUrl = default(string))
        {
            this.TopicId = topicId;
            this.Text = text;
            this.ImageUrl = imageUrl;
        }
        
        /// <summary>
        /// Gets or Sets TopicId
        /// </summary>
        [DataMember(Name="topicId", EmitDefaultValue=false)]
        public Guid TopicId { get; set; }

        /// <summary>
        /// Gets or Sets Text
        /// </summary>
        [DataMember(Name="text", EmitDefaultValue=true)]
        public string Text { get; set; }

        /// <summary>
        /// Gets or Sets ImageUrl
        /// </summary>
        [DataMember(Name="imageUrl", EmitDefaultValue=true)]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SendDiscussionModel {\n");
            sb.Append("  TopicId: ").Append(TopicId).Append("\n");
            sb.Append("  Text: ").Append(Text).Append("\n");
            sb.Append("  ImageUrl: ").Append(ImageUrl).Append("\n");
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
            return this.Equals(input as SendDiscussionModel);
        }

        /// <summary>
        /// Returns true if SendDiscussionModel instances are equal
        /// </summary>
        /// <param name="input">Instance of SendDiscussionModel to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SendDiscussionModel input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.TopicId == input.TopicId ||
                    (this.TopicId != null &&
                    this.TopicId.Equals(input.TopicId))
                ) && 
                (
                    this.Text == input.Text ||
                    (this.Text != null &&
                    this.Text.Equals(input.Text))
                ) && 
                (
                    this.ImageUrl == input.ImageUrl ||
                    (this.ImageUrl != null &&
                    this.ImageUrl.Equals(input.ImageUrl))
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
                if (this.TopicId != null)
                    hashCode = hashCode * 59 + this.TopicId.GetHashCode();
                if (this.Text != null)
                    hashCode = hashCode * 59 + this.Text.GetHashCode();
                if (this.ImageUrl != null)
                    hashCode = hashCode * 59 + this.ImageUrl.GetHashCode();
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
            // Text (string) maxLength
            if(this.Text != null && this.Text.Length > 800)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Text, length must be less than 800.", new [] { "Text" });
            }

            // ImageUrl (string) maxLength
            if(this.ImageUrl != null && this.ImageUrl.Length > 100)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for ImageUrl, length must be less than 100.", new [] { "ImageUrl" });
            }

            yield break;
        }
    }

}
