import React, { useEffect, useState } from 'react';
import { createClient } from 'graphql-sse';

const client = createClient({
  url: '/graphql',
  headers: {
    Authorization: `Bearer ${localStorage.getItem('token')}`,
  }
});

const Test = () => {
  const [messages, setMessages] = useState([]);

  useEffect(() => {
    const MESSAGES_QUERY = `subscription {
      allGroupMessages {
        content
        createdAt
        editedAt
        roomId
        sender {
          createdAt
          email
          firstName
          fullName
          lastName
          username
        }
      }
    }`;

    client.subscribe(
        { query: MESSAGES_QUERY },
        {
          next(data) {
            console.log('Received data:', data);
            setMessages(prevMessages => [...prevMessages, data.data.allGroupMessages]);
          },
          error(err) {
            console.error('Subscription error:', err);
          },
          complete() {
            console.log('Subscription complete');
          },
        }
    );
  }, []);

  return (
      <div className="message-container">
        {messages.map((message, index) => (
            <div key={index} className="message">
              <strong>{message.sender.username}:</strong> {message.content}
              <span>{new Date(message.createdAt).toLocaleString()}</span>
            </div>
        ))}
      </div>
  );
};

export default Test;
