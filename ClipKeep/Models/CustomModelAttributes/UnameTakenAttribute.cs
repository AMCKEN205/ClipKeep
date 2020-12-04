using Microsoft.Azure.Documents.Client;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ClipKeep.Models.CustomModelValidators
{
    /// <summary>
    /// Check to see if a user's chosen username has already been taken on registration form POST.
    /// </summary>
    public class UnameTakenAttribute : ValidationAttribute
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
            var usernameFieldName = "Username";
            var unameTakenQueryString = $"SELECT * FROM {users} WHERE {users}.{usernameFieldName} = '{username}' ";

            using (var client =
                new DocumentClient(new Uri(CosmosConfig.EndPointUrl), CosmosConfig.AuthorizationKey))
            {
                // Generate the URI for the Users collection we're targerting in our cosmos DB
                var userCollectionUri =
                    UriFactory.CreateDocumentCollectionUri(CosmosConfig.DatabaseId, CosmosConfig.UsersCollectionId);

                // Create/execute the query while ensuring we can run it across all partitions
                var unameTakenQueryResults = client.CreateDocumentQuery(userCollectionUri, unameTakenQueryString, new FeedOptions { EnableCrossPartitionQuery = true }).ToList();
                if (unameTakenQueryResults.Count() > 0)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }

                // Field is valid
                return null;
            }
        
        }
    }
}