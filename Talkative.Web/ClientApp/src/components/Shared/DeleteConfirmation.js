import {useState} from "react";

const DeleteConfirmation = ({ isOpen, name, onClose, onConfirm }) => {
  const [inputValue, setInputValue] = useState('');

  const handleConfirm = () => {
    if (inputValue === name) {
      onConfirm();
      onClose();
    } else {
      alert('Name does not match. Please try again.');
    }
  };

  if (!isOpen) return null;

  return (
      <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50">
        <div className="bg-white p-5 rounded shadow-lg">
          <h2 className="text-lg font-bold">Confirm Deletion</h2>
          <p>Please type the "<strong>{name}</strong>" to confirm:</p>
          <input
              type="text"
              value={inputValue}
              onChange={(e) => setInputValue(e.target.value)}
              className="border p-1 rounded w-full mt-2"
          />
          <div className="mt-4">
            <button
                onClick={handleConfirm}
                className="bg-red-500 text-white px-4 py-2 rounded mr-2"
            >
              Delete
            </button>
            <button
                onClick={onClose}
                className="bg-gray-300 px-4 py-2 rounded"
            >
              Cancel
            </button>
          </div>
        </div>
      </div>
  );
};

export default DeleteConfirmation