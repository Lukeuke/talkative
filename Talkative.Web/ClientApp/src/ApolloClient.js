import { ApolloClient, InMemoryCache, HttpLink, ApolloLink, split } from '@apollo/client';
import { createClient } from 'graphql-ws';
import { getMainDefinition } from "@apollo/client/utilities";
import { GraphQLWsLink } from '@apollo/client/link/subscriptions';
import {WebSocketLink} from "@apollo/client/link/ws";

const authLink = new ApolloLink((operation, forward) => {
  const token = localStorage.getItem('token');
  if (token) {
    operation.setContext({
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }
  return forward(operation);
});

const httpLink = new HttpLink({
  uri: '/graphql'
});

const wsLink = new WebSocketLink({
      uri: 'ws://localhost:5251/graphql',
      connectionParams: {
        Authorization: `Bearer ${localStorage.getItem('token')}`,
      },
      options: {
        reconnect: false
      }
    }
);
console.log("WebSocket URL:", wsLink);
const splitLink = split(
    ({ query }) => {
      const definition = getMainDefinition(query);
      return (
          definition.kind === 'OperationDefinition' &&
          definition.operation === 'subscription'
      );
    },
    wsLink,
    authLink.concat(httpLink)
);

const client = new ApolloClient({
  cache: new InMemoryCache(),
  link: splitLink,
});

export default client;
/*

import { ApolloClient, InMemoryCache, HttpLink, ApolloLink, split } from '@apollo/client';
import { createClient } from 'graphql-ws';
import { getMainDefinition } from "@apollo/client/utilities";
import { GraphQLWsLink } from '@apollo/client/link/subscriptions';

const authLink = new ApolloLink((operation, forward) => {
  const token = localStorage.getItem('token');
  if (token) {
    operation.setContext({
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }
  return forward(operation);
});

const httpLink = new HttpLink({
  uri: '/graphql'
});

const wsLink = new GraphQLWsLink(
    createClient({
      url: 'wss://localhost:7279/graphql',
      connectionParams: {
        Authorization: `Bearer ${localStorage.getItem('token')}`,
      },
      on: {
        opened: () => console.log('WebSocket connection established'),
        closed: () => console.log('WebSocket connection closed'),
        error: (error) => console.error('WebSocket error:', error),
        connected: () => console.log('WebSocket connected'),
      },
    }),
);

const splitLink = split(
    ({ query }) => {
      const definition = getMainDefinition(query);
      return (
          definition.kind === 'OperationDefinition' &&
          definition.operation === 'subscription'
      );
    },
    wsLink,
    authLink.concat(httpLink)
);

const client = new ApolloClient({
  link: splitLink,
  cache: new InMemoryCache(),
});

export default client;*/
