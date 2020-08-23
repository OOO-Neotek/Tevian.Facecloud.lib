using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Tevian
{
    public partial class Tevian
    {
        /// <summary>
        /// Retrieves list of databases by pages
        /// </summary>
        /// <param name="page">page number in 0-indexation</param>
        /// <param name="per_page">number of items to show on one page</param>
        /// <returns></returns>
        public async Task<Tuple<Database[], Pagination>> GetDatabases(int page, int per_page)
        {
            var r = await Get<Database[]>("databases", new {page, per_page});
            return Tuple.Create(r.Data, r.Pagination);
        }

        /// <summary>
        /// Creates new database
        /// </summary>
        /// <param name="data">arbitrary data to save along</param>
        /// <returns></returns>
        public async Task<Database> CreateDatabase(object data)
        {
            var content = new StringContent(
                Serialize(new {data}),
                Encoding.UTF8, "application/json");
            return await Post<Database>("databases", content, goodStatus: 201);
        }

        /// <summary>
        /// Deletes database by its id.
        /// WARNING: All connected persons, person tags and photos will be removed
        /// </summary>
        /// <param name="databaseId">unique identifier of database</param>
        /// <returns></returns>
        public async Task DeleteDatabase(long databaseId)
        {
            await Delete($"databases/{databaseId}");
        }

        /// <summary>
        /// Reads database by its id
        /// </summary>
        /// <param name="databaseId">unique identifier of database</param>
        /// <returns></returns>
        public async Task<Database> GetDatabase(long databaseId)
        {
            return (await Get<Database>($"databases/{databaseId}")).Data;
        }

        /// <summary>
        /// Updates database by its id
        /// </summary>
        /// <param name="databaseId">unique identifier of database</param>
        /// <param name="data">arbitrary data to save along</param>
        /// <returns></returns>
        public async Task<Database> UpdateDatabase(long databaseId, object data)
        {
            var content = new StringContent(
                Serialize(new {data}),
                Encoding.UTF8, "application/json");
            return await Post<Database>($"databases/{databaseId}", content);
        }

        /// <summary>
        /// Searches face from the given image in the database specified by id.
        /// NOTE: If given image has more than one face detected and face parameter isn't used, the largest face will be chosen. If there are multiple, one with the largest score is preferred
        /// </summary>
        /// <param name="image">source image</param>
        /// <param name="databaseId">unique identifier of database</param>
        /// <param name="fr_threshold">face similiarity threshold</param>
        /// <param name="fd_min_size">minimal size of face that detector will search</param>
        /// <param name="fd_max_size">maximal size of face that detector will search.
        /// NOTE: zero value disables this constraint</param>
        /// <param name="fd_threshold">score threshold to be applied on found faces.
        /// NOTE: smaller values increase detector recall, but decrease precision</param>
        /// <param name="rotate_until_faces_found">Apply rotations to the input image until some faces will be found
        /// NOTE: Rotation order is: counterclockwise 90 degrees, clockwise 90 degrees, 180 degrees</param>
        /// <param name="face">position of the face on the image in format [x, y, width, height].
        /// NOTE: Usage of this parameter disables face detector functionality</param>
        /// <param name="fr_rank">number of most similar matches to retrieve</param>
        /// <returns></returns>
        public async Task<IdentifyResult> DatabaseSearchFace(byte[] image, long databaseId,
            float? fr_threshold = null,
            int? fd_min_size = null, int? fd_max_size = null,
            float? fd_threshold = null,
            bool rotate_until_faces_found = false,
            int[] face = null,
            int? fr_rank = null)
        {
            var content = JpegContent(image);

            return await Post<IdentifyResult>($"databases/{databaseId}/identify", content, new
            {
                fr_threshold,
                fd_min_size,
                fd_max_size,
                fd_threshold,
                rotate_until_faces_found,
                face,
                fr_rank
            });
        }
    }
}
