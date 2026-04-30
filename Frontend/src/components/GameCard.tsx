interface GameDetail{
    id: string,
    title: string,
    platform: string,
    playTimeHours: number;
}

export const GameCard = ({title,platform,playTimeHours}: GameDetail) => {
    return (
        <div className="game-card">
            <h3>Title: {title}</h3>
            <p>Game platform: {platform}</p>
            <p>Number of hours played: {playTimeHours}</p>
        </div>
    );
};

export default GameCard;