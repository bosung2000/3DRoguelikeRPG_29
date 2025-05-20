using System;

[Serializable]
public class GameData
{
    public int gold;
    public int soul;
    public bool isTutorialDone = false; // 최초 실행 시 false
}
