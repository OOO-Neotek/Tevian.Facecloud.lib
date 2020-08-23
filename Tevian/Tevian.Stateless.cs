using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tevian
{
    public partial class Tevian
    {
        /// <summary>
        /// Detect faces on the given image. Demographics, attributes or face landmarks information may be included
        /// </summary>
        /// <param name="image">Source image</param>
        /// <param name="fd_min_size">Minimal size of face that detector will search</param>
        /// <param name="fd_max_size">Maximal size of face that detector will search.
        /// NOTE: zero value disables this constraint</param>
        /// <param name="fd_threshold">Score threshold to be applied on found faces.
        /// NOTE: smaller values increase detector recall, but decrease precision</param>
        /// <param name="rotate_until_faces_found">Apply rotations to the input image until some faces will be found
        /// NOTE: Rotation order is: counterclockwise 90 degrees, clockwise 90 degrees, 180 degrees</param>
        /// <param name="face">Position of the face on the image in format [x, y, width, height].
        /// NOTE: Usage of this parameter disables face detector functionality</param>
        /// <param name="demographics">Pass "true" to include demographics information to response</param>
        /// <param name="attributes">Pass "true" to include face attributes information to response</param>
        /// <param name="landmarks">Pass "true" to include positions of face landmarks to response</param>
        /// <param name="liveness">Pass "true" to include probability of a live person photo to response</param>
        /// <returns>Tuple of the FaceWithInfo and rotation</returns>
        public async Task<Tuple<FaceWithInfo[], int>> Detect(byte[] image,
            float? fd_min_size = null, float? fd_max_size = null, float? fd_threshold = null,
            bool rotate_until_faces_found = false,
            int[] face = null, bool? demographics = null, bool? attributes = null,
            bool? landmarks = null, bool? liveness = null)
        {
            var content = JpegContent(image);

            var result = await PostBase<DetectResult>("detect", content, new
            {
                fd_min_size,
                fd_max_size,
                fd_threshold,
                rotate_until_faces_found,
                face,
                demographics,
                attributes,
                landmarks,
                liveness
            });

            switch (result.StatusCode)
            {
                case null:
                    throw new TevianException("Deserializing response failed.");
                case 200:
                    return new Tuple<FaceWithInfo[], int>(result.Data, result.Rotation);
                default:
                    throw new TevianException(result.Message ?? "Unknown error. StatusCode = " + result.StatusCode);
            }
        }

        /// <summary>
        /// Validates that the same person is present on two images
        /// </summary>
        /// <remarks>
        /// If given image has more than one face detected and face parameter isn't used, the largest face will be chosen.
        /// If there are multiple, one with the largest score is preferred
        /// </remarks>
        /// <param name="image1">First image</param>
        /// <param name="image2">Second image</param>
        /// <param name="fd_min_size">Minimal size of face that detector will search</param>
        /// <param name="fd_max_size">Maximal size of face that detector will search.
        /// NOTE: zero value disables this constraint</param>
        /// <param name="fd_threshold">Score threshold to be applied on found faces.
        /// NOTE: smaller values increase detector recall, but decrease precision</param>
        /// <param name="rotate_until_faces_found">Apply rotations to the input image until some faces will be found
        /// NOTE: Rotation order is: counterclockwise 90 degrees, clockwise 90 degrees, 180 degrees</param>
        /// <param name="face1">Position of the face on the first image in format [x, y, width, height].
        /// NOTE: Usage of this parameter disables face detector functionality for the first image</param>
        /// <param name="face2">Position of the face on the second image in format [x, y, width, height].
        /// NOTE: Usage of this parameter disables face detector functionality for the second image</param>
        /// <returns>Match</returns>
        public async Task<MatchResult> Match(byte[] image1, byte[] image2,
            float? fd_min_size = null, float? fd_max_size = null, float? fd_threshold = null,
            bool rotate_until_faces_found = false,
            int[] face1 = null, int[] face2 = null
        )
        {
            var content = new MultipartFormDataContent
            {
                {JpegContent(image1), "image1", "image1.jpeg"},
                {JpegContent(image2), "image2", "image2.jpeg"}
            };

            return await Post<MatchResult>("match", content, new
            {
                fd_min_size,
                fd_max_size,
                fd_threshold,
                rotate_until_faces_found,
                face1,
                face2
            });
        }
    }
}
