using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ClipKeep.Models.CustomModelValidators
{
    /// <summary>
    /// Check the usernamePropertyName and password entered are attributed to the same user.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class UnameMatchesPassAttribute : ValidationAttribute
    {
        /// <summary>
        /// Username passed in on validatior instantiation. As we tie this to the password field
        /// no need to pass in the password too.
        /// </summary>
        private readonly string _usernamePropertyName;
        public UnameMatchesPassAttribute(string usernamePropertyName)
        {
            _usernamePropertyName = usernamePropertyName;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Use parameterised queries to avoid SQL injection

            // seems like 'c' is cosmos' default table name from looking at the web client
            // not sure how to change but shouldn't really matter as we're seperating things
            // into different containers.
            var users = "c";

            // We'll only be getting string values from the password textbox so should be safe cast.
            var password = (string) value;

            // Get the user model's entered username

            // User reflection to get details of the username property we want to validate with.
            var getUsername = validationContext.ObjectType.GetProperties()
                .Where(property => property.Name == _usernamePropertyName).Take(1).ToArray();

            // Then get the value of this property with respect to the current user model instance we are looking at.
            var username = getUsername[0].GetValue(validationContext.ObjectInstance);

            var usernameFieldString = "Username";
            var userGetQueryString = $"SELECT * FROM {users} WHERE {users}.{usernameFieldString} = '{username}' ";

            // Generate the URI for the Users collection we're targerting in our cosmos DB
            var userCollectionUri = UriFactory.CreateDocumentCollectionUri(CosmosConfig.DatabaseId, CosmosConfig.UsersCollectionId);

            using (var client = new DocumentClient(new Uri(CosmosConfig.EndPointUrl), CosmosConfig.AuthorizationKey))
            {
                // Create/execute the query while ensuring we can run it across all partitions
                var userGetQueryResults = client.CreateDocumentQuery(userCollectionUri, userGetQueryString,
                    new FeedOptions {EnableCrossPartitionQuery = true}).ToList();

                // Validate against edge cases

                if (userGetQueryResults.Count() == 0)
                {
                    return new ValidationResult("We don't recognise the username + password combo you've entered. Double check it's correct.");
                }
                else if (userGetQueryResults.Count() > 1)
                {
                    // We shouldn't hit this point, as username uniqueness get checked on registration.
                    // However if we do something has gone seriously wrong, and something 
                    // malicious is probably going on, so hopefully redirection helps stop it!

                    // Log on server terminal output so we can see the stacktrace server side.
                    Console.WriteLine(new Exception("*** Critical error! Multiple identical usernames in the DB! ***"));

                    return new ValidationResult("Critical error! Try again later.");
                }

                var userGetModel = JsonConvert.DeserializeObject<User>(userGetQueryResults[0].ToString());

               // Validate entered password has is the same as stored password hash.
                var enteredPassHash = PasswordHasher.GetPassHashString(password, userGetModel.PassSalt);

                if (enteredPassHash != userGetModel.Password)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }

            }

            // Field is valid
            return null;
        }
    }
}