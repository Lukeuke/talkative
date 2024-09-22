import React from 'react';
import CreateGroup from "./Group/CreateGroup";
import { useQuery, gql, useMutation } from "@apollo/client";
import { refreshNavMenu } from "./NavMenu";

const INVITES_QUERY = gql`query {
  allInvites {
    id
    roomId
    roomName
  }
}`;

const ACCEPT_INVITE_MUTATION = gql`
  mutation AcceptInvite($inviteId: UUID!) {
    acceptInvite(inviteId: $inviteId) {
      id
      name
      ownerId
    }
  }
`;

const GET_ALL_ROOMS = gql`
  query {
    allRooms {
      id
      name
      ownerId
    }
  }
`;

export const Home = () => {
  const { loading, error, data } = useQuery(INVITES_QUERY);

  const { refetch: refetchRooms } = useQuery(GET_ALL_ROOMS);

  const [acceptInvite] = useMutation(ACCEPT_INVITE_MUTATION, {
    onCompleted: (data) => {
      refreshNavMenu(refetchRooms);
    },
    onError: (error) => {
      console.error("Error accepting invite:", error);
    },
  });
  
  if (loading) return <div>Loading...</div>;
  if (error) {
    console.error("Error fetching invites:", error);
    return <div>Error fetching invites</div>;
  }

  const invites = data.allInvites || [];
  const handleAccept = (inviteId) => {
    acceptInvite({ variables: { inviteId } });
  };

  return (
      <main className="flex min-h-screen text-white">
        <div className="grid grid-cols-1 md:grid-cols-3 w-full h-[100vh]">
          <div className="bg-gray-800 h-full p-4 flex justify-center">Column 1</div>
          <div className="bg-gray-700 h-full p-4 flex flex-col items-center justify-center">
            <div className="text-[2rem] mb-4">Create a Group</div>
            <CreateGroup />
          </div>
          <div className="bg-gray-600 h-full p-4 flex flex-col">
            <h2 className="text-lg font-bold mb-2">Invites:</h2>
            {invites.length > 0 ? (
                <ul className="space-y-4">
                  {invites.map(invite => {
                    return(
                    <li key={invite.id} className="bg-gray-700 p-4 rounded-lg flex items-center justify-between shadow-md transition-transform transform hover:scale-105 user-list">
                        <div className="flex-1">
                          You've got invited to <span className="font-bold text-lg">{invite.roomName}</span>!
                        </div>
                        <div className="flex gap-2">
                          <button 
                              className="bg-blue-600 text-white p-2 rounded hover:bg-blue-700 transition-colors"
                              onClick={() => handleAccept(invite.id)}
                          >
                            Accept
                          </button>
                          <button className="bg-red-600 text-white p-2 rounded hover:bg-red-700 transition-colors">
                            Decline
                          </button>
                        </div>
                      </li>)
                  })}
                </ul>
            ) : (
                <p>No one invited you ): :sad:</p>
            )}
          </div>
        </div>
      </main>
  );
};
