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
    /// RegisterModel
    /// </summary>
    [DataContract]
    public partial class RegisterModel :  IEquatable<RegisterModel>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterModel" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected RegisterModel() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterModel" /> class.
        /// </summary>
        /// <param name="username">username (required).</param>
        /// <param name="password">password (required).</param>
        /// <param name="nickname">nickname (required).</param>
        /// <param name="description">description (required).</param>
        public RegisterModel(string username = default(string), string password = default(string), string nickname = default(string), string description = default(string))
        {
            // to ensure "username" is required (not null)
            this.Username = username ?? throw new ArgumentNullException("username is a required property for RegisterModel and cannot be null");;
            // to ensure "password" is required (not null)
            this.Password = password ?? throw new ArgumentNullException("password is a required property for RegisterModel and cannot be null");;
            // to ensure "nickname" is required (not null)
            this.Nickname = nickname ?? throw new ArgumentNullException("nickname is a required property for RegisterModel and cannot be null");;
            // to ensure "description" is required (not null)
            this.Description = description ?? throw new ArgumentNullException("description is a required property for RegisterModel and cannot be null");;
        }
        
        /// <summary>
        /// Gets or Sets Username
        /// </summary>
        [DataMember(Name="username", EmitDefaultValue=false)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or Sets Password
        /// </summary>
        [DataMember(Name="password", EmitDefaultValue=false)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or Sets Nickname
        /// </summary>
        [DataMember(Name="nickname", EmitDefaultValue=false)]
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or Sets Description
        /// </summary>
        [DataMember(Name="description", EmitDefaultValue=false)]
        public string Description { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class RegisterModel {\n");
            sb.Append("  Username: ").Append(Username).Append("\n");
            sb.Append("  Password: ").Append(Password).Append("\n");
            sb.Append("  Nickname: ").Append(Nickname).Append("\n");
            sb.Append("  Description: ").Append(Description).Append("\n");
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
            return this.Equals(input as RegisterModel);
        }

        /// <summary>
        /// Returns true if RegisterModel instances are equal
        /// </summary>
        /// <param name="input">Instance of RegisterModel to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(RegisterModel input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Username == input.Username ||
                    (this.Username != null &&
                    this.Username.Equals(input.Username))
                ) && 
                (
                    this.Password == input.Password ||
                    (this.Password != null &&
                    this.Password.Equals(input.Password))
                ) && 
                (
                    this.Nickname == input.Nickname ||
                    (this.Nickname != null &&
                    this.Nickname.Equals(input.Nickname))
                ) && 
                (
                    this.Description == input.Description ||
                    (this.Description != null &&
                    this.Description.Equals(input.Description))
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
                if (this.Username != null)
                    hashCode = hashCode * 59 + this.Username.GetHashCode();
                if (this.Password != null)
                    hashCode = hashCode * 59 + this.Password.GetHashCode();
                if (this.Nickname != null)
                    hashCode = hashCode * 59 + this.Nickname.GetHashCode();
                if (this.Description != null)
                    hashCode = hashCode * 59 + this.Description.GetHashCode();
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
            // Username (string) maxLength
            if(this.Username != null && this.Username.Length > 12)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Username, length must be less than 12.", new [] { "Username" });
            }

            // Username (string) minLength
            if(this.Username != null && this.Username.Length < 3)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Username, length must be greater than 3.", new [] { "Username" });
            }

            // Password (string) maxLength
            if(this.Password != null && this.Password.Length > 32)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Password, length must be less than 32.", new [] { "Password" });
            }

            // Password (string) minLength
            if(this.Password != null && this.Password.Length < 6)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Password, length must be greater than 6.", new [] { "Password" });
            }

            // Nickname (string) maxLength
            if(this.Nickname != null && this.Nickname.Length > 16)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Nickname, length must be less than 16.", new [] { "Nickname" });
            }

            // Nickname (string) minLength
            if(this.Nickname != null && this.Nickname.Length < 1)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Nickname, length must be greater than 1.", new [] { "Nickname" });
            }

            // Description (string) maxLength
            if(this.Description != null && this.Description.Length > 32)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Description, length must be less than 32.", new [] { "Description" });
            }

            yield break;
        }
    }

}
