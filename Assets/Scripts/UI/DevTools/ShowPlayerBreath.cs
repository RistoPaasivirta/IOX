using UnityEngine;

public class ShowPlayerBreath : MonoBehaviour 
{
    /*bool active;

    public void Toggle()
    {
        Toggle(!active);
    }

    public void Toggle(bool state)
    {
        if (PlayerCharacter.Local == null)
        {
            active = false;
            return;
        }

        active = state;
        if (active)
        {
            PlayerCharacter.Local.GetComponent<ThingController>().GridPosChanged.AddListener(ShowBreath);
            ShowBreath(PlayerCharacter.playerBreath.startPos);
        }
        else
        {
            PlayerCharacter.Local.GetComponent<ThingController>().GridPosChanged.RemoveListener(ShowBreath);
            DevTools.ClearVisualizations();
        }
    }
    
    static void ShowBreath(Vec2I gridPos)
    {
        DevTools.ClearVisualizations();
        PlayerCharacter.playerBreath.BreathList.Perform((n)=>
        {
            float ratio = Vec2I.Distance(n.Data.gridPos, PlayerCharacter.playerBreath.startPos) / (float)PlayerCharacter.playerBreath.maxDistance;
            Color c = Color.Lerp(Color.green, Color.red, ratio);
            DevTools.VisualizeGridLine(n.Data.gridPos, n.Data.parent, c);
        });
    }*/
}
