namespace Tevian
{
    public partial class Tevian
    {
        public Tevian(string email, string passw)
        {
            jwt = Login(email, passw).Result;
            if (string.IsNullOrEmpty(jwt))
                throw new TevianException("Login error.");
        }
    }
}
