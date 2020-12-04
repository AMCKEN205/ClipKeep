using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using ClipKeep.Models.CustomModelValidators;
using ClipKeep.Models.Interfaces;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Linq;

namespace ClipKeep.Models
{
    /// <summary>
    /// Hold data used to generate new users.
    /// </summary>
    
    public class UserRegister : IDbStoreable
    {
        // Properties

        /// <summary>
        /// Property value corresponding to the user's entered username.
        /// Validated against the listed validation attributes.
        /// </summary>
        [DisplayName("Username")]
        [Required(ErrorMessage = "Enter your username")]
        [MaxLength(15, ErrorMessage = "Usernames shouldn't be longer than 15 characters.")]
        [MinLength(3, ErrorMessage = "Usernames shouldn't be shorter than 3 characters.")]
        [UnameTaken(ErrorMessage = "Sorry your chosen username has been taken, select another!")]
        public string Username { get; set; }

        /// <summary>
        /// Property value corresponding to the user's confirmed username.
        /// Validated against the listed validation attributes.
        /// </summary>
        [Compare("Username", ErrorMessage = "Usernames don't match")]
        [DisplayName("Confirm Username")]
        public string ConfirmUsername { get; set; }
        
        /// <summary>
        /// Property value corresponding to the user's entered password.
        /// Validated against the listed validation attributes.
        /// </summary>
        [DisplayName("Password")]
        [Required(ErrorMessage = "Enter your password")]
        [MaxLength(20, ErrorMessage = "Passwords shouldn't be longer than 20 characters.")]
        [MinLength(6, ErrorMessage = "Passwords shouldn't be shorter than 6 characters.")]
        [ContainsSpecialCharacters(ErrorMessage = "Password should contain a special character [one of: '!' '@' '#' '$' '%' '^' '&' '*' '(' ')']")]
        public string Password { get; set; }
        
        /// <summary>
        /// Property value corresponding to the user's confirmed password.
        /// Validated against the listed validation attributes.
        /// </summary>
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
        
        // fields

        /// <summary>
        /// Salt applied to the user password hash to ensure hashes aren't 'guessable'
        /// Used Guig here as it's a one-liner, however maybe another randomisation method would be better?
        /// </summary>
        private readonly string _hashSalt = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Store the data we want from the UserRegister model object in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> StoreInDb()
        {
            try
            {
                // Retrieve the user's sign up details formatted ion a JSON doc as required for DB storage
                var userStoreDoc = GetStoreJsonDoc();

                // Generate the URI for the Users collection we're targerting in our cosmos DB
                var userCollectionUri =
                    UriFactory.CreateDocumentCollectionUri(CosmosConfig.DatabaseId, CosmosConfig.UsersCollectionId);

                // Generate a client connection to the cosmos DB that gets disposed of once we've stored the new user's info.
                using (var client =
                    new DocumentClient(new Uri(CosmosConfig.EndPointUrl), CosmosConfig.AuthorizationKey))
                {
                    // Open the asynchronous connection await document retrieval,
                    // feedback the result of this to the controller so it can act appropriately. 
                    await client.OpenAsync();

                    await client.CreateDocumentAsync(userCollectionUri, userStoreDoc);

                    return true;

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to add user, see below exception");
                Console.WriteLine(e);
                return false;
            }
            

        }

        /// <summary>
        /// Generates the JSON doc used for storage of newly signed up users.
        /// </summary>
        /// <returns> JSON doc with properties we want to store for a new user. </returns>
        private JObject GetStoreJsonDoc()
        {
            // use MS' inbuilt GUID generation functionality to generate unique user ids.
            var userId = Guid.NewGuid().ToString("N");

            // Get a random partition key, used by cosmos for DB scaling.
            // Would be better to use a value shared by many users but none exist in this app.

            var partitionKeys = new string[] {"A", "B", "C"}; // Three different paritions

            // Will need to be in string format to store in JSON.
            int partitionKeyIndex = new Random().Next(0, 2);

            string partitionKey = partitionKeys[partitionKeyIndex];

            var passHash = PasswordHasher.GetPassHashString(Password, _hashSalt);

            // Define the properties we'll want to store on user registration.
            var propertiesForStore = new Dictionary<string, string>()
            {
                {"UserId", userId },
                {"PartitionKey", partitionKey },
                {"Username", Username},
                {"Password", passHash},
                {"PassSalt", _hashSalt }
            };

            // Generate Json doc of properties for store and return.
            var userRegisterJsonDoc = JsonConvert.SerializeObject(propertiesForStore);

            return JObject.Parse(userRegisterJsonDoc);
        }

        

    }
}