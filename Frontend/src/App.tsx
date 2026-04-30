import { useState, useEffect } from 'react';
import './App.css';
import GameCard from './components/GameCard';

export interface GameDetail {
    id: string;
    title: string;
    platformName: string;
    playtime: number;
}

function App() {
  const [myGames, setMyGames] = useState<GameDetail[]>([]);

  useEffect(() => {
    const fetchGames = async () => {
      try {
        const response = await fetch('https://localhost:7154/api/mygames'); 
        
        if (response.ok) {
          const data = await response.json();
          setMyGames(data);
        }
      } catch (error) {
        console.error("Failed to connect to backend:", error);
      }
    };

    fetchGames();
  }, []);

  return (
    <div className="app-container">
      <h1>My Game Vault</h1>
      
      {myGames.length === 0 ? (
        <p>Loading games from the server...</p>
      ) : (
        <div className="games-grid">
          {myGames.map((game) => (
            <GameCard 
              key={game.id} 
              id={game.id} 
              title={game.title} 
              platformName={game.platformName} 
              playtime={game.playtime} 
            />
          ))}
        </div>
      )}
    </div>
  );
}

export default App;