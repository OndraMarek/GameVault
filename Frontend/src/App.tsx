import { useState, useEffect } from 'react';
import GameCard from './components/GameCard';

export interface GameDetail {
    id: string;
    title: string;
    platformNames: string[];
    playtime: number;
    coverImageUrl?: string;
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
    <div className="bg-sky-950 text-center">
        <h2 className="text-5xl font-bold text-heading text-white p-7">My Game Vault</h2>
        
        {myGames.length === 0 ? (
          <p>Loading games from the server...</p>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-6 gap-x-8 gap-y-4 p-4">
            {myGames.map((game: GameDetail) => (
              <GameCard 
                key={game.id} 
                id={game.id} 
                title={game.title} 
                platformNames={game.platformNames} 
                playtime={game.playtime} 
              />
            ))} 
          </div>
        )}
      </div>
  );
}

export default App;