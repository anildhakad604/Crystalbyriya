using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.EntityFrameworkCore;
using CrystalByRiya.Models;

namespace CrystalByRiya.Classes
{
    public class AmazonS3
    {
        ApplicationDbContext _context;
        private readonly IAmazonS3 _s3Client;
        private readonly AwsCredentials _awsCredentials;

        public AmazonS3(ApplicationDbContext context, AwsCredentials awsCredentials)
        {
            _context = context;
            _awsCredentials = awsCredentials;
            _s3Client = new AmazonS3Client(_awsCredentials.accessKey, _awsCredentials.secretKey, Amazon.RegionEndpoint.USEast2);
        }

        public async Task<string> UploadFileToS3(IFormFile file, string foldername)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            string s3Key = $"{foldername}/{fileName}";




            using var fileStream = file.OpenReadStream();

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = fileStream,
                Key = s3Key,
                BucketName = _awsCredentials.bucketName
            };

            using var fileTransferUtility = new TransferUtility(_s3Client);
            await fileTransferUtility.UploadAsync(uploadRequest);

            return $"{fileName}";
        }
        public async Task<bool> DeleteFileFromS3(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return false;

            try
            {

                string fileKey = fileUrl.Replace(_awsCredentials.cloudFrontURL, "");

                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _awsCredentials.bucketName,
                    Key = fileKey
                };

                await _s3Client.DeleteObjectAsync(deleteRequest);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file from S3: {ex.Message}");
                return false;
            }
        }
    }
}