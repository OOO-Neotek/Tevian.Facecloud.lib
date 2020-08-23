using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tevian
{
    public partial class Tevian
    {
        /// <summary>
        /// Retrieves list of persons by pages
        /// </summary>
        /// <param name="page">page number in 0-indexation</param>
        /// <param name="per_page">number of items to show on one page</param>
        /// <returns></returns>
        public async Task<Tuple<Person[], Pagination>> GetPersons(int page, int per_page)
        {
            var r = await Get<Person[]>("persons", new {page, per_page});
            return Tuple.Create(r.Data, r.Pagination);
        }

        /// <summary>
        /// Creates new person
        /// </summary>
        /// <param name="databaseId">unique identifier of database</param>
        /// <param name="data">arbitrary data to save along</param>
        /// <returns></returns>
        public async Task<Person> CreatePerson(long databaseId, object data)
        {
            var content = new StringContent(
                Serialize(new {data, databaseId}),
                Encoding.UTF8, "application/json");
            return await Post<Person>("persons", content, goodStatus: 201);
        }

        /// <summary>
        /// Deletes person by its id.
        /// WARNING: All connected photos also will be removed
        /// </summary>
        /// <param name="personId">unique identifier of person</param>
        /// <returns></returns>
        public async Task DeletePerson(long personId)
        {
            await Delete($"persons/{personId}");
        }

        /// <summary>
        /// Reads person by its id
        /// </summary>
        /// <param name="personId">unique identifier of person</param>
        /// <returns></returns>
        public async Task<Person> GetPerson(long personId)
        {
            return (await Get<Person>($"persons/{personId}")).Data;
        }

        /// <summary>
        /// Updates person by its id
        /// </summary>
        /// <param name="personId">unique identifier of person</param>
        /// <param name="databaseId">unique identifier of database</param>
        /// <param name="data">arbitrary data to save along</param>
        /// <returns></returns>
        public async Task<Person> UpdatePerson(long personId, long databaseId, object data)
        {
            var content = new StringContent(
                Serialize(new {data, databaseId}),
                Encoding.UTF8, "application/json");
            return await Post<Person>($"persons/{personId}", content);
        }

        /// <summary>
        /// Matches face from the given image with the person specified by id.
        /// NOTE: If given image has more than one face detected and face parameter isn't used, the largest face will be chosen. If there are multiple, one with the largest score is preferred
        /// </summary>
        /// <param name="image">source image</param>
        /// <param name="personId">unique identifier of person</param>
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
        /// <returns>null if not matched</returns>
        public async Task<PersonMatchResult> MatchFaceWithPerson(byte[] image, long personId,
            float? fr_threshold = null,
            int? fd_min_size = null, int? fd_max_size = null,
            float? fd_threshold = null,
            bool rotate_until_faces_found = false,
            int[] face = null)
        {
            var content = JpegContent(image);

            return await Post<PersonMatchResult>($"persons/{personId}/match", content, new
            {
                fr_threshold,
                fd_min_size,
                fd_max_size,
                fd_threshold,
                rotate_until_faces_found,
                face
            });
        }
    }
}
