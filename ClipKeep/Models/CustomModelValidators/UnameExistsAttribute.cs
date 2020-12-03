using Microsoft.Azure.Documents.Client;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ClipKeep.Models.CustomModelValidators
{
    /// <summary>
    /// Check an entered username exists in the DB.
    /// </summary>
    public class UnameExistsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Use parameterised queries to avoid SQL injection

            // seems like 'c' is cosmos' default table name from looking at the web client
            // not sure how to change but shouldn't really matter as we're seperating things
            // into different containers.
            var users = "c";

            // We'll only be getting string values from the username textbox so should be safe cast.
            var username = (string) value;
            var usernameFieldString = "Username";
            var unameExistsQueryString = $"SELECT * FROM {users} WHERE {users}.{usernameFieldString} = '{username}' ";

            using (var client =
                new DocumentClient(new Uri(CosmosConfig.EndPointUrl), CosmosConfig.AuthorizationKey))
            {
                // Generate the URI for the Users collection we're targerting in our cosmos DB
                var userCollectionUri =
                    UriFactory.CreateDocumentCollectionUri(CosmosConfig.DatabaseId, CosmosConfig.UsersCollectionId);

                // Create/execute the query while ensuring we can run it across all partitions
                var unameTakenQueryResults = client.CreateDocumentQuery(userCollectionUri, unameExistsQueryString,
                    new FeedOptions {EnableCrossPartitionQuery = true}).ToList();

                if (unameTakenQueryResults.Count() == 0)
                {
                    // Username doesn't exist
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                else if (unameTakenQueryResults.Count() > 1)
                {
                    // We shouldn't hit this point, as username uniqueness get checked on registration.
                    // However if we do something has gone seriously wrong, and something 
                    // malicious is probably going on, so hopefully redirection helps stop it!

                    // Log on server terminal output so we can see the stacktrace server side.
                    Console.WriteLine(new Exception("*** Critical error! Multiple identical usernames in the DB! ***"));

                    return new ValidationResult("Critical error! Try again later.");
                }

                // Field is valid
                return null;
            }
        }
    }
}