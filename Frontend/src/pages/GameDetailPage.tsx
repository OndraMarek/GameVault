import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router';

export interface GameDetailDto {
  id: string;
  rawgId: number;
  title: string;
  platformNames: string[];
  hasPlayed: boolean;
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
  }, [id]);

  if (!game) return <div>Loading game details...</div>;

  return (
    <div
      className="relative text-black text-3xl text-center p-50 bg-cover bg-center min-h-[400px] flex flex-col justify-center"
      style={{ backgroundImage: `url(${game.coverImageUrl})` }}
    >
      <Link
        to={`/`}
        className="absolute top-4 left-4 bg-blue-600 hover:bg-blue-500 text-white rounded px-4 py-2 transition-colors"
      >
        Home
      </Link>
      <h1 className="text-5xl font-bold text-white text-shadow-lg/30">
        {game.title}
      </h1>
    </div>
  );
}

export default GameDetailPage;
