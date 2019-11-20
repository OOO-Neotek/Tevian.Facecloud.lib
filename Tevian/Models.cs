using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tevian
{
    public class Age
    {
        public int Mean { get; set; }
        public int Variance { get; set; }
    }

    public class Attributes
    {
        public string FacialHair { get; set; }
        public string Glasses { get; set; }
        public string HairColor { get; set; }
        public string HairType { get; set; }
        public string Headwear { get; set; }
    }

    public class Bbox
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class Database
    {
        public dynamic Data { get; set; }
        public int Id { get; set; }
    }

    public class Demographics
    {
        public Age Age { get; set; }
        public string Ethnicity { get; set; }
        public string Gender { get; set; }
    }

    public class Face
    {
        public Bbox Bbox { get; set; }
        public int Score { get; set; }
    }

    public class FaceWithInfo
    {
        public Attributes Attributes { get; set; }
        public Bbox Bbox { get; set; }
        public Demographics Demographics { get; set; }
        public Point[] Landmarks { get; set; }
        public int Liveness { get; set; }
        public int Score { get; set; }
    }

    public class _Person
    {
        public int Id { get; set; }
    }

    public class _Photo
    {
        public int Id { get; set; }
    }

    public class _Database
    {
        public int Id { get; set; }
    }

    public class IdentifyResult
    {
        public Face Face { get; set; }

        public class Match
        {
            public _Person Person { get; set; }
            public _Photo Photo { get; set; }
            public int Score { get; set; }
        }

        public Match[] matches { get; set; }
    }

    public class MatchResult
    {
        public Bbox Face1Bbox { get; set; }
        public Bbox Face2Bbox { get; set; }
        public int Score { get; set; }
    }

    public class PersonMatch
    {
        public Bbox FaceBbox { get; set; }
        public _Photo Photo { get; set; }
        public int Score { get; set; }
    }

    public class Person
    {
        public dynamic Data { get; set; }
        public _Database Database { get; set; }
        public int database_id { get; set; }
        public int Id { get; set; }
    }

    public class Photo
    {
        public Bbox FaceBbox { get; set; }
        public int Id { get; set; }
        public _Person Person { get; set; }
    }

    public class User
    {
        public string BillingType { get; set; }
        public dynamic Data { get; set; }
        public string Email { get; set; }
        public int? Id { get; set; }
        public string Password { get; set; }
    }
}
