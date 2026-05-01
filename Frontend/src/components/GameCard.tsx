import { useState } from "react";
import type { GameDetail } from "../App";

interface RawgSearchResult{
    id: number,
    name: string,
    background_image: string,
    released: string
}

export const GameCard = ({title,platformNames,playtime,coverImageUrl}: GameDetail) => {

    const [searchResults, setSearchResults] = useState<RawgSearchResult[]>([]);

    const handleSearch = async () => { 
        try {
            const response = await fetch(`https://localhost:7154/api/search/${title}`); 
        
            if (response.ok) {
            const data = await response.json();
            setSearchResults(data);
            }
        } catch (error) {
        console.error("Failed to connect to backend:", error);
        }
    }

    return (
        <div className="group relative w-full aspect-[2/3] rounded-lg shadow-md overflow-hidden bg-cyan-800">
            {coverImageUrl ? <img src={coverImageUrl} className="absolute inset-0 w-full h-full object-cover" alt={title} /> : 
            <img src={"https://media.rawg.io/media/games/b4e/b4e4c73d5aa4ec66bbf75375c4847a2b.jpg"} className="absolute inset-0 w-full h-full object-cover" alt={title}/>}
            <div className="opacity-0 group-hover:opacity-100 transition-opacity duration-300 text-white absolute bottom-0 w-full p-4 flex flex-col justify-end bg-gradient-to-t from-black/90 to-transparent">
                <h3>{title}</h3>
                <p>{platformNames.join(", ")}</p>
                <p>{playtime}</p>
                {coverImageUrl ? <></> :
                <button onClick={handleSearch} className="bg-cyan-500 hover:bg-cyan-400 text-white rounded py-2">Search cover</button>}
                {searchResults.map(res => <p key={res.id}>{res.name}</p>)}
            </div>
        </div>
    );
};

export default GameCard;