import { useState } from "react";
import type { GameDetail } from "../App";

interface RawgSearchResult {
    id: number,
    name: string,
    background_image: string,
    released: string
}

export const GameCard = ({ title, platformNames, playtime, coverImageUrl }: GameDetail) => {

    const [searchResults, setSearchResults] = useState<RawgSearchResult[]>([]);

    const handleSearch = async () => {
        try {
            const response = await fetch(`https://localhost:7154/api/search/${title}`);

            if (response.ok) {
                const data = await response.json();
                setSearchResults(data.slice(0, 3))
            }
        } catch (error) {
            console.error("Failed to connect to backend:", error);
        }
    }

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
                    src={"https://media.rawg.io/media/games/b4e/b4e4c73d5aa4ec66bbf75375c4847a2b.jpg"}
                    className="absolute inset-0 w-full h-full object-cover transition-all
                     duration-300 group-hover:blur-sm group-hover:brightness-50"
                    alt={title}
                />
            )}

            <div className="absolute inset-0 opacity-0 group-hover:opacity-100 transition-opacity
             duration-300 text-white p-4 flex flex-col items-center justify-center text-center">
                <h3 className="font-bold text-lg">{title}</h3>
                <p>{platformNames.join(", ")}</p>
                <p>{playtime}</p>

                {!coverImageUrl && (
                    <button
                        onClick={handleSearch}
                        className="mt-4 bg-cyan-500 hover:bg-cyan-400 text-white rounded px-4 py-2 transition-colors">
                        Search cover
                    </button>
                )}

                <div className="mt-2">
                    {searchResults.map(res => (
                        <p key={res.id} className="text-sm">{res.name}</p>
                    ))}
                </div>
            </div>
        </div>
    );
};

export default GameCard;