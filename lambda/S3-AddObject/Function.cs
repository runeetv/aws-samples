using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;


using Amazon.Rekognition;
using Amazon.Rekognition.Model;

using Amazon.S3;
using Amazon.S3.Model;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace S3_AddObject;

public class Function
{
    private const string FilePath = "/tmp/downloadedobject";

    IAmazonS3 S3Client { get; }


    public Function()
    {
        this.S3Client = new AmazonS3Client();
    }
    
    public Function(IAmazonS3 s3Client)
    {
        this.S3Client = s3Client;        
    }
    // Function Invoked by source S3 bucket on a file upload 
    public async Task FunctionHandler(S3Event input, ILambdaContext context)
    {
        //prints file info
        Console.WriteLine($"Number of records {input.Records.Count}");
        Console.WriteLine($"Record number one {input.Records[0]}");

        foreach (var record in input.Records)
        {
            //Get S3 Object
            Console.WriteLine($"Working on file {record.S3.Bucket.Name}:{record.S3.Object.Key}");            
            GetObjectRequest request = new GetObjectRequest 
            {
                BucketName = record.S3.Bucket.Name,
                Key = record.S3.Object.Key
            };

            GetObjectResponse response  = await this.S3Client.GetObjectAsync(request);

            //Write file locally
            Console.WriteLine($"Downloading file locally");
            await response.WriteResponseStreamToFileAsync(FilePath, false, default(System.Threading.CancellationToken));
            Console.WriteLine($"Downloading done");


            //Tage the S3 file after downloading
            var tags = new List<Tag>();
            tags.Add(new Tag { Key = "Download", Value = "Yes" });

            Console.WriteLine($"Tags begin");
            await this.S3Client.PutObjectTaggingAsync(new PutObjectTaggingRequest
            {
                BucketName = record.S3.Bucket.Name,
                Key = record.S3.Object.Key,
                Tagging = new Tagging
                {
                    TagSet = tags
                }
            });
            Console.WriteLine($"Tags done");

            //Upload the object to a different S3 bucket
            var targetS3Bucket = "lambda-useast1";
            var objectKey = "Item1_" + new Random().Next(1, 10000).ToString();
            PutObjectRequest putRequest = new PutObjectRequest
            {
                BucketName = targetS3Bucket,
                Key = objectKey,
                FilePath = FilePath
            };

            Console.WriteLine($"Uploading file to S3 bucket {targetS3Bucket}");
            PutObjectResponse putResponse = await this.S3Client.PutObjectAsync(putRequest);
            Console.WriteLine($"Uploading file with request id {putResponse.ResponseMetadata.RequestId}");


            //Generate pre-signed URL
            Console.WriteLine($"Generating pre-signed url for object {objectKey } in bucket: {targetS3Bucket}");
            var expiry = DateTime.Now.AddMinutes(15); //URL expires in 15 minutes
            GetPreSignedUrlRequest presignedRequest = new GetPreSignedUrlRequest
            {
                BucketName = targetS3Bucket,
                Key = objectKey,
                Expires = expiry
            };
            
            string presignedURL =  S3Client.GetPreSignedURL(presignedRequest);
            Console.WriteLine($"Pre-signed url : {presignedURL} , expires at :  {expiry} ");

        }
        return;
    }

    /// <summary>
    /// A function for responding to S3 create events. It will determine if the object is an image and use Amazon Rekognition
    /// to detect labels and add the labels as tags on the S3 object.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    //public async Task FunctionHandler(S3Event input, ILambdaContext context)
    //{
    //    foreach(var record in input.Records)
    //    {
    //        if(!SupportedImageTypes.Contains(Path.GetExtension(record.S3.Object.Key)))
    //        {
    //            Console.WriteLine($"Object {record.S3.Bucket.Name}:{record.S3.Object.Key} is not a supported image type");
    //            continue;
    //        }

    //        Console.WriteLine($"Looking for labels in image {record.S3.Bucket.Name}:{record.S3.Object.Key}");
    //        var detectResponses = await this.RekognitionClient.DetectLabelsAsync(new DetectLabelsRequest
    //        {
    //            MinConfidence = MinConfidence,
    //            Image = new Image
    //            {
    //                S3Object = new Amazon.Rekognition.Model.S3Object
    //                {
    //                    Bucket = record.S3.Bucket.Name,
    //                    Name = record.S3.Object.Key
    //                }
    //            }
    //        });

    //        var tags = new List<Tag>();
    //        foreach(var label in detectResponses.Labels)
    //        {
    //            if(tags.Count < 10)
    //            {
    //                Console.WriteLine($"\tFound Label {label.Name} with confidence {label.Confidence}");
    //                tags.Add(new Tag { Key = label.Name, Value = label.Confidence.ToString() });
    //            }
    //            else
    //            {
    //                Console.WriteLine($"\tSkipped label {label.Name} with confidence {label.Confidence} because the maximum number of tags has been reached");
    //            }
    //        }

    //        await this.S3Client.PutObjectTaggingAsync(new PutObjectTaggingRequest
    //        {
    //            BucketName = record.S3.Bucket.Name,
    //            Key = record.S3.Object.Key,
    //            Tagging = new Tagging
    //            {
    //                TagSet = tags
    //            }
    //        });
    //    }
    //    return;
    //}
}