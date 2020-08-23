using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tevian
{
    /// <summary>
    /// Person age prediction as normal distribution parameters
    /// </summary>
    public class Age
    {
        /// <value>Normal distribution mean</value>
        public double Mean { get; set; }

        /// <value>Normal distribution variance. NOTE: to get standard deviation apply square root</value>
        public double Variance { get; set; }
    }

    /// <summary>
    /// Face attributes/features
    /// </summary>
    public class Attributes
    {
        /// <value>Type of facial hair. [ unknown, beard, bristle, gm, goatee, mustache, shaved ]</value>
        public string FacialHair { get; set; }

        /// <value>Type of glasses. [ unknown, dark, none, usual ]</value>
        public string Glasses { get; set; }

        /// <value>Hair color. [ unknown, black, blond, brown, gray ]</value>
        public string HairColor { get; set; }

        /// <value>Type of hair. [ unknown, bald, high_temple, normal ]</value>
        public string HairType { get; set; }

        /// <value>Type of headwear. [ unknown, b-cap, band, beret, cap, ear_flap, fur_hood, glasses, hat, helmet, hood, kepi, kerchief, no, peaked_cap ]</value>
        public string Headwear { get; set; }
    }

    /// <summary>
    /// Position of the bounding box on the image
    /// </summary>
    public class Bbox
    {
        /// <value>Height of the bounding box in pixels</value>
        public int Height { get; set; }

        /// <value>Width of the bounding box in pixels</value>
        public int Width { get; set; }

        /// <value>x-coordinate of the top-left pixel</value>
        public int X { get; set; }

        /// <value>y-coordinate of the top-left pixel</value>
        public int Y { get; set; }
    }

    /// <summary>
    /// Specifies pixel position on the image
    /// </summary>
    public class Point
    {
        /// <value>x-coordinate</value>
        public int X { get; set; }

        /// <value>y-coordinate</value>
        public int Y { get; set; }
    }

    /// <summary>
    /// Database information
    /// </summary>
    public class Database
    {
        /// <value>Arbitrary data to save along</value>
        public dynamic Data { get; set; }

        /// <value>Unique identifier</value>
        public long Id { get; set; }
    }

    /// <summary>
    /// Person's demographics information
    /// </summary>
    public class Demographics
    {
        /// <value><see cref="Age"/></value>
        public Age Age { get; set; }

        /// <value>Ethnicity. [ unknown, asian, black, caucasian, east_indian ]</value>
        public string Ethnicity { get; set; }

        /// <value>Gender. [ unknown, female, male ]</value>
        public string Gender { get; set; }
    }

    /// <summary>
    /// Face information
    /// </summary>
    public class Face
    {
        /// <value><see cref="Bbox"/></value>
        public Bbox Bbox { get; set; }

        /// <value>Face detector confidence score. null if face bbox is manually provided</value>
        public double Score { get; set; }
    }

    /// <summary>
    /// Face information with attributes, landmarks or demographics
    /// </summary>
    public class FaceWithInfo
    {
        /// <value><see cref="Attributes"/></value>
        public Attributes Attributes { get; set; }

        /// <value><see cref="Bbox"/></value>
        public Bbox Bbox { get; set; }

        /// <value><see cref="Demographics"/></value>
        public Demographics Demographics { get; set; }

        /// <value>Positions of 29 landmarks on the face <see cref="Point"/></value>
        public Point[] Landmarks { get; set; }

        /// <value>Probability that a face is captured from a live photo</value>
        public double Liveness { get; set; }

        /// <value>Face detector confidence score. null if face bbox is manually provided</value>
        public double Score { get; set; }
    }

    public class _Person
    {
        /// <value>Unique person identifier</value>
        public long Id { get; set; }
    }

    public class _Photo
    {
        /// <value>Unique photo identifier</value>
        public long Id { get; set; }
    }

    public class _Database
    {
        /// <value>Unique database identifier</value>
        public long Id { get; set; }
    }

    /// <summary>
    /// Matches with database
    /// </summary>
    public class IdentifyResult
    {
        /// <value>Face on the request image <see cref="Face"/></value>
        public Face Face { get; set; }

        /// <summary>
        /// Single match with database
        /// </summary>
        public class Match
        {
            /// <value><see cref="_Person"/></value>
            public _Person Person { get; set; }

            /// <value><see cref="_Photo"/></value>
            public _Photo Photo { get; set; }

            /// <value>Face similarity score</value>
            public double Score { get; set; }
        }

        /// <value>Matches with database. . <see cref="Match"/></value>
        public Match[] Matches { get; set; }

        /// <value>Rotation applied to the input image before face detection</value>
        public int Rotation { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class DetectResult
    {
        ///  <value>Face information with attributes, landmarks or demographics</value>
        public FaceWithInfo[] Data { get; set; }
        /// <value>Rotation applied to the input image before face detection</value>
        public int Rotation { get; set; }
        public int? StatusCode { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Two faces similarity
    /// </summary>
    public class MatchResult
    {
        /// <value>Position of the face on the first request image. <see cref="Face"/></value>
        public Face Face1 { get; set; }

        /// <value>Position of the face on the second request image. <see cref="Face"/></value>
        public Face Face2 { get; set; }

        /// <value>Face similarity score</value>
        public double Score { get; set; }

        /// <value>Rotation applied to the input image before face detection</value>
        public int Rotation1 { get; set; }

        /// <value>Rotation applied to the input image before face detection</value>
        public int Rotation2 { get; set; }
    }

    /// <summary>
    /// Match with the person
    /// </summary>
    public class PersonMatchResult
    {
        /// <value>Position of the face on the request image. <see cref="Bbox"/></value>
        public Bbox FaceBbox { get; set; }

        /// <value><see cref="_Photo"/></value>
        public _Photo Photo { get; set; }

        /// <value>Face similarity score</value>
        public double Score { get; set; }

        /// <value>Rotation applied to the input image before face detection</value>
        public int Rotation { get; set; }
    }

    /// <summary>
    /// Person information
    /// </summary>
    public class Person
    {
        /// <value>Arbitrary data to save along</value>
        public dynamic Data { get; set; }

        /// <value><see cref="_Database"/></value>
        public _Database Database { get; set; }

        /// <value>Sets database_id</value>
        public int database_id { get; set; }

        /// <value>Unique identifier</value>
        public long Id { get; set; }
    }

    /// <summary>
    /// Photo information
    /// </summary>
    public class Photo
    {
        /// <value>Position of persons face on the image. <see cref="Bbox"/></value>
        public Bbox FaceBbox { get; set; }

        /// <value>Unique identifier</value>
        public long Id { get; set; }

        /// <value>Associated person</value>
        public _Person Person { get; set; }
    }

    /// <summary>
    /// User information
    /// </summary>
    public class User
    {
        /// <value>Billing plan</value>
        public string BillingType { get; set; }

        /// <value>Arbitrary data to save along</value>
        public dynamic Data { get; set; }

        /// <value>Email</value>
        public string Email { get; set; }

        /// <value>Unique identifier</value>
        public long? Id { get; set; }

        /// <value>Password</value>
        public string Password { get; set; }
    }

    /// <summary>
    /// Pagination state
    /// </summary>
    public class Pagination
    {
        /// <value>Current page number in 0-indexation</value>
        public int Page { get; set; }

        /// <value>Number of items to show per page</value>
        public int PerPage { get; set; }

        /// <value>Number of items on all pages</value>
        public int TotalItems { get; set; }

        /// <value>Total number of pages</value>
        public int TotalPages { get; set; }
    }
}
