using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mbNES
{
    public class Game
    {
        private bool _gameRunning = false;                  // Start up the game instance with the game stopped
        public Game()
        {

        }

        public async void asyncSetupGame()
        {
            while (true)
            {
                if (_gameRunning)                             // The main game loop
                {
                    
                    CPU.nesCPU.ExecuteInstruction();
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        public void StartGame()
        {
            _gameRunning = true;
        }

        public void StopGame()
        {
            _gameRunning = false;
        }

        public bool IsRunning()
        {
            return _gameRunning;
        }

    }
}
