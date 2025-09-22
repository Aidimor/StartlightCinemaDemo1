using UnityEngine;

public class IntermissionScript : MonoBehaviour
{
    public GameCodesMain _scriptMainCodes;

    public void InstantiateVoid()
    {
        _scriptMainCodes.InstantiateGameAssets();
    }

    public void GameEnds()
    {
        _scriptMainCodes.NextGameVoid();
    }

    // Looping sounds
    public void FilmStartSound1()
    {
        _scriptMainCodes._scriptMain._scriptSXF.PlayStoppableSound("Proyector");
    }

    public void FilmStartStop1()
    {
        _scriptMainCodes._scriptMain._scriptSXF.StopStoppableSound("Proyector");
    }

    public void FilmStartSound2()
    {
        _scriptMainCodes._scriptMain._scriptSXF.PlayStoppableSound("ProyectorEnd");
    }

    public void FilmStartStop2()
    {
        _scriptMainCodes._scriptMain._scriptSXF.StopStoppableSound("Proyector");
    }

    // One-shot sounds
    public void CacletaSound1()
    {
        _scriptMainCodes._scriptMain._scriptSXF.PlaySound("Action");
    }

    public void CacletaSound2()
    {
        _scriptMainCodes._scriptMain._scriptSXF.PlaySound("ClapperSound");
    }

    public void GameStarts()
    {
        _scriptMainCodes._gameStarts = true;
    }
}
