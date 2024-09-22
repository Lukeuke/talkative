import {useEffect, useState} from "react";
import {UserSearchPopup} from "./UserSearchPopup";
import { createClient } from 'graphql-sse';

const client = createClient({
  url: '/graphql',
  headers: {
    Authorization: `Bearer ${localStorage.getItem('token')}`,
  }
});

export const UserPanel = ({ users, roomId }) => {
  const [isPopupOpen, setIsPopupOpen] = useState(false);
  const [userStatuses, setUserStatuses] = useState(() =>
      users.reduce((acc, user) => {
        acc[user.id] = user.onlineStatus;
        return acc;
      }, {})
  );
  
  useEffect(() => {
    const unsubscribe = client.subscribe(
        {
          operationName: "onUserStatusChanged",
          query: `
            subscription onUserStatusChanged($roomId: String!) {
              onUserStatusChanged(roomId: $roomId) {
                isOnline
                userId
              }
            }
          `,
          variables: { roomId },
        },
        {
          next: (response) => {
            if (response.data && response.data.onUserStatusChanged) {
              const { isOnline, userId } = response.data.onUserStatusChanged;
              setUserStatuses((prevStatuses) => ({
                ...prevStatuses,
                [userId]: isOnline,
              }));
              setForceUpdate((forceUpdate) => forceUpdate + 1);
            } else {
              console.error("Unexpected subscription response format", response);
            }
          },
          error: (err) => {
            console.error("Subscription error:", err);
          }
        }
    );
    
      return () => unsubscribe();
    }, [roomId]);

  const [forceUpdate, setForceUpdate] = useState(0);

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
                {/* Status indicator */}
                <span
                    className={`inline-block h-3 w-3 rounded-full mr-2 ${
                        userStatuses[user.id] ? "bg-green-500" : "bg-gray-500"
                    }`}
                ></span>
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
