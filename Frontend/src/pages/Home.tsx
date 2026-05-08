import { useState, useEffect } from 'react';
import GameCard from '../components/GameCard';
import GameFormModal from '../components/GameFormModal';

export interface GameDetail {
  id: string;
  rawgId: number;
  title: string;
  platformNames: string[];
  playtime: number;
  coverImageUrl?: string;
}

function Home() {
  const [myGames, setMyGames] = useState<GameDetail[]>([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingGame, setEditingGame] = useState<GameDetail | null>(null);

  const fetchGames = async () => {
    try {
      const response = await fetch('https://localhost:7154/api/mygames');

      if (response.ok) {
        const data = await response.json();
        setMyGames(data);
      }
    } catch (error) {
      console.error('Failed to connect to backend:', error);
    }
  };

  useEffect(() => {
    fetchGames();
  }, []);

  const handleCoverUpdated = (gameId: string, newCoverUrl: string) => {
    setMyGames((prevGames) =>
      prevGames.map((game) =>
        game.id === gameId ? { ...game, coverImageUrl: newCoverUrl } : game,
      ),
    );
  };

  const handleGameDelete = (gameId: string) => {
    setMyGames((prevGames) => prevGames.filter((game) => game.id !== gameId));
  };

  const handleOpenAddModal = () => {
    setEditingGame(null);
    setIsModalOpen(true);
  };

  const handleOpenEditModal = (game: GameDetail) => {
    setEditingGame(game);
    setIsModalOpen(true);
  };

  return (
    <div className="bg-sky-950 text-center">
      <h2 className="text-5xl font-bold text-heading text-white p-7">
        My Game Vault
      </h2>
      <button
        onClick={handleOpenAddModal}
        className="text-white px-4 py-2 bg-green-600 hover:bg-green-500 rounded transition-colors"
      >
        Add Game
      </button>

      {myGames.length === 0 ? (
        <p>Loading games from the server...</p>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-6 gap-x-8 gap-y-4 p-4">
          {myGames.map((game: GameDetail) => (
            <GameCard
              key={game.id}
              game={game}
              onCoverUpdated={handleCoverUpdated}
              onGameDeleted={handleGameDelete}
              onEditRequest={handleOpenEditModal}
            />
          ))}
        </div>
      )}
      <GameFormModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        initialData={editingGame}
        onSaveSuccess={() => {
          fetchGames();
        }}
      />
    </div>
  );
}

export default Home;
