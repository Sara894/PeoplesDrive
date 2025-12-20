using System;

public class MiscEvents
{
    public event Action onItemCollected;
    public void ItemCollected() 
    {
        if (onItemCollected != null) 
        {
            onItemCollected();
        }
    }

    //public event Action onGemCollected;
    /*public void GemCollected() 
    {
        if (onGemCollected != null) 
        {
            onGemCollected();
        }
    }*/
}
