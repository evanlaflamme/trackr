﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace trackr.ImageProcessing
{
    class FaceApiController
    {
        /// <summary>
        /// Subscription key to azure face PAI instance
        /// </summary>
        private const string _faceAPIKey = "f197462dd3eb415d8c5a8e367efd709c";
        /// <summary>
        /// API endpoint of azure face api
        /// </summary>
        private const string _uri = "https://westus.api.cognitive.microsoft.com/face/v1.0/detect?";

        /// <summary>
        /// Request parameters to specify how much info to extract
        /// </summary>
        private const string _requestParameters = "returnFaceAttributes=emotion";

        /// <summary>
        /// Compute the emotions expressed in the image and return the emotion data
        /// </summary>
        /// <param name="img">Image represented in a byte array</param>
        public static EmotionData CalculateEmotions(byte[] img)
        {
            var client = new HttpClient();

            // Add the dubscription of face API to header
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _faceAPIKey);

            string uriParameters = _uri + _requestParameters;

            HttpResponseMessage response;
            string responseContent;


            using (var content = new ByteArrayContent(img))
            {
                // Uses content type "application/octet-stream".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Process the data and wait for a response 
                response = client.PostAsync(uriParameters, content).Result;
                responseContent = response.Content.ReadAsStringAsync().Result;

                EmotionData returnData = null;

                try
                {
                    // Get the first face found
                    JToken face = JArray.Parse(responseContent).First.Last.First.First.First;

                    // Get the string representing just 
                    string emotionString = JsonConvert.SerializeObject(face);

                    returnData = EmotionData.Deserialize(emotionString);

                }
                finally
                {

                }

                return returnData;
            }

        }

        static public byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

    }
}
