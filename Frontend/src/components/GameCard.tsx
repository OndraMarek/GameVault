import type { GameDetail } from "../App";

export const GameCard = ({title,platformNames,playtime,coverImageUrl}: GameDetail) => {
    return (
        <div className="relative w-full aspect-[2/3] rounded-lg shadow-md overflow-hidden bg-cyan-800">
            {coverImageUrl ? <img src={coverImageUrl} className="absolute inset-0 w-full h-full object-cover" alt={title} /> : 
            <img src={"https://media.rawg.io/media/games/b4e/b4e4c73d5aa4ec66bbf75375c4847a2b.jpg"} className="absolute inset-0 w-full h-full object-cover" alt={title}/>}
            <div className="text-white absolute bottom-0 w-full p-4 flex flex-col justify-end bg-gradient-to-t from-black/90 to-transparent">
                <h3>{title}</h3>
                <p>{platformNames.join(", ")}</p>
                <p>{playtime}</p>
            </div>
        </div>
    );
};

export default GameCard;