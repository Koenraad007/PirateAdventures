using System.Collections.Generic;

namespace PirateAdventures.Managers
{
    public class LevelManager
    {
        private static LevelManager _instance;
        public static LevelManager Instance => _instance ??= new LevelManager();

        private readonly List<string> _levelPaths;
        private int _currentLevelIndex;

        private LevelManager()
        {
            _levelPaths = new List<string>
            {
                "./../../../Content/Level1.tmx",
                "./../../../Content/Level2.tmx"
            };
            _currentLevelIndex = 0;
        }

        public string GetCurrentLevel() => _levelPaths[_currentLevelIndex];

        public string GetNextLevel()
        {
            if (_currentLevelIndex < _levelPaths.Count - 1)
            {
                _currentLevelIndex++;
                return _levelPaths[_currentLevelIndex];
            }
            return null;
        }

        public void ResetToFirstLevel() => _currentLevelIndex = 0;

        public bool HasNextLevel() => _currentLevelIndex < _levelPaths.Count - 1;

        public int CurrentLevelNumber => _currentLevelIndex + 1;
        public int TotalLevels => _levelPaths.Count;
    }
}