interface GameDetail{
    id: string,
    title: string,
    platformName: string,
    playtime: number;
}

export const GameCard = ({title,platformName,playtime}: GameDetail) => {
    return (
        <div className="text-white text-xl font-bold bg-cyan-800 rounded-lg shadow-md p-4 w-full aspect-[2/3] 
        overflow-hidden flex flex-col justify-center items-center p-4 text-center">
            <h3>Title: {title}</h3>
            <p>Game platform: {platformName}</p>
            <p>Number of hours played: {playtime}</p>
        </div>
    );
};

export default GameCard;