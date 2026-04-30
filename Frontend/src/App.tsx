import './App.css'
import GameCard from './components/GameCard'

function App() {
  const myGames = [{id: "1", title: "Minecraft", platform: "Xbox", playTimeHours: 100},
    {id: "2", title: "Fortnite", platform: "EpicGames", playTimeHours: 100},
    {id: "3", title: "Witcher", platform: "Steam", playTimeHours: 100},
  ];


  return (
    <>
      {myGames.map((game) => (
        <GameCard key={game.id} id={game.id} title={game.title} platform={game.platform} playTimeHours={game.playTimeHours} />
      ))}
    </>
  )
}

export default App
