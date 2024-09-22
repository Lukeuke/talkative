import React, { useState } from 'react';
import { useLazyQuery, gql, useMutation } from '@apollo/client';

const SEARCH_USERS_QUERY = gql`
  query SearchUsers($email: String!) {
    allUsers(where: { email: { contains: $email } }) {
      email
      fullName
      id
    }
  }
`;

const INVITE_USER_MUTATION = gql`
  mutation InviteUser($roomId: UUID!, $email: String!) {
    inviteUser(roomId: $roomId, email: $email)
  }
`;

export const UserSearchPopup = ({ onClose, roomId }) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [searchUsers, { loading, data }] = useLazyQuery(SEARCH_USERS_QUERY);
  const [inviteUser] = useMutation(INVITE_USER_MUTATION);

  const handleSearch = () => {
    searchUsers({ variables: { email: searchTerm } });
  };

  const handleInvite = async (email) => {
    try {
      const response = await inviteUser({ variables: { roomId, email } });
      if (response.data.inviteUser) {
        alert('User invited successfully!');
      } else {
        alert('Failed to invite user.');
      }
    } catch (error) {
      console.error('Error inviting user:', error);
    }
  };
  const users = data?.allUsers || [];

  return (
      <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center">
        <div className="bg-MainDark p-4 rounded shadow-lg">
          <h2 className="text-lg font-bold mb-2 text-white">Search Users</h2>
          <input
              type="text"
              placeholder="Search by email"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="border p-2 mb-4 w-full rounded bg-SemiDark text-white"
          />
          <button
              className="bg-blue-500 text-white p-2 rounded mb-4"
              onClick={handleSearch}
              disabled={loading}
          >
            {loading ? 'Searching...' : 'Search'}
          </button>
          <ul className="space-y-2">
            {users.map(user => (
                <li key={user.id} className="flex justify-between items-center">
                  <span className="text-white">{user.fullName} ({user.email})</span>
                  <button
                      className="bg-blue-500 text-white p-1 rounded"
                      onClick={() => handleInvite(user.email)}
                  >
                    Invite
                  </button>
                </li>
            ))}
          </ul>
          <button
              className="mt-4 text-white p-2 rounded"
              onClick={onClose}
          >
            Close
          </button>
        </div>
      </div>
  );
};
