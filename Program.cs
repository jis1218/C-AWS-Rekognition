using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.Runtime.CredentialManagement;
using Amazon.Runtime;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace AWSRekog
{
    class Program
    {
        static void Main(string[] args)
        {
            AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient();
            Amazon.Rekognition.Model.Image image = new Amazon.Rekognition.Model.Image();

            
            MemoryStream ms = null;

            try
            {
                //Bitmap은 base64형식이 아닌듯 하다. AWS에서는 base64형식의 이미지 파일이 필요한데 이를 바꿔주는 코드는 아래와 같다.
                //Base64는 8비트짜리 바이트 3개를 6비트씩 4개로 쪼개어 표현한다.
                Bitmap bitmap = new Bitmap(@"D:\picture\insup.bmp");
                ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Jpeg);
                byte[] data = new byte[ms.Length];
                ms.Read(data, 0, (int)ms.Length);
                image.Bytes = ms;

            }catch(Exception e)
            {

            }

            searchFace(image, rekognitionClient);
            
            //using (FileStream fs = new FileStream(@"D:\picture\paul.bmp", FileMode.Open, FileAccess.Read)) 
            //{
            //    byte[] data = null;
            //    data = new byte[fs.Length];
            //    fs.Read(data, 0, (int)fs.Length);
            //    image.Bytes = new MemoryStream(data);
            //}

        }

        private static void searchFace(Amazon.Rekognition.Model.Image image, AmazonRekognitionClient rekognitionClient)
        {
            String collectionId = "MyCollection";

            SearchFacesByImageRequest request = new SearchFacesByImageRequest()
            {
                CollectionId = collectionId,
                Image = image
            };

            SearchFacesByImageResponse response = rekognitionClient.SearchFacesByImage(request);

            foreach(FaceMatch face in response.FaceMatches)
            {
                Console.WriteLine("FaceId: " + face.Face.FaceId + ", Similarity: " + face.Similarity);
            }
        }

        private static void detectFace(Amazon.Rekognition.Model.Image image, AmazonRekognitionClient rekognitionClient)
        {
            DetectFacesRequest request = new DetectFacesRequest()
            {
                Image = image
            };

            try
            {
                DetectFacesResponse detectFacesResponse = rekognitionClient.DetectFaces(request);

                foreach (FaceDetail face in detectFacesResponse.FaceDetails)
                {
                    Console.WriteLine("Confidence : {0}\nAge :" + face.Confidence + ", " + face.BoundingBox.Top + ", " + face.BoundingBox.Left + ", " +
                        face.BoundingBox.Height + ", " + face.BoundingBox.Width);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
