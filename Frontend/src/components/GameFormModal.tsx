import { useState, useEffect } from 'react';
import type { GameDetail } from '../pages/Home';

interface GameFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  initialData?: GameDetail | null;
  onSaveSuccess: () => void;
}

export default function GameFormModal({
  isOpen,
  onClose,
  initialData,
  onSaveSuccess,
}: GameFormModalProps) {
  const [title, setTitle] = useState('');
  const [platforms, setPlatforms] = useState('');
  const [playtime, setPlaytime] = useState<number>(0);
  const [coverUrl, setCoverUrl] = useState('');

  useEffect(() => {
    if (initialData) {
      setTitle(initialData.title);
      setPlatforms(initialData.platformNames.join(', '));
      setPlaytime(initialData.playtime);
      setCoverUrl(initialData.coverImageUrl || '');
    } else {
      setTitle('');
      setPlatforms('');
      setPlaytime(0);
      setCoverUrl('');
    }
  }, [initialData, isOpen]);

  if (!isOpen) return null;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const platformArray = platforms
      .split(',')
      .map((p) => p.trim())
      .filter((p) => p !== '');

    if (initialData) {
      try {
        const response = await fetch(
          `https://localhost:7154/api/mygames/${initialData.id}`,
          {
            method: 'PUT',
            headers: {
              'Content-Type': 'application/json',
            },
            body: JSON.stringify({
              RawgId: initialData.rawgId || null,
              Title: title,
              Platforms: platformArray,
              PlaytimeHours: playtime,
              CoverImageUrl: coverUrl,
            }),
          },
        );

        if (response.ok) {
          onSaveSuccess();
          onClose();
        }
      } catch (error) {
        console.error('Failed to connect to backend:', error);
      }
    } else {
      try {
        const response = await fetch(`https://localhost:7154/api/mygames`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            RawgId: null,
            Title: title,
            Platforms: platformArray,
            PlaytimeHours: playtime,
            CoverImageUrl: coverUrl,
          }),
        });

        if (response.ok) {
          onSaveSuccess();
          onClose();
        }
      } catch (error) {
        console.error('Failed to connect to backend:', error);
      }
    }
  };

  return (
    <div className="fixed inset-0 bg-black/70 flex items-center justify-center z-50 p-4">
      <div className="bg-sky-900 rounded-xl shadow-2xl p-6 w-full max-w-md text-left text-white">
        <h2 className="text-2xl font-bold mb-4 border-b border-sky-700 pb-2">
          {initialData ? 'Edit Game' : 'Add New Game'}
        </h2>
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <label className="flex flex-col text-sm font-medium">
            Title:
            <input
              type="text"
              required
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              className="mt-1 p-2 rounded bg-sky-950 border border-sky-700 focus:outline-none focus:border-cyan-400"
            />
          </label>

          <label className="flex flex-col text-sm font-medium">
            Platforms (comma separated):
            <input
              type="text"
              value={platforms}
              onChange={(e) => setPlatforms(e.target.value)}
              placeholder="e.g. Steam, Xbox"
              className="mt-1 p-2 rounded bg-sky-950 border border-sky-700 focus:outline-none focus:border-cyan-400"
            />
          </label>

          <label className="flex flex-col text-sm font-medium">
            Playtime (hours):
            <input
              type="number"
              min="0"
              value={playtime}
              onChange={(e) => setPlaytime(Number(e.target.value))}
              className="mt-1 p-2 rounded bg-sky-950 border border-sky-700 focus:outline-none focus:border-cyan-400"
            />
          </label>

          <label className="flex flex-col text-sm font-medium">
            Cover Image URL:
            <input
              type="text"
              value={coverUrl}
              onChange={(e) => setCoverUrl(e.target.value)}
              className="mt-1 p-2 rounded bg-sky-950 border border-sky-700 focus:outline-none focus:border-cyan-400"
            />
          </label>

          <div className="mt-6 flex justify-end gap-3">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 bg-gray-600 hover:bg-gray-500 rounded transition-colors"
            >
              Cancel
            </button>
            <button
              type="submit"
              className="px-4 py-2 bg-blue-600 hover:bg-blue-500 text-white rounded font-bold transition-colors shadow-lg"
            >
              Save
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
