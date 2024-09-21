import { ApolloClient, InMemoryCache, HttpLink, ApolloLink } from '@apollo/client';

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

const client = new ApolloClient({
  link: authLink.concat(new HttpLink({ uri: '/graphql' })),
  cache: new InMemoryCache(),
});

export default client;
