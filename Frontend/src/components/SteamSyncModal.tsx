import { useState, useEffect } from 'react';

interface SteamSyncModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSaveSuccess: () => void;
}

function SteamSyncModal({
  isOpen,
  onClose,
  onSaveSuccess,
}: SteamSyncModalProps) {
  const [steamId, setSteamId] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (isOpen) {
      setSteamId('');
      setIsLoading(false);
    }
  }, [isOpen]);

  if (!isOpen) return null;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);

    try {
      const response = await fetch(
        `https://localhost:7154/api/sync/steam/${steamId}`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
        },
      );

      if (response.ok) {
        onSaveSuccess();
        onClose();
      }
    } catch (error) {
      console.error('Failed to connect to backend:', error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black/70 flex items-center justify-center z-50 p-4">
      <div className="bg-sky-900 rounded-xl shadow-2xl p-6 w-full max-w-md text-left text-white">
        <h2 className="text-2xl font-bold mb-4 border-b border-sky-700 pb-2">
          Sync with Steam
        </h2>

        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <label className="flex flex-col text-sm font-medium">
            Steam ID (17-digit number):
            <input
              type="text"
              required
              value={steamId}
              onChange={(e) => setSteamId(e.target.value)}
              placeholder="e.g. 76561197960287930"
              className="mt-1 p-2 rounded bg-sky-950 border border-sky-700 focus:outline-none focus:border-cyan-400"
            />
          </label>

          <p className="text-xs text-sky-300">
            Don't know your Steam ID? You can find it using{' '}
            <a
              href="https://steamid.io/"
              target="_blank"
              rel="noopener noreferrer"
              className="text-cyan-400 underline hover:text-cyan-300"
            >
              steamid.io
            </a>
            .
          </p>

          <div className="mt-6 flex justify-end gap-3">
            <button
              type="button"
              onClick={onClose}
              disabled={isLoading}
              className="px-4 py-2 bg-gray-600 hover:bg-gray-500 rounded transition-colors disabled:opacity-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isLoading}
              className="px-4 py-2 bg-blue-600 hover:bg-blue-500 text-white rounded font-bold transition-colors shadow-lg disabled:opacity-50"
            >
              {isLoading ? 'Syncing...' : 'Sync'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default SteamSyncModal;
