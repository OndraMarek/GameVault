interface GameDetail{
    id: string,
    title: string,
    platformName: string,
    playtime: number;
}

export const GameCard = ({title,platformName,playtime}: GameDetail) => {
    return (
        <div className="game-card">
            <h3>Title: {title}</h3>
            <p>Game platform: {platformName}</p>
            <p>Number of hours played: {playtime}</p>
        </div>
    );
};

export default GameCard;