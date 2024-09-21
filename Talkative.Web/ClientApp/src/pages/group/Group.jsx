import { useQuery, gql, useMutation } from '@apollo/client';
import { useParams } from 'react-router-dom';
import React, { useState, useEffect, useRef } from 'react';
import { validate as isUUID } from 'uuid';
import { Message } from "../../components/Message/MessageComponent";

const GET_ALL_MESSAGES = gql`
  query GetMessages($id: UUID) {
    allRooms(where: { id: { eq: $id } }) {
      id
      name
      ownerId
      messages(order: { createdAt: ASC }) {
        content
        createdAt
        editedAt
        id
        roomId
        senderId
        sender {
          createdAt
          email
          firstName
          fullName
          id
          lastName
          username
        }
      }
      users {
        id
        username
        fullName
      }
    }
  }
`;

const SEND_MESSAGE = gql`
  mutation SendMessage($roomId: UUID!, $content: String!) {
    sendMessage(roomId: $roomId, content: $content) {
      content
      createdAt
      editedAt
      id
      roomId
      senderId
    }
  }
`;

export default function GroupPage() {
  const { id } = useParams();
  const [message, setMessage] = useState('');
  const messagesEndRef = useRef(null);

  const { loading, error, data } = useQuery(GET_ALL_MESSAGES, {
    variables: { id: id },
    skip: !id,
  });

  const [sendMessage] = useMutation(SEND_MESSAGE, {
    variables: { roomId: id, content: message },
    onCompleted: () => {
      setMessage('');
    }
  });

  useEffect(() => {
    if (messagesEndRef.current) {
      messagesEndRef.current.scrollIntoView({ behavior: "smooth" });
    }
  }, [data, message]);

  if (!isUUID(id)) {
    return <p>Invalid room ID format</p>;
  }
  if (loading) return <></>;
  if (error) return <p>Error: {error.message}</p>;

  const room = data?.allRooms?.[0];
  const messages = room?.messages || [];
  const users = room?.users || [];

  const handleMessageSent = async (e) => {
    e.preventDefault();
    try {
      await sendMessage();
    } catch (e) {
      console.error("Error sending message:", e);
    }
  };

  return (
      <div className="flex flex-col md:flex-row h-screen bg-MainSemiLight">
        <div className="flex-1 flex flex-col">
          <h1 className="text-xl font-bold border-b border-MainLight text-white p-4">
            {room ? room.name : 'Loading Room...'}
          </h1>
          <div className="flex-1 overflow-y-auto p-4 bg-MainDark custom-overflow">
            <div className="flex flex-col space-y-2">
              {messages.map((msg) => (
                  <Message key={msg.id} {...msg} />
              ))}
              <div ref={messagesEndRef} />
            </div>
          </div>

          <form onSubmit={handleMessageSent} className="p-2 bg-MainDark">
            <div className="flex items-center">
              <input
                  type="text"
                  className="flex-1 p-2 rounded bg-MainDark text-white border border-gray-700 focus:border-blue-500 focus:outline-none"
                  placeholder="Type your message..."
                  onChange={(e) => setMessage(e.target.value)}
                  value={message}
              />
              <button type="submit" className="ml-2 p-2 bg-blue-500 text-white rounded hover:bg-blue-600">
                Send
              </button>
            </div>
          </form>
        </div>

        {/* Right Panel for Users */}
        <div className="w-64 bg-MainDark border-l border-gray-700 p-4 md:block hidden overflow-y-auto">
          <h2 className="text-lg font-bold text-white mb-2">Users</h2>
          <ul className="space-y-2">
            {users.map(user => (
                <li key={user.id} className="text-white hover:bg-SemiDark p-2 rounded user-list">
                  {user.fullName} ({user.username})
                </li>
            ))}
          </ul>
        </div>
      </div>
  );
}
