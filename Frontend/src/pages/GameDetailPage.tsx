import { useEffect, useState } from 'react';
import { useParams } from 'react-router';

export interface GameDetailDto {
  id: string;
  title: string;
  platformNames: string[];
  playtime: number;
  coverImageUrl?: string;
}

function GameDetailPage() {
  const { id } = useParams();
  const [game, setGame] = useState<GameDetailDto | null>(null);

  useEffect(() => {
    const fetchGame = async () => {
      try {
        const response = await fetch(
          `https://localhost:7154/api/mygames/${id}`,
        );

        if (response.ok) {
          const data = await response.json();
          setGame(data);
        }
      } catch (error) {
        console.error('Failed to connect to backend:', error);
      }
    };

    fetchGame();
  }, []);

  if (!game) return <div>Loading game details...</div>;

  return (
    <div
      className="text-black text-3xl text-center p-50 bg-cover bg-center"
      style={{ backgroundImage: `url(${game.coverImageUrl})` }}
    >
      <h1 className="text-5xl font-bold text-heading text-white text-shadow-lg/30">
        {game.title}
      </h1>
    </div>
  );
}

export default GameDetailPage;
