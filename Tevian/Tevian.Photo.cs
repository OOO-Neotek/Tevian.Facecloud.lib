using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tevian
{
    public partial class Tevian
    {
        /// <summary>
        /// Retrieves list of photos by pages
        /// </summary>
        /// <param name="page">page number in 0-indexation</param>
        /// <param name="per_page">number of items to show on one page</param>
        /// <returns></returns>
        public async Task<Tuple<Photo[], Pagination>> GetPhotos(int page, int per_page)
        {
            var r = await Get<Photo[]>("photos", new {page, per_page});
            return Tuple.Create(r.Data, r.Pagination);
        }

        /// <summary>
        /// Creates new photo
        /// </summary>
        /// <param name="image">arbitrary data to save along</param>
        /// <param name="person_id">unique identifier of person</param>
        /// <param name="fd_min_size">minimal size of face that detector will search</param>
        /// <param name="fd_max_size">maximal size of face that detector will search.
        /// NOTE: zero value disables this constraint</param>
        /// <param name="fd_threshold">score threshold to be applied on found faces.
        /// NOTE: smaller values increase detector recall, but decrease precision</param>
        /// <param name="rotate_until_faces_found">Apply rotations to the input image until some faces will be found
        /// NOTE: Rotation order is: counterclockwise 90 degrees, clockwise 90 degrees, 180 degrees</param>
        /// <param name="face">position of the face on the image in format [x, y, width, height].
        /// NOTE: Usage of this parameter disables face detector functionality</param>
        /// <returns></returns>
        public async Task<Photo> CreatePhoto(byte[] image, long person_id,
            int? fd_min_size = null, int? fd_max_size = null,
            float? fd_threshold = null,
            bool rotate_until_faces_found = false,
            int[] face = null)
        {
            var content = JpegContent(image);
            return await Post<Photo>("photos", content, new
            {
                person_id,
                fd_min_size,
                fd_max_size,
                fd_threshold,
                rotate_until_faces_found,
                face
            }, 201);
        }

        /// <summary>
        /// Deletes photos by its id.
        /// </summary>
        /// <param name="photoId">unique identifier of photo</param>
        /// <returns></returns>
        public async Task DeletePhoto(long photoId)
        {
            await Delete($"photos/{photoId}");
        }

        /// <summary>
        /// Reads photo by its id
        /// </summary>
        /// <param name="photoId">unique identifier of photo</param>
        /// <returns></returns>
        public async Task<Photo> GetPhoto(long photoId)
        {
            return (await Get<Photo>($"photos/{photoId}")).Data;
        }

        /// <summary>
        /// Returns jpeg-encoded cropped image of the face associated with photo by its id
        /// </summary>
        /// <param name="photoId">unique identifier of photo</param>
        /// <param name="width">image width</param>
        /// <param name="height">image height</param>
        /// <returns></returns>
        public async Task<byte[]> GetFaceFromPhoto(long photoId, int? width = null, int? height = null)
        {
            return (await Get<byte[]>($"photos/{photoId}/image/face", new {width, height})).Data;
        }

        /// <summary>
        /// Returns jpeg-encoded image of the photo by its id
        /// </summary>
        /// <param name="photoId">unique identifier of photo</param>
        /// <param name="width">image width</param>
        /// <param name="height">image height</param>
        /// <returns></returns>
        public async Task<byte[]> GetImageFromPhoto(long photoId, int? width = null, int? height = null)
        {
            return (await Get<byte[]>($"photos/{photoId}/image/full", new { width, height })).Data;
        }

    }
}
