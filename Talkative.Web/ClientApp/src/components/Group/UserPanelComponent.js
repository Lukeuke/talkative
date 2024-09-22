import {useState} from "react";
import {UserSearchPopup} from "./UserSearchPopup";

export const UserPanel = ({ users, roomId }) => {
  const [isPopupOpen, setIsPopupOpen] = useState(false);

  return (
      <div className="w-64 bg-MainDark border-l border-gray-700 p-4 md:block hidden overflow-y-auto">
        <h2 className="text-lg font-bold text-white mb-2">Users</h2>
        <button
            className="bg-blue-500 text-white p-2 rounded mb-4"
            onClick={() => setIsPopupOpen(true)}
        >
          Invite Users
        </button>
        <ul className="space-y-2">
          {users.map(user => (
              <li key={user.id} className="text-white hover:bg-SemiDark p-2 rounded user-list">
                {user.fullName} ({user.username})
              </li>
          ))}
        </ul>
        {isPopupOpen && (
            <UserSearchPopup
                onClose={() => setIsPopupOpen(false)}
                users={users}
                roomId={roomId}
            />
        )}
      </div>
  );
};
