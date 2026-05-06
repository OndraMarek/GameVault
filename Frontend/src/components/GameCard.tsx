import { useState } from 'react';
import type { GameDetail } from '../pages/Home';
import { Link } from 'react-router-dom';

interface RawgSearchResult {
  id: number;
  name: string;
  background_image: string;
  released: string;
}

function GameCard({
  id,
  title,
  platformNames,
  playtime,
  coverImageUrl,
  onCoverUpdated,
  onGameDeleted,
}: GameDetail) {
  const [searchResults, setSearchResults] = useState<RawgSearchResult[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  const handleSearch = async () => {
    try {
      setIsLoading(true);
      const response = await fetch(
        `https://localhost:7154/api/search/${title}`,
      );

      if (response.ok) {
        const data = await response.json();
        setSearchResults(data.slice(0, 4));
      }
    } catch (error) {
      console.error('Failed to connect to backend:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleSelectCover = async (selectedGame: RawgSearchResult) => {
    try {
      const response = await fetch(`https://localhost:7154/api/mygames/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          RawgId: selectedGame.id,
          Title: title,
          Platforms: platformNames,
          PlaytimeHours: playtime,
          CoverImageUrl: selectedGame.background_image,
        }),
      });

      if (response.ok) {
        onCoverUpdated(id, selectedGame.background_image);
        setSearchResults([]);
      }
    } catch (error) {
      console.error('Failed to connect to backend:', error);
    }
  };

  const handleDelete = async () => {
    try {
      const response = await fetch(`https://localhost:7154/api/mygames/${id}`, {
        method: 'DELETE',
      });

      if (response.ok) {
        onGameDeleted(id);
      }
    } catch (error) {
      console.error('Failed to connect to backend:', error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="group relative w-full aspect-[2/3] rounded-lg shadow-md overflow-hidden bg-cyan-800">
      {coverImageUrl ? (
        <img
          src={coverImageUrl}
          className="absolute inset-0 w-full h-full object-cover transition-all 
                    duration-300 group-hover:blur-sm group-hover:brightness-50"
          alt={title}
        />
      ) : (
        <img
          src={
            'https://media.rawg.io/media/games/b4e/b4e4c73d5aa4ec66bbf75375c4847a2b.jpg'
          }
          className="absolute inset-0 w-full h-full object-cover transition-all
                     duration-300 group-hover:blur-sm group-hover:brightness-50"
          alt={title}
        />
      )}

      <div
        className="absolute inset-0 opacity-0 group-hover:opacity-100 transition-opacity
             duration-300 text-white p-4 flex flex-col items-center justify-center text-center"
      >
        <h3 className="font-bold text-lg">{title}</h3>

        <p>{platformNames.join(', ')}</p>
        <p>{playtime}</p>
        <Link
          to={`/game/${id}`}
          className="mt-4 bg-blue-600 hover:bg-blue-500 text-white rounded px-4 py-2 transition-colors block text-center"
        >
          Detail
        </Link>

        <button
          onClick={handleDelete}
          className="mt-4 bg-red-600 hover:bg-red-500 text-white rounded px-4 py-2 transition-colors"
        >
          Delete
        </button>

        {!coverImageUrl &&
          (!isLoading ? (
            <button
              onClick={handleSearch}
              className="mt-4 bg-cyan-500 hover:bg-cyan-400 text-white rounded px-4 py-2 transition-colors"
            >
              Search cover
            </button>
          ) : (
            <p>Loading...</p>
          ))}

        <div className="mt-2">
          {searchResults.map((res) => (
            <p
              onClick={() => handleSelectCover(res)}
              key={res.id}
              className="text-sm cursor-pointer hover:text-cyan-300 transition-colors"
            >
              {res.name}
            </p>
          ))}
        </div>
      </div>
    </div>
  );
}

export default GameCard;
