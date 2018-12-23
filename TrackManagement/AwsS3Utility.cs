using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.IO;
using System.Threading.Tasks;

namespace TrackManagement
{
    public class AwsS3Utility
    {
        public static async Task UploadFileAsync(Stream fileStream, string bucketName, string keyName, RegionEndpoint bucketRegion)
        {
            using (var s3Client = new AmazonS3Client(bucketRegion))
            {
                var fileTransferUtility = new TransferUtility(s3Client);
                await fileTransferUtility.UploadAsync(fileStream, bucketName, keyName);
            }
        }
    }
}