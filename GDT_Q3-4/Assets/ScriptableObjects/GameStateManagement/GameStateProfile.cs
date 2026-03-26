using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameState", menuName = "Game State Profile")]
public class GameStateProfile : ScriptableObject
{
    [System.Serializable]
    public struct InitialState
    {
        public string key;
        public bool value;
    }

    public List<InitialState> states;
}