using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

Console.WriteLine("Azure Blob Storage exercise !");

// Run the examples asynchronously, wait for the results before proceeding
ProcessAsync().GetAwaiter().GetResult();

Console.WriteLine("Press enter to exit the sample application.");
Console.ReadLine();

static async Task ProcessAsync()
{
    string storageConnectionString = "CONNECTION STRING";

    var blobServiceClient = new BlobServiceClient(storageConnectionString);

    var containerName = "myBlobContainer" + Guid.NewGuid().ToString();

    BlobContainerClient blobContainerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);

    Console.WriteLine("A container named '" + containerName + "' has been created. " +
    "\nTake a minute and verify in the portal." +
    "\nNext a file will be created and uploaded to the container.");

    var localPath = "./data/";
    var fileName = "wtfile" + Guid.NewGuid().ToString() + ".txt";
    var localFilePath = Path.Combine(localPath, fileName);

    var blobClient = blobContainerClient.GetBlobClient(fileName);

    await UploadBlobToContainerAsync(blobClient, fileName, localFilePath);

    await ListBlobsInContainer(blobContainerClient);

    await DownloadBlobsToLocal(blobClient, localFilePath);

    await DeleteContainerAsync(blobContainerClient);

    Console.WriteLine("Press 'Enter' to continue.");
    Console.ReadLine();
}

/// <summary>
/// Upload blobs to a container
/// </summary>
/// <param name="blobClient"></param>
/// <param name="fileName"></param>
/// <param name="localFilePath"></param>
/// <returns></returns>
static async Task UploadBlobToContainerAsync(BlobClient blobClient, string fileName, string localFilePath)
{
    // Write text to the file
    await File.WriteAllTextAsync(localFilePath, "Hello, World!");

    Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

    using var uploadFileStream = File.OpenRead(localFilePath);
    await blobClient.UploadAsync(uploadFileStream, overwrite: true);

    Console.WriteLine("\nThe file was uploaded. We'll verify by listing" +
        " the blobs next.");
    Console.WriteLine("Press 'Enter' to continue.");
    Console.ReadLine();
}

/// <summary>
/// List the blobs in a container
/// </summary>
/// <param name="blobContainerClient"></param>
/// <returns></returns>
static async Task ListBlobsInContainer(BlobContainerClient blobContainerClient)
{
    Console.WriteLine("Listing blobs...");
    await foreach (var blobItem in blobContainerClient.GetBlobsAsync())
    {
        Console.WriteLine("\t" + blobItem.Name);
    }

    Console.WriteLine("\nYou can also verify by looking inside the " +
        "container in the portal." +
        "\nNext the blob will be downloaded with an altered file name.");
    Console.WriteLine("Press 'Enter' to continue.");
    Console.ReadLine();
}


/// <summary>
/// 
/// </summary>
/// <param name="blobClient"></param>
/// <param name="localFilePath"></param>
/// <returns></returns>
static async Task DownloadBlobsToLocal(BlobClient blobClient, string localFilePath)
{
    var downloadFilePath = localFilePath.Replace(".txt", "DOWNLOADED.txt");
    Console.WriteLine("\nDownloading blob to\n\t{0}\n", downloadFilePath);

    // Download the blob's contents and save it to a file
    BlobDownloadInfo download = await blobClient.DownloadAsync();

    using (var downloadFileStream = File.OpenWrite(downloadFilePath))
    {
        await download.Content.CopyToAsync(downloadFileStream);
    }
    Console.WriteLine("\nLocate the local file in the data directory created earlier to verify it was downloaded.");
    Console.WriteLine("The next step is to delete the container and local files.");
    Console.WriteLine("Press 'Enter' to continue.");
    Console.ReadLine();
}

/// <summary>
/// Delete a container
/// </summary>
/// <param name="blobContainerClient"></param>
/// <returns></returns>
static async Task DeleteContainerAsync(BlobContainerClient blobContainerClient)
{
    // Delete the container and clean up local files created
    Console.WriteLine("\n\nDeleting blob container...");
    await blobContainerClient.DeleteAsync();
}
