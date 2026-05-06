import { Routes, Route } from 'react-router';
import Home from './pages/Home';
import GameDetailPage from './pages/GameDetailPage';

function App() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/game/:id" element={<GameDetailPage />} />
    </Routes>
  );
}

export default App;
