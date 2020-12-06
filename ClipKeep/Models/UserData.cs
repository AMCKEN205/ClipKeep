using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ClipKeep.Models.Interfaces;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ClipKeep.Models
{
    /// <summary>
    /// Used to hold all the content models pertaining to a user.
    /// </summary>
    public class UserData : IDbStoreable
    {
        /// <summary>
        /// The actual files a user has pasted into clipkeep.
        /// </summary>
        [Required]
        public List<IPastedItem> UserClipKeepContents { get; private set; } = new List<IPastedItem>();

        /// <summary>
        /// Identifies which user we're modelling data against.
        /// </summary>
        [Required]
        public string ParentUserName { get; private set; } = HttpContext.Current.User.Identity.Name;

        /// <summary>
        /// Allows reception of new pasted content. Cotnent cna either be string or base 64 data url (for images)
        /// so only need to handle string types atm.
        /// </summary>
        [Required]
        public string PastedContent { get; set; }

        /// <summary>
        /// Identify what type of new pasted content we are dealing with (text or image ( + image file type) ).
        /// </summary>
        [Required]
        public string PastedContentType { get; set; }

        /// <summary>
        /// The HTML used to display the user's clipkeep clipboard contents.
        /// </summary>
        [Required]
        public List<string> PastedContentsDisplayHTML { get; private set; }

        /// <summary>
        /// Initalise the user's data collection model on start of index view
        /// </summary>
        /// <returns> An indication as to whether the read operation completed successfully </returns>
        public bool GetFromDb()
        {
            try
            {
                // Use parameterised queries to avoid SQL injection

                // seems like 'c' is cosmos' default table name from looking at the web client
                // not sure how to change but shouldn't really matter as we're seperating things
                // into different containers.
                var pastedItems = "c";

                // This suggests we're getting ALL pasted items. However as we target by partition key,
                // which is the current user's username, we are also filtering out other users.
                var itemBelongsToUserQueryString = $"SELECT * FROM {pastedItems}";

                // Generate the URI for the pasted items collection we're targerting in our cosmos DB
                var pastedItemsCollectionUri = UriFactory.CreateDocumentCollectionUri(CosmosConfig.DatabaseId, CosmosConfig.PastedItemsCollectionId);

                //Partition key is stored as a hash value in Cosmos. Essentially tells the DB to only look at those documents
                // with the provided partition key value. Makes queries far more efficient.
                var partitionKey = new PartitionKey(ParentUserName);
                var targetPartitionKey = new FeedOptions()
                {
                    PartitionKey = partitionKey
                };

                // Declare this here so we can close the client while parsing client data collection.
                var contentCollectionGetQueryResultsList = new List<dynamic>();
                using (var client = new DocumentClient(new Uri(CosmosConfig.EndPointUrl), CosmosConfig.AuthorizationKey))
                {

                    // Create/execute the query while ensuring we target the user's partition (far more efficient).
                    var contentCollectionGetQueryResults = client.CreateDocumentQuery(
                        pastedItemsCollectionUri,
                        itemBelongsToUserQueryString,
                        targetPartitionKey
                    );

                    contentCollectionGetQueryResultsList = contentCollectionGetQueryResults.ToList();

                    if (contentCollectionGetQueryResultsList.Count() == 0)
                    {
                        // DB request success but user doesn't have any items pasted yet.
                        return true;
                    }

                }
                InitContentCollection(contentCollectionGetQueryResultsList);
                
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }

        /// <summary>
        /// Store a new pasted item in the application database.
        /// </summary>
        /// <returns> An indication as to whether the store operation completed successfully </returns>
        public async Task<bool> StoreInDb()
        {
            try
            {
                // Retrieve the pasted item's details formatted in a JSON doc as required for DB storage
                var pastedItemsStoreDoc = GetStoreJsonDoc();

                // Generate the URI for the Users collection we're targerting in our cosmos DB
                var pastedItemsCollectionUri =
                    UriFactory.CreateDocumentCollectionUri(CosmosConfig.DatabaseId, CosmosConfig.PastedItemsCollectionId);

                // Generate a client connection to the cosmos DB that gets disposed of once we've stored the new user's info.
                using (var client =
                    new DocumentClient(new Uri(CosmosConfig.EndPointUrl), CosmosConfig.AuthorizationKey))
                {
                    // Open the asynchronous connection await document retrieval,
                    // feedback the result of this to the controller so it can act appropriately. 
                    await client.OpenAsync();

                    await client.CreateDocumentAsync(pastedItemsCollectionUri, pastedItemsStoreDoc);

                    return true;

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to add pasted item, see below exception");
                Console.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        /// Delete the oldest item the user has stored on clipkeep.
        /// </summary>
        /// <returns> An indication as to whether the oldest item has beed delete successfully. </returns>
        public async Task<bool> DeleteOldestFromDb(string docIdToDelete)
        {
            try
            {
                // Generate the URI for the specific document we're targerting for deletion in our cosmos DB
                var pastedItemsCollectionUri = UriFactory.CreateDocumentUri
                (
                    CosmosConfig.DatabaseId, 
                    CosmosConfig.PastedItemsCollectionId, docIdToDelete
                );

                // Setup partition key to target the current/ logged in user.
                var partitionKey = new PartitionKey(ParentUserName);
                var targetPartitionKey = new RequestOptions
                {
                    PartitionKey = partitionKey
                };

                // Declare this here so we can close the client while parsing client data collection.
                var contentCollectionGetQueryResultsList = new List<dynamic>();
                using (var client = new DocumentClient(new Uri(CosmosConfig.EndPointUrl), CosmosConfig.AuthorizationKey))
                {

                // Delete the document
                await client.DeleteDocumentAsync
                (
                        pastedItemsCollectionUri,
                        targetPartitionKey
                );

                }

                // Delete the oldest document from the objects ClipKeepContent collection. Shouldn't really matter 
                // as the client will refresh anyway, however just in case!
                UserClipKeepContents.RemoveAt(UserClipKeepContents.Count - 1);
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        /// Initalises a users content collection on get from the DB.
        /// </summary>
        private void InitContentCollection(List<dynamic> getQueryResultsList)
        {
            // Fill the user's ClipKeep contents
            IPastedItem pastedItem = null;
            foreach (JObject result in getQueryResultsList)
            {
                var contentType = (string) result["PastedContentType"];

                if (contentType == "Text")
                {
                    var pastedText = new PastedText(result);
                    pastedItem = pastedText;
                }

                else if (contentType.Contains("image"))
                {
                    var pastedImage = new PastedImage(result);
                    pastedItem = pastedImage;
                }

                UserClipKeepContents.Add(pastedItem);
            }
            // And sort in descending time order for ordered deletion and display.
            UserClipKeepContents = UserClipKeepContents.OrderByDescending(clipKeepItem => clipKeepItem.DatePasted).ToList();
        }


        /// <summary>
        /// Generates the JSON doc used for storage of newly signed up users.
        /// </summary>
        /// <returns> JSON doc with properties we want to store for a new user. </returns>
        private JObject GetStoreJsonDoc()
        {
            // Not totally sure why but this sometimes happens when pasting (at least during debug).
            // However fix on the server if it does. TODO: Do something less hacky than this.
            if (PastedContentType == null & PastedContent.Contains("image"))
            {
                PastedContentType = "image";
            }
            else
            {
                PastedContentType = "Text";
            }
            // Define the properties we'll want to store on user registration.
            var propertiesForStore = new Dictionary<string, string>()
            {
                {"ParentUserName", ParentUserName },
                {"PastedContent", PastedContent },
                {"PastedContentType", PastedContentType},
                {"DatePasted", DateTime.Now.ToString("dd/MM/yy HH:mm:ss", CultureInfo.InvariantCulture)}
            };

            // Generate Json doc of properties for store and return.
            var userRegisterJsonDoc = JsonConvert.SerializeObject(propertiesForStore);

            return JObject.Parse(userRegisterJsonDoc);
        }
    }
}