namespace awes0mecoderz.Retarder
{
    public interface IRetarder 
    {
        bool HangOn();

        bool HangOn(int timeoutInMillis);
    }
}