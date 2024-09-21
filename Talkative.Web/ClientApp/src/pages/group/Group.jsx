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

  const [sendMessage, { loadingMessageSent, errorMessageSent }] = useMutation(SEND_MESSAGE, {
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

  const handleMessageSent = async (e) => {
    e.preventDefault();
    try {
      await sendMessage();
    } catch (e) {
      console.error("Error sending message:", e);
    }
  };

  return (
      <div className="flex-1 p-4 bg-MainSemiLight h-[100vh]">
        <h1 className="text-xl font-bold border-b-[1px] border-MainLight text-white">
          {room ? room.name : 'Loading Room...'}
        </h1>
        <div className="h-[87vh] overflow-y-auto">
          {messages.map((msg) => (
              <Message key={msg.id} {...msg} />
          ))}
          <div ref={messagesEndRef} />
        </div>

        <form onSubmit={handleMessageSent}>
          <div className="flex items-center p-2 bg-MainDark rounded sticky">
            <input
                type="text"
                className="flex-1 p-2 rounded bg-MainDark text-white"
                placeholder="Type your message..."
                onChange={(e) => setMessage(e.target.value)}
                value={message}
            />
            <button type="submit" className="ml-2 p-2 bg-blue-500 text-white rounded">
              Send
            </button>
          </div>
        </form>
      </div>
  );
}